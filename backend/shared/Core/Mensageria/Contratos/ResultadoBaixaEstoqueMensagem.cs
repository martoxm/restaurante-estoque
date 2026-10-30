namespace RestauranteEstoque.Core.Mensageria.Contratos;

public sealed record ResultadoBaixaEstoqueMensagem(Guid VendaId, Guid RestauranteId, bool Baixado, string? Motivo);
