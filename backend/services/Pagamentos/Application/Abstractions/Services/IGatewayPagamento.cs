namespace RestauranteEstoque.Pagamentos.Application.Abstractions.Services;

public interface IGatewayPagamento
{
    Task<CobrancaAceita> CobrarAsync(PedidoCobranca pedido, CancellationToken cancellationToken);
}

public record PedidoCobranca(Guid PagamentoId, Guid RestauranteId, decimal Valor, string TokenCartao);

public record CobrancaAceita(string IdTransacao);

public class GatewayIndisponivelException(string mensagem) : Exception(mensagem);

public class CobrancaRecusadaException(string motivo) : Exception(motivo);
