using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Core.Api;

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
                        .GroupBy(failure => ConverterNomeDoCampo(failure.PropertyName))
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());

                    await Results.ValidationProblem(
                        errors,
                        title: "Um ou mais campos estão inválidos.")
                        .ExecuteAsync(context);
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

                await Results.Problem(
                    detail: "Ocorreu um erro inesperado. Tente novamente em instantes.",
                    statusCode: StatusCodes.Status500InternalServerError)
                    .ExecuteAsync(context);
            });
        });

        return app;
    }

    private static string ConverterNomeDoCampo(string nomeDaPropriedade) =>
        string.Join('.', nomeDaPropriedade
            .Split('.')
            .Select(parte => JsonNamingPolicy.CamelCase.ConvertName(parte)));
}
