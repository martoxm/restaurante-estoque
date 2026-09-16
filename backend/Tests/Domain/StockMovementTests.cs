using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;

namespace RestauranteEstoque.Tests.Domain;

public class StockMovementTests
{
    private static readonly DateTime MovementDate = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_ComDadosValidos_DeveCriarMovimentacao()
    {
        var productId = Guid.NewGuid();

        var movement = StockMovement.Create(productId, StockMovementType.Entrada, 10, MovementDate, "Compra do mês");

        Assert.Equal(productId, movement.ProductId);
        Assert.Equal(StockMovementType.Entrada, movement.Type);
        Assert.Equal(10, movement.Quantity);
        Assert.Equal("Compra do mês", movement.Notes);
    }

    [Fact]
    public void Create_ComProductIdVazio_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            StockMovement.Create(Guid.Empty, StockMovementType.Entrada, 10, MovementDate));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_ComQuantidadeZeroOuNegativa_DeveLancarDomainException(int quantidade)
    {
        Assert.Throws<DomainException>(() =>
            StockMovement.Create(Guid.NewGuid(), StockMovementType.Saida, quantidade, MovementDate));
    }
}
