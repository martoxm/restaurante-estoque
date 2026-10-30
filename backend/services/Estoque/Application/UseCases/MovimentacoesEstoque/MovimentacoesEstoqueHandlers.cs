using Microsoft.Extensions.Logging;
using RestauranteEstoque.Core.Application.Abstractions;
using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Domain.Services;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;

public class CriarMovimentacaoEstoqueHandler : IRequestHandler<CriarMovimentacaoEstoqueCommand, Guid>
{
    private readonly IMovimentacaoEstoqueRepository _movimentacaoEstoqueRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IApplicationDbContext _context;
    private readonly IRelogio _relogio;

    public CriarMovimentacaoEstoqueHandler(
        IMovimentacaoEstoqueRepository movimentacaoEstoqueRepository,
        IProdutoRepository produtoRepository,
        IApplicationDbContext context,
        IRelogio relogio)
    {
        _movimentacaoEstoqueRepository = movimentacaoEstoqueRepository;
        _produtoRepository = produtoRepository;
        _context = context;
        _relogio = relogio;
    }

    public async Task<Guid> Handle(CriarMovimentacaoEstoqueCommand request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.ProdutoId, cancellationToken);

        if (produto is null)
            throw new NaoEncontradoException($"Produto com Id '{request.ProdutoId}' não foi encontrado.");

        var quantidade = Quantidade.Criar(request.Quantidade);

        var movimentacao = MovimentadorEstoque.Movimentar(
            produto,
            request.Tipo,
            quantidade,
            _relogio.AgoraUtc,
            request.Observacoes);

        _produtoRepository.Atualizar(produto);
        _movimentacaoEstoqueRepository.Adicionar(movimentacao);
        await _context.SaveChangesAsync(cancellationToken);

        return movimentacao.Id;
    }
}

public class BaixarEstoqueVendaHandler : IRequestHandler<BaixarEstoqueVendaCommand>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoEstoqueRepository;
    private readonly IOutboxMensagemRepository _outbox;
    private readonly IMensagemProcessadaRepository _mensagensProcessadas;
    private readonly IDefinidorRestauranteAtual _definidorRestaurante;
    private readonly IApplicationDbContext _context;
    private readonly IRelogio _relogio;

    public BaixarEstoqueVendaHandler(
        IProdutoRepository produtoRepository,
        IMovimentacaoEstoqueRepository movimentacaoEstoqueRepository,
        IOutboxMensagemRepository outbox,
        IMensagemProcessadaRepository mensagensProcessadas,
        IDefinidorRestauranteAtual definidorRestaurante,
        IApplicationDbContext context,
        IRelogio relogio)
    {
        _produtoRepository = produtoRepository;
        _movimentacaoEstoqueRepository = movimentacaoEstoqueRepository;
        _outbox = outbox;
        _mensagensProcessadas = mensagensProcessadas;
        _definidorRestaurante = definidorRestaurante;
        _context = context;
        _relogio = relogio;
    }

    public async Task Handle(BaixarEstoqueVendaCommand request, CancellationToken cancellationToken)
    {
        if (await _mensagensProcessadas.ExisteAsync(request.MensagemId, cancellationToken))
            return;

        _definidorRestaurante.Definir(request.RestauranteId);

        var agora = _relogio.AgoraUtc;

        var aBaixar = new List<(Produto Produto, Quantidade Quantidade)>();
        string? motivo = null;
        foreach (var item in request.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId, cancellationToken);
            var quantidade = Quantidade.Criar(item.Quantidade);

            if (produto is null)
            {
                motivo = $"Produto '{item.ProdutoId}' não encontrado no estoque.";
                break;
            }

            if (produto.QuantidadeEmEstoque < quantidade)
            {
                motivo = $"Estoque insuficiente para '{produto.Nome}': tem {produto.QuantidadeEmEstoque.Valor}, a venda pede {quantidade.Valor}.";
                break;
            }

            aBaixar.Add((produto, quantidade));
        }

        var baixado = motivo is null;
        if (baixado)
        {
            foreach (var (produto, quantidade) in aBaixar)
            {
                var movimentacao = MovimentadorEstoque.Movimentar(
                    produto, TipoMovimentacaoEstoque.Saida, quantidade, agora, $"Venda {request.VendaId}");

                _produtoRepository.Atualizar(produto);
                _movimentacaoEstoqueRepository.Adicionar(movimentacao);
            }
        }

        var resultado = new ResultadoBaixaEstoqueMensagem(request.VendaId, request.RestauranteId, baixado, motivo);
        _outbox.Adicionar(OutboxMensagem.Criar(Filas.ResultadoBaixaEstoque, resultado, agora));
        _mensagensProcessadas.Adicionar(MensagemProcessada.Criar(request.MensagemId, Filas.BaixarEstoque, agora));

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class DevolverEstoqueVendaHandler : IRequestHandler<DevolverEstoqueVendaCommand>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoEstoqueRepository;
    private readonly IMensagemProcessadaRepository _mensagensProcessadas;
    private readonly IDefinidorRestauranteAtual _definidorRestaurante;
    private readonly IApplicationDbContext _context;
    private readonly IRelogio _relogio;
    private readonly ILogger<DevolverEstoqueVendaHandler> _logger;

    public DevolverEstoqueVendaHandler(
        IProdutoRepository produtoRepository,
        IMovimentacaoEstoqueRepository movimentacaoEstoqueRepository,
        IMensagemProcessadaRepository mensagensProcessadas,
        IDefinidorRestauranteAtual definidorRestaurante,
        IApplicationDbContext context,
        IRelogio relogio,
        ILogger<DevolverEstoqueVendaHandler> logger)
    {
        _produtoRepository = produtoRepository;
        _movimentacaoEstoqueRepository = movimentacaoEstoqueRepository;
        _mensagensProcessadas = mensagensProcessadas;
        _definidorRestaurante = definidorRestaurante;
        _context = context;
        _relogio = relogio;
        _logger = logger;
    }

    public async Task Handle(DevolverEstoqueVendaCommand request, CancellationToken cancellationToken)
    {
        if (await _mensagensProcessadas.ExisteAsync(request.MensagemId, cancellationToken))
            return;

        _definidorRestaurante.Definir(request.RestauranteId);
        var agora = _relogio.AgoraUtc;

        foreach (var item in request.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId, cancellationToken);

            if (produto is null)
            {
                _logger.LogWarning("Devolução da venda {VendaId}: produto {ProdutoId} não existe mais.", request.VendaId, item.ProdutoId);
                continue;
            }

            var movimentacao = MovimentadorEstoque.Movimentar(
                produto, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(item.Quantidade), agora,
                $"Estorno da venda {request.VendaId}: {request.Motivo}");

            _produtoRepository.Atualizar(produto);
            _movimentacaoEstoqueRepository.Adicionar(movimentacao);
        }

        _mensagensProcessadas.Adicionar(MensagemProcessada.Criar(request.MensagemId, Filas.DevolverEstoque, agora));
        await _context.SaveChangesAsync(cancellationToken);
    }
}
