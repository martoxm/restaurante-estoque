namespace RestauranteEstoque.Identidade.Contracts.Auth;

public record LoginResponse(string Token, string RefreshToken, string Nome, string Email);
