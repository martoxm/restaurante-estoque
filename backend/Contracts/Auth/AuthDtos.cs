namespace RestauranteEstoque.Contracts.Auth;

public record LoginResponse(string Token, string Nome, string Email);

public static class RoleNames
{
    public const string Admin = "Admin";

    public const string Funcionario = "Funcionario";
}
