namespace RestauranteEstoque.Contracts.Auth;

public record LoginResponse(string Token, string Name, string Email);
