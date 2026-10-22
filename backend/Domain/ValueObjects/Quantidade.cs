using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Domain.ValueObjects;

public sealed record Quantidade : IComparable<Quantidade>
{
    public int Valor { get; }

    public static Quantidade Zero { get; } = new(0);

    private Quantidade(int valor)
    {
        Valor = valor;
    }

    public static Quantidade Criar(int valor)
    {
        if (valor < 0)
            throw new DomainException("A quantidade não pode ser negativa.");

        return new Quantidade(valor);
    }

    public bool EhZero => Valor == 0;

    public Quantidade Somar(Quantidade outra) => new(Valor + outra.Valor);

    public Quantidade Subtrair(Quantidade outra) => Criar(Valor - outra.Valor);


    public int CompareTo(Quantidade? outra) => outra is null ? 1 : Valor.CompareTo(outra.Valor);

    public static bool operator >(Quantidade esquerda, Quantidade direita) => esquerda.CompareTo(direita) > 0;
    public static bool operator <(Quantidade esquerda, Quantidade direita) => esquerda.CompareTo(direita) < 0;
    public static bool operator >=(Quantidade esquerda, Quantidade direita) => esquerda.CompareTo(direita) >= 0;
    public static bool operator <=(Quantidade esquerda, Quantidade direita) => esquerda.CompareTo(direita) <= 0;

    public override string ToString() => Valor.ToString();
}
