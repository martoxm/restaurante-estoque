using FluentValidation;
using RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;
using RestauranteEstoque.Domain.Enums;

namespace RestauranteEstoque.Application.Validators;

public class CriarMovimentacaoEstoqueCommandValidator : AbstractValidator<CriarMovimentacaoEstoqueCommand>
{
    public CriarMovimentacaoEstoqueCommandValidator()
    {
        RuleFor(command => command.ProdutoId)
            .NotEqual(Guid.Empty).WithMessage("A movimentação precisa estar vinculada a um produto.");

        RuleFor(command => command.Tipo)
            .IsInEnum().WithMessage("O tipo de movimentação deve ser Entrada ou Saida.");

        RuleFor(command => command.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade movimentada deve ser maior que zero.");

        RuleFor(command => command.Observacoes)
            .MaximumLength(500).WithMessage("As observações devem ter no máximo 500 caracteres.");
    }
}
