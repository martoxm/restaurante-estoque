using RabbitMQ.Client;

namespace RestauranteEstoque.Core.Mensageria;

public static class Filas
{
    public const string Email = "email-queue";

    public const string BaixarEstoque = "baixar-estoque-queue";

    public const string ResultadoBaixaEstoque = "resultado-baixa-estoque-queue";

    public const string CatalogoProdutos = "catalogo-produtos-queue";

    public static string DeadLetter(string fila) => $"{fila}.dlq";

    public static async Task DeclararAsync(IChannel canal, string fila, CancellationToken cancellationToken = default)
    {
        var filaMorta = DeadLetter(fila);

        await canal.QueueDeclareAsync(filaMorta, durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        await canal.QueueDeclareAsync(fila, durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = string.Empty,
                ["x-dead-letter-routing-key"] = filaMorta
            },
            cancellationToken: cancellationToken);
    }
}
