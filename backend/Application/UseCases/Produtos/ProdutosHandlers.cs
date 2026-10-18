using System.Net;
using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Events;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Produtos;
using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Events;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Application.UseCases.Produtos;

public class CriarProdutoHandler : IRequestHandler<CriarProdutoCommand, Guid>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IRelogio _relogio;

    public CriarProdutoHandler(
        IProdutoRepository produtoRepository,
        IApplicationDbContext context,
        IUsuarioAtualService usuarioAtualService,
        IRelogio relogio)
    {
        _produtoRepository = produtoRepository;
        _context = context;
        _usuarioAtualService = usuarioAtualService;
        _relogio = relogio;
    }

    public async Task<Guid> Handle(CriarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = Produto.Criar(
            _usuarioAtualService.ObterRestauranteIdObrigatorio(),
            request.Nome,
            request.Descricao,
            Dinheiro.Criar(request.Preco),
            request.CategoriaId,
            _relogio.AgoraUtc,
            Quantidade.Criar(request.QuantidadeInicial));

        _produtoRepository.Adicionar(produto);
        await _context.SaveChangesAsync(cancellationToken);

        return produto.Id;
    }
}

public class ExcluirProdutoHandler : IRequestHandler<ExcluirProdutoCommand>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoEstoqueRepository;
    private readonly IApplicationDbContext _context;

    public ExcluirProdutoHandler(
        IProdutoRepository produtoRepository,
        IMovimentacaoEstoqueRepository movimentacaoEstoqueRepository,
        IApplicationDbContext context)
    {
        _produtoRepository = produtoRepository;
        _movimentacaoEstoqueRepository = movimentacaoEstoqueRepository;
        _context = context;
    }

    public async Task Handle(ExcluirProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (produto is null)
            throw new NaoEncontradoException($"Produto com Id '{request.Id}' não encontrado.");

        var temMovimentacoes = await _movimentacaoEstoqueRepository.ExisteParaProdutoAsync(request.Id, cancellationToken);

        if (temMovimentacoes)
            throw new DomainException("Não é possível excluir um produto que possui movimentações de estoque vinculadas.");

        _produtoRepository.Remover(produto);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class AtualizarProdutoHandler : IRequestHandler<AtualizarProdutoCommand>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IApplicationDbContext _context;

    public AtualizarProdutoHandler(IProdutoRepository produtoRepository, IApplicationDbContext context)
    {
        _produtoRepository = produtoRepository;
        _context = context;
    }

    public async Task Handle(AtualizarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (produto is null)
            throw new NaoEncontradoException($"Produto com Id '{request.Id}' não encontrado.");

        produto.Atualizar(request.Nome, request.Descricao, Dinheiro.Criar(request.Preco), request.CategoriaId);

        _produtoRepository.Atualizar(produto);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class ObterProdutoPorIdHandler : IRequestHandler<ObterProdutoPorIdQuery, ProdutoDetalhesDto?>
{
    private readonly IProdutoRepository _produtoRepository;

    public ObterProdutoPorIdHandler(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<ProdutoDetalhesDto?> Handle(ObterProdutoPorIdQuery request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (produto is null)
            return null;

        return new ProdutoDetalhesDto(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.Preco.Valor,
            produto.QuantidadeEmEstoque.Valor,
            produto.CategoriaId,
            produto.Categoria!.Nome,
            produto.CriadoEm);
    }
}

public class ListarProdutosHandler : IRequestHandler<ListarProdutosQuery, ResultadoPaginado<ProdutoItemListaDto>>
{
    private readonly IProdutoRepository _produtoRepository;

    public ListarProdutosHandler(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<ResultadoPaginado<ProdutoItemListaDto>> Handle(ListarProdutosQuery request, CancellationToken cancellationToken)
    {
        var (produtos, totalItens) = await _produtoRepository.ListarPaginadoAsync(
            request.Pagina,
            request.TamanhoPagina,
            request.Nome,
            request.CategoriaId,
            request.OrdenarPor,
            request.DirecaoOrdenacao,
            cancellationToken);

        var itens = produtos
            .Select(produto => new ProdutoItemListaDto(
                produto.Id,
                produto.Nome,
                produto.Preco.Valor,
                produto.QuantidadeEmEstoque.Valor,
                produto.CategoriaId,
                produto.Categoria!.Nome,
                produto.CriadoEm))
            .ToList();

        return new ResultadoPaginado<ProdutoItemListaDto>(itens, request.Pagina, request.TamanhoPagina, totalItens);
    }
}

public class AvisarEstoqueBaixoHandler : INotificationHandler<DomainEventNotification<EstoqueFicouBaixoEvent>>
{
    private readonly IOutboxEmailRepository _outboxEmailRepository;
    private readonly IRelogio _relogio;

    public AvisarEstoqueBaixoHandler(IOutboxEmailRepository outboxEmailRepository, IRelogio relogio)
    {
        _outboxEmailRepository = outboxEmailRepository;
        _relogio = relogio;
    }

    public Task Handle(DomainEventNotification<EstoqueFicouBaixoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.Evento;

        var nomeProduto = WebUtility.HtmlEncode(evento.NomeProduto);
        var limite = Produto.LimiteEstoqueBaixo.Valor;

        var assunto = $"Estoque baixo: {evento.NomeProduto}";
        var corpoHtml = $"""
            <p>Olá,</p>
            <p>O produto <b>{nomeProduto}</b> está com o estoque baixo:
            restam <b>{evento.QuantidadeAtual}</b> unidade(s) (o limite é {limite}).</p>
            <p>Considere fazer uma nova compra.</p>
            <p style="color:#888">Aviso automático do Controle de Estoque.</p>
            """;

        _outboxEmailRepository.Adicionar(OutboxEmail.Criar(evento.RestauranteId, assunto, corpoHtml, _relogio.AgoraUtc));

        return Task.CompletedTask;
    }
}
