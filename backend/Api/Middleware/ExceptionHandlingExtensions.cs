using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Api.Middleware;

public static class ExceptionHandlingExtensions
{
    public static WebApplication UseValidationExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                if (exception is ValidationException validationException)
                {
                    var errors = validationException.Errors
                        .GroupBy(failure => failure.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(failure => failure.ErrorMessage).ToArray());

                    await Results.ValidationProblem(errors).ExecuteAsync(context);
                    return;
                }

                if (exception is CredenciaisInvalidasException credenciaisInvalidasException)
                {
                    await Results.Problem(
                        detail: credenciaisInvalidasException.Message,
                        statusCode: StatusCodes.Status401Unauthorized)
                        .ExecuteAsync(context);
                    return;
                }

                if (exception is SessaoInvalidaException sessaoInvalidaException)
                {
                    await Results.Problem(
                        detail: sessaoInvalidaException.Message,
                        statusCode: StatusCodes.Status401Unauthorized)
                        .ExecuteAsync(context);
                    return;
                }

                if (exception is NaoEncontradoException naoEncontradoException)
                {
                    await Results.Problem(
                        detail: naoEncontradoException.Message,
                        statusCode: StatusCodes.Status404NotFound)
                        .ExecuteAsync(context);
                    return;
                }

                if (exception is DomainException domainException)
                {
                    await Results.Problem(
                        detail: domainException.Message,
                        statusCode: StatusCodes.Status400BadRequest)
                        .ExecuteAsync(context);
                    return;
                }

                await Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
                    .ExecuteAsync(context);
            });
        });

        return app;
    }
}
