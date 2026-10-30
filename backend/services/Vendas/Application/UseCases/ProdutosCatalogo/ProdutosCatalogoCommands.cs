using MediatR;
using RestauranteEstoque.Core.Mensageria.Contratos;

namespace RestauranteEstoque.Vendas.Application.UseCases.ProdutosCatalogo;

public record SincronizarProdutoCatalogoCommand(ProdutoCatalogoMensagem Produto) : IRequest;
