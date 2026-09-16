using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Domain;

public class CategoryTests
{
    [Fact]
    public void Create_ComNomeValido_DeveCriarCategoria()
    {
        var category = Category.Create("Bebidas", "Bebidas em geral");

        Assert.Equal("Bebidas", category.Name);
        Assert.Equal("Bebidas em geral", category.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ComNomeInvalido_DeveLancarDomainException(string? nomeInvalido)
    {
        Assert.Throws<DomainException>(() => Category.Create(nomeInvalido!));
    }

    [Fact]
    public void Create_ComNomeMaiorQue100Caracteres_DeveLancarDomainException()
    {
        var nomeMuitoLongo = new string('A', 101);

        Assert.Throws<DomainException>(() => Category.Create(nomeMuitoLongo));
    }

    [Fact]
    public void Update_ComDadosValidos_DeveAtualizarNomeEDescricao()
    {
        var category = Category.Create("Bebidas");

        category.Update("Bebidas Geladas", "Cervejas e refrigerantes");

        Assert.Equal("Bebidas Geladas", category.Name);
        Assert.Equal("Cervejas e refrigerantes", category.Description);
    }
}
