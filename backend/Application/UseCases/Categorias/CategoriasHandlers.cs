using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Categorias;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Categorias;

public class CriarCategoriaHandler : IRequestHandler<CriarCategoriaCommand, Guid>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IRelogio _relogio;

    public CriarCategoriaHandler(
        ICategoriaRepository categoriaRepository,
        IApplicationDbContext context,
        IUsuarioAtualService usuarioAtualService,
        IRelogio relogio)
    {
        _categoriaRepository = categoriaRepository;
        _context = context;
        _usuarioAtualService = usuarioAtualService;
        _relogio = relogio;
    }

    public async Task<Guid> Handle(CriarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = Categoria.Criar(_usuarioAtualService.ObterRestauranteIdObrigatorio(), request.Nome, _relogio.AgoraUtc, request.Descricao);

        _categoriaRepository.Adicionar(categoria);
        await _context.SaveChangesAsync(cancellationToken);

        return categoria.Id;
    }
}

public class ExcluirCategoriaHandler : IRequestHandler<ExcluirCategoriaCommand>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _context;

    public ExcluirCategoriaHandler(ICategoriaRepository categoriaRepository, IApplicationDbContext context)
    {
        _categoriaRepository = categoriaRepository;
        _context = context;
    }

    public async Task Handle(ExcluirCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (categoria is null)
            throw new NaoEncontradoException($"Categoria com Id '{request.Id}' não encontrada.");

        categoria.GarantirQuePodeSerExcluida();

        _categoriaRepository.Remover(categoria);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class AtualizarCategoriaHandler : IRequestHandler<AtualizarCategoriaCommand>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IApplicationDbContext _context;

    public AtualizarCategoriaHandler(ICategoriaRepository categoriaRepository, IApplicationDbContext context)
    {
        _categoriaRepository = categoriaRepository;
        _context = context;
    }

    public async Task Handle(AtualizarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (categoria is null)
            throw new NaoEncontradoException($"Categoria com Id '{request.Id}' não encontrada.");

        categoria.Atualizar(request.Nome, request.Descricao);

        _categoriaRepository.Atualizar(categoria);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class ListarCategoriasHandler : IRequestHandler<ListarCategoriasQuery, ResultadoPaginado<CategoriaItemListaDto>>
{
    private readonly ICategoriaRepository _categoriaRepository;

    public ListarCategoriasHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<ResultadoPaginado<CategoriaItemListaDto>> Handle(ListarCategoriasQuery request, CancellationToken cancellationToken)
    {
        var (categorias, totalItens) = await _categoriaRepository.ListarPaginadoAsync(
            request.Pagina,
            request.TamanhoPagina,
            request.Nome,
            request.DirecaoOrdenacao,
            cancellationToken);

        var itens = categorias
            .Select(categoria => categoria.ParaItemListaDto())
            .ToList();

        return new ResultadoPaginado<CategoriaItemListaDto>(itens, request.Pagina, request.TamanhoPagina, totalItens);
    }
}

public class ObterCategoriaPorIdHandler : IRequestHandler<ObterCategoriaPorIdQuery, CategoriaDetalhesDto?>
{
    private readonly ICategoriaRepository _categoriaRepository;

    public ObterCategoriaPorIdHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<CategoriaDetalhesDto?> Handle(ObterCategoriaPorIdQuery request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (categoria is null)
            return null;

        return categoria.ParaDetalhesDto();
    }
}

file static class CategoriaMapeamentos
{
    public static CategoriaDetalhesDto ParaDetalhesDto(this Categoria categoria) => new(
        categoria.Id,
        categoria.Nome,
        categoria.Descricao,
        categoria.Produtos.Count,
        categoria.CriadoEm);

    public static CategoriaItemListaDto ParaItemListaDto(this Categoria categoria) => new(
        categoria.Id,
        categoria.Nome,
        categoria.Descricao,
        categoria.Produtos.Count,
        categoria.CriadoEm);
}
