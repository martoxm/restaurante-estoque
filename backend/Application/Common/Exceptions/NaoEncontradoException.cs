namespace RestauranteEstoque.Application.Common.Exceptions;

public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string message) : base(message)
    {
    }
}
