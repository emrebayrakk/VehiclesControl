using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VehiclesControl.API.Filters;

/// <summary>Runs the registered FluentValidation validator for every action argument and returns 400 on failure.</summary>
public sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                continue;

            var result = await validator.ValidateAsync(new ValidationContext<object>(argument), context.HttpContext.RequestAborted);
            if (!result.IsValid)
            {
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(result.ToDictionary())
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Gönderilen bilgiler geçersiz.",
                });
                return;
            }
        }

        await next();
    }
}
