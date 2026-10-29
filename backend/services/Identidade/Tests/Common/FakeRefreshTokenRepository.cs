using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Identidade.Application.Abstractions.Repositories;
using RestauranteEstoque.Identidade.Domain.Entities;

namespace RestauranteEstoque.Identidade.Tests.Common;

public class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly TestDbContext _context;

    public FakeRefreshTokenRepository(TestDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> ObterPorHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> ListarAtivosDoUsuarioAsync(Guid usuarioId, DateTime agora, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(token => token.UsuarioId == usuarioId && token.RevogadoEm == null && token.ExpiraEm > agora)
            .ToListAsync(cancellationToken);
    }

    public void Adicionar(RefreshToken refreshToken) => _context.RefreshTokens.Add(refreshToken);

    public void Atualizar(RefreshToken refreshToken) => _context.RefreshTokens.Update(refreshToken);

    public void Remover(RefreshToken refreshToken) => _context.RefreshTokens.Remove(refreshToken);

    public async Task<int> RemoverVencidosAsync(DateTime agora, CancellationToken cancellationToken)
    {
        var limiteRevogados = agora.AddDays(-1);
        var vencidos = await _context.RefreshTokens
            .Where(token => token.ExpiraEm <= agora || token.RevogadoEm < limiteRevogados)
            .ToListAsync(cancellationToken);

        _context.RefreshTokens.RemoveRange(vencidos);
        await _context.SaveChangesAsync(cancellationToken);
        return vencidos.Count;
    }
}
