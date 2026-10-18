using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Restaurantes;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Restaurantes;

public class AtualizarRestauranteHandler : IRequestHandler<AtualizarRestauranteCommand>
{
    private readonly IRestauranteRepository _restauranteRepository;
    private readonly IApplicationDbContext _context;

    public AtualizarRestauranteHandler(IRestauranteRepository restauranteRepository, IApplicationDbContext context)
    {
        _restauranteRepository = restauranteRepository;
        _context = context;
    }

    public async Task Handle(AtualizarRestauranteCommand request, CancellationToken cancellationToken)
    {
        var restaurante = await _restauranteRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (restaurante is null)
            throw new NaoEncontradoException($"Restaurante com Id '{request.Id}' não encontrado.");

        restaurante.Atualizar(request.Nome);

        _restauranteRepository.Atualizar(restaurante);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class ObterRestaurantePorIdHandler : IRequestHandler<ObterRestaurantePorIdQuery, RestauranteDetalhesDto?>
{
    private readonly IRestauranteRepository _restauranteRepository;

    public ObterRestaurantePorIdHandler(IRestauranteRepository restauranteRepository)
    {
        _restauranteRepository = restauranteRepository;
    }

    public async Task<RestauranteDetalhesDto?> Handle(ObterRestaurantePorIdQuery request, CancellationToken cancellationToken)
    {
        var restaurante = await _restauranteRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (restaurante is null)
            return null;

        return new RestauranteDetalhesDto(restaurante.Id, restaurante.Nome, restaurante.CriadoEm);
    }
}

public class ListarRestaurantesHandler : IRequestHandler<ListarRestaurantesQuery, ResultadoPaginado<RestauranteItemListaDto>>
{
    private readonly IRestauranteRepository _restauranteRepository;

    public ListarRestaurantesHandler(IRestauranteRepository restauranteRepository)
    {
        _restauranteRepository = restauranteRepository;
    }

    public async Task<ResultadoPaginado<RestauranteItemListaDto>> Handle(ListarRestaurantesQuery request, CancellationToken cancellationToken)
    {
        var (restaurantes, totalItens) = await _restauranteRepository.ListarPaginadoAsync(
            request.Pagina,
            request.TamanhoPagina,
            request.Nome,
            request.DirecaoOrdenacao,
            cancellationToken);

        var itens = restaurantes
            .Select(restaurante => new RestauranteItemListaDto(restaurante.Id, restaurante.Nome, restaurante.CriadoEm))
            .ToList();

        return new ResultadoPaginado<RestauranteItemListaDto>(itens, request.Pagina, request.TamanhoPagina, totalItens);
    }
}
