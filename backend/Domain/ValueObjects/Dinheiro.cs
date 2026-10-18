using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Domain.ValueObjects;

public sealed record Dinheiro : IComparable<Dinheiro>
{
    public decimal Valor { get; }

    public static Dinheiro Zero { get; } = new(0m);

    private Dinheiro(decimal valor)
    {
        Valor = valor;
    }

    public static Dinheiro Criar(decimal valor)
    {
        if (valor < 0)
            throw new DomainException("O valor em dinheiro não pode ser negativo.");

        return new Dinheiro(valor);
    }

    public int CompareTo(Dinheiro? outro) => outro is null ? 1 : Valor.CompareTo(outro.Valor);

    public override string ToString() => Valor.ToString("F2");
}
