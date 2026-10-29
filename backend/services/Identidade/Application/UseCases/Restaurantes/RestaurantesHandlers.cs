using MediatR;
using RestauranteEstoque.Identidade.Application.Abstractions.Repositories;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Identidade.Contracts.Restaurantes;
using RestauranteEstoque.Identidade.Domain.Entities;

namespace RestauranteEstoque.Identidade.Application.UseCases.Restaurantes;

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

        return restaurante.ParaDetalhesDto();
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
            .Select(restaurante => restaurante.ParaItemListaDto())
            .ToList();

        return new ResultadoPaginado<RestauranteItemListaDto>(itens, request.Pagina, request.TamanhoPagina, totalItens);
    }
}

public class ListarEmailsAdminsHandler : IRequestHandler<ListarEmailsAdminsQuery, EmailsAdminsDto>
{
    private readonly IIdentityService _identityService;

    public ListarEmailsAdminsHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<EmailsAdminsDto> Handle(ListarEmailsAdminsQuery request, CancellationToken cancellationToken)
    {
        var emails = await _identityService.ListarEmailsAdminsAsync(request.RestauranteId, cancellationToken);
        return new EmailsAdminsDto(emails);
    }
}

file static class RestauranteMapeamentos
{
    public static RestauranteDetalhesDto ParaDetalhesDto(this Restaurante restaurante) =>
        new(restaurante.Id, restaurante.Nome, restaurante.CriadoEm);

    public static RestauranteItemListaDto ParaItemListaDto(this Restaurante restaurante) =>
        new(restaurante.Id, restaurante.Nome, restaurante.CriadoEm);
}
