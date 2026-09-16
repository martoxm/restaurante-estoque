using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Domain.Entities;

public class Category : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category()
    {
    }

    public static Category Create(string name, string? description = null)
    {
        ValidateName(name);

        return new Category
        {
            Name = name.Trim(),
            Description = description?.Trim()
        };
    }

    public void Update(string name, string? description = null)
    {
        ValidateName(name);

        Name = name.Trim();
        Description = description?.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome da categoria é obrigatório.");

        if (name.Trim().Length > 100)
            throw new DomainException("O nome da categoria deve ter no máximo 100 caracteres.");
    }
}
