using MediatR;
using Microsoft.Extensions.Logging;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Repositories;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Services;
using RestauranteEstoque.Pagamentos.Contracts.Pagamentos;
using RestauranteEstoque.Pagamentos.Domain.Entities;
using RestauranteEstoque.Pagamentos.Domain.Enums;

namespace RestauranteEstoque.Pagamentos.Application.UseCases.Pagamentos;

public class CriarPagamentoHandler : IRequestHandler<CriarPagamentoCommand, PagamentoCriadoResponse>
{
    private readonly IPagamentoRepository _repository;
    private readonly IApplicationDbContext _context;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IRelogio _relogio;

    public CriarPagamentoHandler(
        IPagamentoRepository repository,
        IApplicationDbContext context,
        IUsuarioAtualService usuarioAtualService,
        IRelogio relogio)
    {
        _repository = repository;
        _context = context;
        _usuarioAtualService = usuarioAtualService;
        _relogio = relogio;
    }

    public async Task<PagamentoCriadoResponse> Handle(CriarPagamentoCommand request, CancellationToken cancellationToken)
    {
        var restauranteId = _usuarioAtualService.ObterRestauranteIdObrigatorio();

        var existente = await _repository.ObterPorChaveIdempotenciaAsync(request.ChaveIdempotencia, cancellationToken);
        if (existente is not null)
        {
            if (!existente.EhMesmoPedido(request.VendaId, request.Valor, request.TokenCartao))
                throw new DomainException("Esse Idempotency-Key já foi usado num pedido diferente. Gere uma chave nova para um pagamento novo.");

            return new PagamentoCriadoResponse(existente.Id, existente.Status.ToString());
        }

        var pagamento = Pagamento.Criar(
            restauranteId,
            request.VendaId,
            Dinheiro.Criar(request.Valor),
            request.TokenCartao,
            request.ChaveIdempotencia,
            _relogio.AgoraUtc);

        _repository.Adicionar(pagamento);
        await _context.SaveChangesAsync(cancellationToken);

        return new PagamentoCriadoResponse(pagamento.Id, pagamento.Status.ToString());
    }
}

public class ProcessarPagamentoHandler : IRequestHandler<ProcessarPagamentoCommand>
{
    private readonly IPagamentoRepository _repository;
    private readonly IApplicationDbContext _context;
    private readonly IOutboxMensagemRepository _outbox;
    private readonly IGatewayPagamento _gateway;
    private readonly IRelogio _relogio;
    private readonly ILogger<ProcessarPagamentoHandler> _logger;

    public ProcessarPagamentoHandler(
        IPagamentoRepository repository,
        IOutboxMensagemRepository outbox,
        IApplicationDbContext context,
        IGatewayPagamento gateway,
        IRelogio relogio,
        ILogger<ProcessarPagamentoHandler> logger)
    {
        _repository = repository;
        _outbox = outbox;
        _context = context;
        _gateway = gateway;
        _relogio = relogio;
        _logger = logger;
    }

    public async Task Handle(ProcessarPagamentoCommand request, CancellationToken cancellationToken)
    {
        var pagamento = await _repository.ObterPorIdAsync(request.PagamentoId, cancellationToken);

        if (pagamento is null || pagamento.Status != StatusPagamento.Pendente)
            return;

        CobrancaAceita cobranca;
        try
        {
            cobranca = await _gateway.CobrarAsync(
                new PedidoCobranca(pagamento.Id, pagamento.RestauranteId, pagamento.Valor.Valor, pagamento.TokenCartao),
                cancellationToken);
        }
        catch (GatewayIndisponivelException ex)
        {
            var desistiu = pagamento.RegistrarFalhaGateway(_relogio.AgoraUtc);
            _logger.LogWarning(ex, "Gateway indisponível ao cobrar o pagamento {PagamentoId} (tentativa {Tentativa}).", pagamento.Id, pagamento.Tentativas);

            _repository.Atualizar(pagamento);
            if (desistiu)
                AvisarVenda(pagamento);
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }
        catch (CobrancaRecusadaException ex)
        {
            pagamento.Recusar(ex.Message, _relogio.AgoraUtc);
            _repository.Atualizar(pagamento);
            AvisarVenda(pagamento);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Pagamento {PagamentoId} recusado na hora pelo gateway: {Motivo}.", pagamento.Id, ex.Message);
            return;
        }

        pagamento.MarcarComoProcessando(cobranca.IdTransacao);
        _repository.Atualizar(pagamento);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pagamento {PagamentoId} aceito pelo gateway ({IdTransacao}); esperando o webhook.", pagamento.Id, cobranca.IdTransacao);
    }

