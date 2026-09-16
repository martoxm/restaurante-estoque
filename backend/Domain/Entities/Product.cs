using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Domain.Entities;

public class Product : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int QuantityInStock { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private Product()
    {
    }

    public static Product Create(string name, string? description, decimal price, Guid categoryId, int initialQuantity = 0)
    {
        ValidateName(name);
        ValidatePrice(price);
        ValidateCategoryId(categoryId);
        ValidateQuantity(initialQuantity);

        return new Product
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Price = price,
            CategoryId = categoryId,
            QuantityInStock = initialQuantity
        };
    }

    public void Update(string name, string? description, decimal price, Guid categoryId)
    {
        ValidateName(name);
        ValidatePrice(price);
        ValidateCategoryId(categoryId);

        Name = name.Trim();
        Description = description?.Trim();
        Price = price;
        CategoryId = categoryId;
    }

    public void IncreaseStock(int quantity)
    {
        ValidateMovementQuantity(quantity);

        QuantityInStock += quantity;
    }

    public void DecreaseStock(int quantity)
    {
        ValidateMovementQuantity(quantity);

        if (quantity > QuantityInStock)
            throw new DomainException($"Estoque insuficiente: disponível {QuantityInStock}, solicitado {quantity}.");

        QuantityInStock -= quantity;
    }

    private static void ValidateMovementQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("A quantidade movimentada deve ser maior que zero.");
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do produto é obrigatório.");

        if (name.Trim().Length > 150)
            throw new DomainException("O nome do produto deve ter no máximo 150 caracteres.");
    }

    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("O preço do produto não pode ser negativo.");
    }

    private static void ValidateCategoryId(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new DomainException("O produto precisa estar vinculado a uma categoria.");
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("A quantidade em estoque não pode ser negativa.");
    }
}
