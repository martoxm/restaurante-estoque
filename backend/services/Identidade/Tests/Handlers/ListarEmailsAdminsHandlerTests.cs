using RestauranteEstoque.Identidade.Application.UseCases.Restaurantes;
using RestauranteEstoque.Identidade.Tests.Common;

namespace RestauranteEstoque.Identidade.Tests.Handlers;

public class ListarEmailsAdminsHandlerTests
{
    [Fact]
    public async Task Handle_DeveDevolverSoOsEmailsDoRestaurantePedido()
    {
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var identityService = new FakeIdentityService();
        identityService.EmailsAdmins[restauranteA] = ["admin-a@teste.com"];
        identityService.EmailsAdmins[restauranteB] = ["admin-b@teste.com"];

        var resposta = await new ListarEmailsAdminsHandler(identityService)
            .Handle(new ListarEmailsAdminsQuery(restauranteA), CancellationToken.None);

        Assert.Equal(["admin-a@teste.com"], resposta.Emails);
    }

    [Fact]
    public async Task Handle_RestauranteSemAdmin_DeveDevolverListaVazia()
    {
        var resposta = await new ListarEmailsAdminsHandler(new FakeIdentityService())
            .Handle(new ListarEmailsAdminsQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Empty(resposta.Emails);
    }
}
