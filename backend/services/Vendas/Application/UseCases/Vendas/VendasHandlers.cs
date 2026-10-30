using MediatR;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Vendas.Application.Abstractions.Repositories;
using RestauranteEstoque.Vendas.Application.Abstractions.Services;
using RestauranteEstoque.Vendas.Contracts.Vendas;
using RestauranteEstoque.Vendas.Domain.Entities;
using RestauranteEstoque.Vendas.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace RestauranteEstoque.Vendas.Application.UseCases.Vendas;

public class CriarVendaHandler : IRequestHandler<CriarVendaCommand, Guid>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IOutboxMensagemRepository _outbox;
    private readonly IProdutoCatalogoRepository _catalogo;
    private readonly IApplicationDbContext _context;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IRelogio _relogio;

    public CriarVendaHandler(
        IVendaRepository vendaRepository,
        IOutboxMensagemRepository outbox,
        IProdutoCatalogoRepository catalogo,
        IApplicationDbContext context,
        IUsuarioAtualService usuarioAtualService,
        IRelogio relogio)
    {
        _vendaRepository = vendaRepository;
        _outbox = outbox;
        _catalogo = catalogo;
        _context = context;
        _usuarioAtualService = usuarioAtualService;
        _relogio = relogio;
    }

    public async Task<Guid> Handle(CriarVendaCommand request, CancellationToken cancellationToken)
    {
        var restauranteId = _usuarioAtualService.ObterRestauranteIdObrigatorio();

        var itens = new List<NovoItemVenda>();
        foreach (var item in request.Itens)
        {
            var produto = await _catalogo.ObterPorIdAsync(item.ProdutoId, cancellationToken);
            if (produto is null || produto.Excluido)
                throw new NaoEncontradoException($"Produto com Id '{item.ProdutoId}' não encontrado.");

            itens.Add(new NovoItemVenda(produto.Id, produto.Nome, produto.Preco, Quantidade.Criar(item.Quantidade)));
        }

        var venda = Venda.Criar(restauranteId, _relogio.AgoraUtc, itens);

        _vendaRepository.Adicionar(venda);

        var pedido = new BaixarEstoqueMensagem(
            venda.Id,
            restauranteId,
            venda.Itens.Select(item => new ItemBaixaEstoque(item.ProdutoId, item.Quantidade.Valor)).ToList());
        _outbox.Adicionar(OutboxMensagem.Criar(Filas.BaixarEstoque, pedido, _relogio.AgoraUtc));
        await _context.SaveChangesAsync(cancellationToken);

        return venda.Id;
    }
}

public class ObterVendaPorIdHandler : IRequestHandler<ObterVendaPorIdQuery, VendaDetalhesDto?>
{
    private readonly IVendaRepository _vendaRepository;

    public ObterVendaPorIdHandler(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public async Task<VendaDetalhesDto?> Handle(ObterVendaPorIdQuery request, CancellationToken cancellationToken)
    {
        var venda = await _vendaRepository.ObterPorIdAsync(request.Id, cancellationToken);

        return venda?.ParaDetalhesDto();
    }
}

public class ListarVendasHandler : IRequestHandler<ListarVendasQuery, ResultadoPaginado<VendaItemListaDto>>
{
    private readonly IVendaRepository _vendaRepository;

    public ListarVendasHandler(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public async Task<ResultadoPaginado<VendaItemListaDto>> Handle(ListarVendasQuery request, CancellationToken cancellationToken)
    {
        var (vendas, totalItens) = await _vendaRepository.ListarPaginadoAsync(
            request.Pagina,
            request.TamanhoPagina,
            request.DirecaoOrdenacao,
            cancellationToken);

        var itens = vendas
            .Select(venda => venda.ParaItemListaDto())
            .ToList();

        return new ResultadoPaginado<VendaItemListaDto>(itens, request.Pagina, request.TamanhoPagina, totalItens);
    }
}

public class ProcessarResultadoBaixaHandler : IRequestHandler<ProcessarResultadoBaixaCommand>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IMensagemProcessadaRepository _mensagensProcessadas;
    private readonly IDefinidorRestauranteAtual _definidorRestaurante;
    private readonly IApplicationDbContext _context;
    private readonly IRelogio _relogio;
    private readonly ILogger<ProcessarResultadoBaixaHandler> _logger;

    public ProcessarResultadoBaixaHandler(
        IVendaRepository vendaRepository,
        IMensagemProcessadaRepository mensagensProcessadas,
        IDefinidorRestauranteAtual definidorRestaurante,
        IApplicationDbContext context,
        IRelogio relogio,
        ILogger<ProcessarResultadoBaixaHandler> logger)
    {
        _vendaRepository = vendaRepository;
        _mensagensProcessadas = mensagensProcessadas;
        _definidorRestaurante = definidorRestaurante;
        _context = context;
        _relogio = relogio;
        _logger = logger;
    }

    public async Task Handle(ProcessarResultadoBaixaCommand request, CancellationToken cancellationToken)
    {
        if (await _mensagensProcessadas.ExisteAsync(request.MensagemId, cancellationToken))
            return;

        var resultado = request.Resultado;
        _definidorRestaurante.Definir(resultado.RestauranteId);

        var venda = await _vendaRepository.ObterPorIdAsync(resultado.VendaId, cancellationToken);

        if (venda is null)
            _logger.LogWarning("Resultado da baixa chegou pra venda {VendaId}, que não foi encontrada.", resultado.VendaId);
        else if (venda.Status != StatusVenda.Pendente)
            _logger.LogWarning("Venda {VendaId} já está {Status}: resultado da baixa ignorado.", venda.Id, venda.Status);
        else
        {
            if (resultado.Baixado)
                venda.Confirmar();
            else
            {
                venda.Cancelar();
                _logger.LogInformation("Venda {VendaId} cancelada: {Motivo}", venda.Id, resultado.Motivo);
            }

            _vendaRepository.Atualizar(venda);
        }

        _mensagensProcessadas.Adicionar(MensagemProcessada.Criar(request.MensagemId, Filas.ResultadoBaixaEstoque, _relogio.AgoraUtc));
        await _context.SaveChangesAsync(cancellationToken);
    }
}

file static class VendaMapeamentos
{
    public static VendaDetalhesDto ParaDetalhesDto(this Venda venda) => new(
        venda.Id,
        venda.Status.ToString(),
        venda.CriadoEm,
        venda.Total.Valor,
        venda.Itens.Select(item => new ItemVendaDto(
            item.ProdutoId,
            item.NomeProduto,
            item.PrecoUnitario.Valor,
            item.Quantidade.Valor,
            item.Subtotal.Valor)).ToList());

    public static VendaItemListaDto ParaItemListaDto(this Venda venda) => new(
        venda.Id,
        venda.Status.ToString(),
        venda.CriadoEm,
        venda.Itens.Count,
        venda.Total.Valor);
}
