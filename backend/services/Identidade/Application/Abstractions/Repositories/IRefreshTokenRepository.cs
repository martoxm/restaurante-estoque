using RestauranteEstoque.Identidade.Domain.Entities;

namespace RestauranteEstoque.Identidade.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> ObterPorHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task<IReadOnlyList<RefreshToken>> ListarAtivosDoUsuarioAsync(Guid usuarioId, DateTime agora, CancellationToken cancellationToken);

    void Adicionar(RefreshToken refreshToken);

    void Atualizar(RefreshToken refreshToken);

    void Remover(RefreshToken refreshToken);

    Task<int> RemoverVencidosAsync(DateTime agora, CancellationToken cancellationToken);
}