    private void AvisarVenda(Pagamento pagamento) =>
        _outbox.Adicionar(PagamentoSaga.ResultadoParaVenda(pagamento, _relogio.AgoraUtc));
}

public class ProcessarWebhookGatewayHandler : IRequestHandler<ProcessarWebhookGatewayCommand>
{
    private readonly IPagamentoRepository _repository;
    private readonly IOutboxMensagemRepository _outbox;
    private readonly IDefinidorRestauranteAtual _definidorRestaurante;
    private readonly IApplicationDbContext _context;
    private readonly IRelogio _relogio;
    private readonly ILogger<ProcessarWebhookGatewayHandler> _logger;

    public ProcessarWebhookGatewayHandler(
        IPagamentoRepository repository,
        IOutboxMensagemRepository outbox,
        IDefinidorRestauranteAtual definidorRestaurante,
        IApplicationDbContext context,
        IRelogio relogio,
        ILogger<ProcessarWebhookGatewayHandler> logger)
    {
        _repository = repository;
        _outbox = outbox;
        _definidorRestaurante = definidorRestaurante;
        _context = context;
        _relogio = relogio;
        _logger = logger;
    }

    public async Task Handle(ProcessarWebhookGatewayCommand request, CancellationToken cancellationToken)
    {
        _definidorRestaurante.Definir(request.RestauranteId);

        var pagamento = await _repository.ObterPorIdAsync(request.PagamentoId, cancellationToken);
        if (pagamento is null)
        {
            _logger.LogWarning("Webhook de um pagamento que não existe: {PagamentoId}.", request.PagamentoId);
            return;
        }

        if (pagamento.Status != StatusPagamento.Processando)
        {
            _logger.LogInformation("Webhook repetido do pagamento {PagamentoId} (já está {Status}); ignorado.", pagamento.Id, pagamento.Status);
            return;
        }

        if (pagamento.IdTransacaoGateway != request.IdTransacao)
        {
            _logger.LogWarning("Webhook do pagamento {PagamentoId} com transação {IdTransacao} diferente da esperada; ignorado.", pagamento.Id, request.IdTransacao);
            return;
        }

        if (request.Aprovado)
            pagamento.Aprovar(_relogio.AgoraUtc);
        else
            pagamento.Recusar(request.Motivo ?? "Pagamento recusado.", _relogio.AgoraUtc);

        _repository.Atualizar(pagamento);
        _outbox.Adicionar(PagamentoSaga.ResultadoParaVenda(pagamento, _relogio.AgoraUtc));
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pagamento {PagamentoId} da venda {VendaId}: {Status}.", pagamento.Id, pagamento.VendaId, pagamento.Status);
    }
}

file static class PagamentoSaga
{
    public static OutboxMensagem ResultadoParaVenda(Pagamento pagamento, DateTime agora)
    {
        var resultado = new ResultadoPagamentoMensagem(
            pagamento.VendaId,
            pagamento.RestauranteId,
            pagamento.Id,
            pagamento.Status == StatusPagamento.Aprovado,
            pagamento.MotivoRecusa);

        return OutboxMensagem.Criar(Filas.ResultadoPagamento, resultado, agora);
    }
}

public class ObterPagamentoPorIdHandler : IRequestHandler<ObterPagamentoPorIdQuery, PagamentoDetalhesDto?>
{
    private readonly IPagamentoRepository _repository;

    public ObterPagamentoPorIdHandler(IPagamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagamentoDetalhesDto?> Handle(ObterPagamentoPorIdQuery request, CancellationToken cancellationToken)
    {
        var pagamento = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
        return pagamento?.ParaDetalhesDto();
    }
}

public class ObterPagamentoPorVendaHandler : IRequestHandler<ObterPagamentoPorVendaQuery, PagamentoDetalhesDto?>
{
    private readonly IPagamentoRepository _repository;

    public ObterPagamentoPorVendaHandler(IPagamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagamentoDetalhesDto?> Handle(ObterPagamentoPorVendaQuery request, CancellationToken cancellationToken)
    {
        var pagamento = await _repository.ObterPorVendaAsync(request.VendaId, cancellationToken);
        return pagamento?.ParaDetalhesDto();
    }
}

file static class PagamentoMapeamentos
{
    public static PagamentoDetalhesDto ParaDetalhesDto(this Pagamento pagamento) => new(
        pagamento.Id,
        pagamento.VendaId,
        pagamento.Valor.Valor,
        pagamento.Status.ToString(),
        pagamento.CriadoEm,
        pagamento.ProcessadoEm,
        pagamento.IdTransacaoGateway,
        pagamento.MotivoRecusa);
}
