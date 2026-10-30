namespace RestauranteEstoque.Pagamentos.Contracts.Pagamentos;

public record PagamentoCriadoResponse(Guid Id, string Status);

public record PagamentoDetalhesDto(
    Guid Id,
    Guid VendaId,
    decimal Valor,
    string Status,
    DateTime CriadoEm,
    DateTime? ProcessadoEm,
    string? IdTransacaoGateway,
    string? MotivoRecusa);
