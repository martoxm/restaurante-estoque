using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_ComDadosValidos_DeveCriarProdutoComOsValoresInformados()
    {
        var categoryId = Guid.NewGuid();

        var product = Product.Create("Arroz", "Arroz branco tipo 1", 25.90m, categoryId, initialQuantity: 10);

        Assert.Equal("Arroz", product.Name);
        Assert.Equal("Arroz branco tipo 1", product.Description);
        Assert.Equal(25.90m, product.Price);
        Assert.Equal(categoryId, product.CategoryId);
        Assert.Equal(10, product.QuantityInStock);
    }

    [Fact]
    public void Create_DeveRemoverEspacosEmBrancoDoNomeEDaDescricao()
    {
        var product = Product.Create("  Feijão  ", "  Feijão carioca  ", 8m, Guid.NewGuid());

        Assert.Equal("Feijão", product.Name);
        Assert.Equal("Feijão carioca", product.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ComNomeInvalido_DeveLancarDomainException(string? nomeInvalido)
    {
        Assert.Throws<DomainException>(() =>
            Product.Create(nomeInvalido!, null, 10m, Guid.NewGuid()));
    }

    [Fact]
    public void Create_ComPrecoNegativo_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Product.Create("Arroz", null, -1m, Guid.NewGuid()));
    }

    [Fact]
    public void Create_ComCategoryIdVazio_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Product.Create("Arroz", null, 10m, Guid.Empty));
    }

    [Fact]
    public void Create_ComQuantidadeInicialNegativa_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Product.Create("Arroz", null, 10m, Guid.NewGuid(), initialQuantity: -1));
    }

    [Fact]
    public void IncreaseStock_DeveSomarQuantidadeAoEstoqueAtual()
    {
        var product = Product.Create("Arroz", null, 10m, Guid.NewGuid(), initialQuantity: 5);

        product.IncreaseStock(3);

        Assert.Equal(8, product.QuantityInStock);
    }

    [Fact]
    public void DecreaseStock_ComEstoqueSuficiente_DeveSubtrairQuantidade()
    {
        var product = Product.Create("Arroz", null, 10m, Guid.NewGuid(), initialQuantity: 5);

        product.DecreaseStock(3);

        Assert.Equal(2, product.QuantityInStock);
    }

    [Fact]
    public void DecreaseStock_ComEstoqueInsuficiente_DeveLancarDomainExceptionENaoAlterarQuantidade()
    {
        var product = Product.Create("Arroz", null, 10m, Guid.NewGuid(), initialQuantity: 5);

        Assert.Throws<DomainException>(() => product.DecreaseStock(10));
        Assert.Equal(5, product.QuantityInStock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseStock_ComQuantidadeZeroOuNegativa_DeveLancarDomainException(int quantidade)
    {
        var product = Product.Create("Arroz", null, 10m, Guid.NewGuid());

        Assert.Throws<DomainException>(() => product.IncreaseStock(quantidade));
    }
}
