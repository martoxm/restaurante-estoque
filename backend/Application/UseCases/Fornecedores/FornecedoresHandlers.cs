using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Fornecedores;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Application.UseCases.Fornecedores;

public class CriarFornecedorHandler : IRequestHandler<CriarFornecedorCommand, Guid>
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUsuarioAtualService _usuarioAtualService;
    private readonly IRelogio _relogio;

    public CriarFornecedorHandler(
        IFornecedorRepository fornecedorRepository,
        IApplicationDbContext context,
        IUsuarioAtualService usuarioAtualService,
        IRelogio relogio)
    {
        _fornecedorRepository = fornecedorRepository;
        _context = context;
        _usuarioAtualService = usuarioAtualService;
        _relogio = relogio;
    }

    public async Task<Guid> Handle(CriarFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = Fornecedor.Criar(_usuarioAtualService.ObterRestauranteIdObrigatorio(), request.Nome, _relogio.AgoraUtc, request.Telefone, Email.CriarOuNulo(request.Email));

        _fornecedorRepository.Adicionar(fornecedor);
        await _context.SaveChangesAsync(cancellationToken);

        return fornecedor.Id;
    }
}

public class ExcluirFornecedorHandler : IRequestHandler<ExcluirFornecedorCommand>
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IApplicationDbContext _context;

    public ExcluirFornecedorHandler(IFornecedorRepository fornecedorRepository, IApplicationDbContext context)
    {
        _fornecedorRepository = fornecedorRepository;
        _context = context;
    }

    public async Task Handle(ExcluirFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = await _fornecedorRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (fornecedor is null)
            throw new NaoEncontradoException($"Fornecedor com Id '{request.Id}' não encontrado.");

        _fornecedorRepository.Remover(fornecedor);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class AtualizarFornecedorHandler : IRequestHandler<AtualizarFornecedorCommand>
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IApplicationDbContext _context;

    public AtualizarFornecedorHandler(IFornecedorRepository fornecedorRepository, IApplicationDbContext context)
    {
        _fornecedorRepository = fornecedorRepository;
        _context = context;
    }

    public async Task Handle(AtualizarFornecedorCommand request, CancellationToken cancellationToken)
    {
        var fornecedor = await _fornecedorRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (fornecedor is null)
            throw new NaoEncontradoException($"Fornecedor com Id '{request.Id}' não encontrado.");

        fornecedor.Atualizar(request.Nome, request.Telefone, Email.CriarOuNulo(request.Email));

        _fornecedorRepository.Atualizar(fornecedor);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class ObterFornecedorPorIdHandler : IRequestHandler<ObterFornecedorPorIdQuery, FornecedorDetalhesDto?>
{
    private readonly IFornecedorRepository _fornecedorRepository;

    public ObterFornecedorPorIdHandler(IFornecedorRepository fornecedorRepository)
    {
        _fornecedorRepository = fornecedorRepository;
    }

    public async Task<FornecedorDetalhesDto?> Handle(ObterFornecedorPorIdQuery request, CancellationToken cancellationToken)
    {
        var fornecedor = await _fornecedorRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (fornecedor is null)
            return null;

        return new FornecedorDetalhesDto(
            fornecedor.Id,
            fornecedor.Nome,
            fornecedor.Telefone,
            fornecedor.Email?.Valor,
            fornecedor.CriadoEm);
    }
}

public class ListarFornecedoresHandler : IRequestHandler<ListarFornecedoresQuery, ResultadoPaginado<FornecedorItemListaDto>>
{
    private readonly IFornecedorRepository _fornecedorRepository;

    public ListarFornecedoresHandler(IFornecedorRepository fornecedorRepository)
    {
        _fornecedorRepository = fornecedorRepository;
    }

    public async Task<ResultadoPaginado<FornecedorItemListaDto>> Handle(ListarFornecedoresQuery request, CancellationToken cancellationToken)
    {
        var (fornecedores, totalItens) = await _fornecedorRepository.ListarPaginadoAsync(
            request.Pagina,
            request.TamanhoPagina,
            request.Nome,
            request.DirecaoOrdenacao,
            cancellationToken);

        var itens = fornecedores
            .Select(fornecedor => new FornecedorItemListaDto(
                fornecedor.Id,
                fornecedor.Nome,
                fornecedor.Telefone,
                fornecedor.Email?.Valor,
                fornecedor.CriadoEm))
            .ToList();

        return new ResultadoPaginado<FornecedorItemListaDto>(itens, request.Pagina, request.TamanhoPagina, totalItens);
    }
}
