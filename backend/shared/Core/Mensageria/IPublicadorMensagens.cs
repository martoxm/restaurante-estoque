namespace RestauranteEstoque.Core.Mensageria;

public interface IPublicadorMensagens
{
    Task PublicarAsync(string fila, string tipo, string conteudoJson, Guid mensagemId, CancellationToken cancellationToken);
}
