namespace RestauranteEstoque.Core.Mensageria.Contratos;

public sealed record EnviarEmailMensagem(Guid RestauranteId, string Assunto, string CorpoHtml);
