namespace RestauranteEstoque.Application.Common.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("E-mail ou senha inválidos.")
    {
    }
}
