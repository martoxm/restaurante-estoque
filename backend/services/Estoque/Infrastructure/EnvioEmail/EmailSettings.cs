using RestauranteEstoque.Core.Seguranca;
namespace RestauranteEstoque.Infrastructure.EnvioEmail;

public class EmailSettings
{
    public const string SectionName = "Email";

    public string Host { get; set; } = string.Empty;
    public int Porta { get; set; } = 587;
    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

    public string Remetente { get; set; } = string.Empty;
    public string NomeRemetente { get; set; } = "Controle de Estoque";
}
