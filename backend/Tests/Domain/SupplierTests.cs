using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Domain;

public class SupplierTests
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_ComDadosValidos_DeveCriarFornecedor()
    {
        var supplier = Supplier.Create("Distribuidora ABC", CreatedAt, "(11) 99999-0000", "contato@abc.com");

        Assert.Equal("Distribuidora ABC", supplier.Name);
        Assert.Equal("(11) 99999-0000", supplier.Phone);
        Assert.Equal("contato@abc.com", supplier.Email);
        Assert.Equal(CreatedAt, supplier.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ComNomeInvalido_DeveLancarDomainException(string? nomeInvalido)
    {
        Assert.Throws<DomainException>(() => Supplier.Create(nomeInvalido!, CreatedAt));
    }

    [Fact]
    public void Update_ComDadosValidos_DeveAtualizarTodosOsCampos()
    {
        var supplier = Supplier.Create("Distribuidora ABC", CreatedAt);

        supplier.Update("Distribuidora XYZ", "(11) 98888-1111", "novo@xyz.com");

        Assert.Equal("Distribuidora XYZ", supplier.Name);
        Assert.Equal("(11) 98888-1111", supplier.Phone);
        Assert.Equal("novo@xyz.com", supplier.Email);
    }
}
