namespace RestauranteEstoque.Application.Abstractions.Services;

public interface IIdentidadeClient
{
    Task<IReadOnlyList<string>> ListarEmailsAdminsAsync(Guid restauranteId, CancellationToken cancellationToken);
}
