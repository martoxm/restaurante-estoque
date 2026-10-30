namespace RestauranteEstoque.Core.Application.Abstractions;

public interface IDefinidorRestauranteAtual
{
    void Definir(Guid restauranteId);
}
