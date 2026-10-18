namespace RestauranteEstoque.Application.Common.Exceptions;

public class SessaoInvalidaException : Exception
{
    public SessaoInvalidaException()
        : base("Sua sessão não tem um restaurante vinculado. Faça login novamente.")
    {
    }
}
