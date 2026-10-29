namespace RestauranteEstoque.Core.Application.Exceptions;

public class SessaoInvalidaException : Exception
{
    public SessaoInvalidaException()
        : base("Sua sessão não tem um restaurante vinculado. Faça login novamente.")
    {
    }

    public SessaoInvalidaException(string mensagem)
        : base(mensagem)
    {
    }
}
