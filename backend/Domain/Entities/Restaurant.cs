using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Domain.Entities;

public class Restaurant : Entity
{
    public string Name { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    private Restaurant()
    {
    }

    public static Restaurant Create(string name, DateTime createdAt)
    {
        ValidateName(name);

        return new Restaurant
        {
            Name = name.Trim(),
            CreatedAt = createdAt
        };
    }

    public void Update(string name)
    {
        ValidateName(name);

        Name = name.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do restaurante é obrigatório.");

        if (name.Trim().Length > 150)
            throw new DomainException("O nome do restaurante deve ter no máximo 150 caracteres.");
    }
}
