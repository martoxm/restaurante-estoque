using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Identidade.Domain.Entities;

public class RefreshToken : Entity
{
    public static readonly TimeSpan Tolerancia = TimeSpan.FromSeconds(60);

    public Guid UsuarioId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime CriadoEm { get; private set; }
    public DateTime ExpiraEm { get; private set; }
    public DateTime? RevogadoEm { get; private set; }

    private RefreshToken()
    {
    }

    public static RefreshToken Criar(Guid usuarioId, string tokenHash, DateTime criadoEm, DateTime expiraEm)
    {
        if (usuarioId == Guid.Empty)
            throw new DomainException("O usuário do refresh token é obrigatório.");

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException("O hash do refresh token é obrigatório.");

        if (expiraEm <= criadoEm)
            throw new DomainException("O refresh token precisa expirar depois de ser criado.");

        return new RefreshToken
        {
            UsuarioId = usuarioId,
            TokenHash = tokenHash,
            CriadoEm = criadoEm,
            ExpiraEm = expiraEm
        };
    }

    public bool EstaExpirado(DateTime agora) => agora >= ExpiraEm;

    public bool EstaRevogado => RevogadoEm.HasValue;

    public bool PodeSerUsado(DateTime agora) =>
        !EstaExpirado(agora) && (!EstaRevogado || agora - RevogadoEm!.Value <= Tolerancia);

    public bool FoiReusadoForaDaTolerancia(DateTime agora) =>
        EstaRevogado && agora - RevogadoEm!.Value > Tolerancia;

    public void Revogar(DateTime agora)
    {
        RevogadoEm ??= agora;
    }
}
