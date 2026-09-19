using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Domain.Entities;

public class Supplier : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }

    public Guid? RestaurantId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Supplier()
    {
    }

    public static Supplier Create(string name, DateTime createdAt, string? phone = null, string? email = null, Guid? restaurantId = null)
    {
        ValidateName(name);

        return new Supplier
        {
            Name = name.Trim(),
            Phone = phone?.Trim(),
            Email = email?.Trim(),
            RestaurantId = restaurantId,
            CreatedAt = createdAt
        };
    }

    public void Update(string name, string? phone = null, string? email = null)
    {
        ValidateName(name);

        Name = name.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do fornecedor é obrigatório.");

        if (name.Trim().Length > 150)
            throw new DomainException("O nome do fornecedor deve ter no máximo 150 caracteres.");
    }
}
