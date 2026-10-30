using RestauranteEstoque.Core.Application.Abstractions;
using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Core.Application.Events;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Contracts.Produtos;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Events;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using System.Net;

namespace RestauranteEstoque.Application.UseCases.Produtos;

public class CriarProdutoHandler : IRequestHandler<CriarProdutoCommand, Guid>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IRelogio _relogio;
    private readonly IOutboxMensagemRepository _outbox;

    public CriarProdutoHandler(
        IProdutoRepository produtoRepository,
        ICategoriaRepository categoriaRepository,
        IApplicationDbContext context,
        IUsuarioAtualService usuarioAtualService,
        IRelogio relogio,
        IOutboxMensagemRepository outbox)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _context = context;
        _usuarioAtualService = usuarioAtualService;
        _relogio = relogio;
        _outbox = outbox;
    }

    public async Task<Guid> Handle(CriarProdutoCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(request.CategoriaId, cancellationToken);

        if (categoria is null)
            throw new NaoEncontradoException($"Categoria com Id '{request.CategoriaId}' não encontrada.");

        var produto = Produto.Criar(
            _usuarioAtualService.ObterRestauranteIdObrigatorio(),
            request.Nome,
            request.Descricao,
            Dinheiro.Criar(request.Preco),
            categoria,
            _relogio.AgoraUtc,
            Quantidade.Criar(request.QuantidadeInicial));

        _produtoRepository.Adicionar(produto);

        _outbox.Adicionar(CatalogoProdutosMensagens.Alterado(produto, _relogio.AgoraUtc));
        await _context.SaveChangesAsync(cancellationToken);

        return produto.Id;
    }
}

public class ExcluirProdutoHandler : IRequestHandler<ExcluirProdutoCommand>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoEstoqueRepository;
    private readonly IApplicationDbContext _context;
    private readonly IOutboxMensagemRepository _outbox;
    private readonly IRelogio _relogio;

    public ExcluirProdutoHandler(
        IProdutoRepository produtoRepository,
        IMovimentacaoEstoqueRepository movimentacaoEstoqueRepository,
        IApplicationDbContext context,
        IOutboxMensagemRepository outbox,
        IRelogio relogio)
    {
        _produtoRepository = produtoRepository;
        _movimentacaoEstoqueRepository = movimentacaoEstoqueRepository;
        _context = context;
        _outbox = outbox;
        _relogio = relogio;
    }

    public async Task Handle(ExcluirProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (produto is null)
            throw new NaoEncontradoException($"Produto com Id '{request.Id}' não encontrado.");

        var possuiMovimentacoes = await _movimentacaoEstoqueRepository.ExisteParaProdutoAsync(request.Id, cancellationToken);
        produto.GarantirQuePodeSerExcluido(possuiMovimentacoes);

        _produtoRepository.Remover(produto);
        _outbox.Adicionar(CatalogoProdutosMensagens.Excluido(produto, _relogio.AgoraUtc));
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class AtualizarProdutoHandler : IRequestHandler<AtualizarProdutoCommand>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _context;
    private readonly IOutboxMensagemRepository _outbox;
    private readonly IRelogio _relogio;

    public AtualizarProdutoHandler(
        IProdutoRepository produtoRepository,
        ICategoriaRepository categoriaRepository,
        IApplicationDbContext context,
        IOutboxMensagemRepository outbox,
        IRelogio relogio)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _context = context;
        _outbox = outbox;
        _relogio = relogio;
    }

    public async Task Handle(AtualizarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (produto is null)
            throw new NaoEncontradoException($"Produto com Id '{request.Id}' não encontrado.");

        var categoria = await _categoriaRepository.ObterPorIdAsync(request.CategoriaId, cancellationToken);

        if (categoria is null)
            throw new NaoEncontradoException($"Categoria com Id '{request.CategoriaId}' não encontrada.");

        produto.Atualizar(request.Nome, request.Descricao, Dinheiro.Criar(request.Preco), categoria);

        _produtoRepository.Atualizar(produto);
        _outbox.Adicionar(CatalogoProdutosMensagens.Alterado(produto, _relogio.AgoraUtc));

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

        return produto.ParaDetalhesDto();
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
            .Select(produto => produto.ParaItemListaDto())
            .ToList();

        return new ResultadoPaginado<ProdutoItemListaDto>(itens, request.Pagina, request.TamanhoPagina, totalItens);
    }
}

public class AvisarEstoqueBaixoHandler : INotificationHandler<DomainEventNotification<EstoqueFicouBaixoEvent>>
{
    private readonly IOutboxMensagemRepository _outboxMensagemRepository;
    private readonly IRelogio _relogio;

    public AvisarEstoqueBaixoHandler(IOutboxMensagemRepository outboxMensagemRepository, IRelogio relogio)
    {
        _outboxMensagemRepository = outboxMensagemRepository;
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

        var mensagem = new EnviarEmailMensagem(evento.RestauranteId, assunto, corpoHtml);
        _outboxMensagemRepository.Adicionar(OutboxMensagem.Criar(Filas.Email, mensagem, _relogio.AgoraUtc));

        return Task.CompletedTask;
    }
}

file static class ProdutoMapeamentos
{
    public static ProdutoDetalhesDto ParaDetalhesDto(this Produto produto) => new(
        produto.Id,
        produto.Nome,
        produto.Descricao,
        produto.Preco.Valor,
        produto.QuantidadeEmEstoque.Valor,
        produto.CategoriaId,
        produto.Categoria!.Nome,
        produto.CriadoEm);

    public static ProdutoItemListaDto ParaItemListaDto(this Produto produto) => new(
        produto.Id,
        produto.Nome,
        produto.Preco.Valor,
        produto.QuantidadeEmEstoque.Valor,
        produto.CategoriaId,
        produto.Categoria!.Nome,
        produto.CriadoEm);
}

file static class CatalogoProdutosMensagens
{
    public static OutboxMensagem Alterado(Produto produto, DateTime agora) => Montar(produto, excluido: false, agora);

    public static OutboxMensagem Excluido(Produto produto, DateTime agora) => Montar(produto, excluido: true, agora);

    private static OutboxMensagem Montar(Produto produto, bool excluido, DateTime agora)
    {
        var mensagem = new ProdutoCatalogoMensagem(produto.Id, produto.RestauranteId, produto.Nome, produto.Preco.Valor, excluido, agora);
        return OutboxMensagem.Criar(Filas.CatalogoProdutos, mensagem, agora);
    }
}
