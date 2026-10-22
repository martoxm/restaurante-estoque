using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests;

public static class DadosTeste
{
    public static readonly Guid RestauranteId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static Categoria CriarCategoria(Guid? restauranteId = null) =>
        Categoria.Criar(restauranteId ?? RestauranteId, "Grãos", new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc));
}
