using FluentValidation;
using RestauranteEstoque.Pagamentos.Application.UseCases.Pagamentos;

namespace RestauranteEstoque.Pagamentos.Application.Validators;

public class CriarPagamentoCommandValidator : AbstractValidator<CriarPagamentoCommand>
{
    public CriarPagamentoCommandValidator()
    {
        RuleFor(command => command.ChaveIdempotencia)
            .NotEmpty().WithMessage("Envie o cabeçalho Idempotency-Key.")
            .MaximumLength(255).WithMessage("O Idempotency-Key pode ter no máximo 255 caracteres.");

        RuleFor(command => command.VendaId)
            .NotEqual(Guid.Empty).WithMessage("Informe a venda.");

        RuleFor(command => command.Valor)
            .GreaterThan(0).WithMessage("O valor precisa ser maior que zero.");

        RuleFor(command => command.TokenCartao)
            .NotEmpty().WithMessage("Informe o token do cartão.")
            .MaximumLength(100).WithMessage("O token do cartão pode ter no máximo 100 caracteres.");
    }
}
