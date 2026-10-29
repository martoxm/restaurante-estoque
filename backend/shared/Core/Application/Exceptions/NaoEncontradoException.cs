namespace RestauranteEstoque.Core.Application.Exceptions;

public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string message) : base(message)
    {
    }
}
