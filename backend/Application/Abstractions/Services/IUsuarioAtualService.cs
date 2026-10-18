namespace RestauranteEstoque.Application.Abstractions.Services;

public interface IUsuarioAtualService
{
    Guid? RestauranteId { get; }

    Guid ObterRestauranteIdObrigatorio();
}
