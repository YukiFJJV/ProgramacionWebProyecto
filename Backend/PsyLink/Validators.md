# Asegurar que todos los Validators se ejecuten en PsyLink.Application

## 1. Causas más comunes cuando los validators ya existen pero no corren

1. **No están registrados en `DependencyInjection.cs`:**
   - Falta `services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);` (o `Assembly.GetExecutingAssembly()`).
   - Falta registrar `ValidationBehavior<,>` como `IPipelineBehavior<,>` en `AddMediatR(...)`.
2. **El `AbstractValidator<T>` apunta al DTO en vez del Command/Query:**
   - `ValidationBehavior<TRequest, TResponse>` inyecta `IEnumerable<IValidator<TRequest>>` usando el **mismo tipo `TRequest`** que envías a `mediator.Send(...)`.
   - Si tu validator hereda de `AbstractValidator<UserCreateRequestDto>` pero envías `CreateUserCommand` a MediatR, `ValidationBehavior` no lo encontrará automáticamente. Debe validar el `Command` o `Query` (o acceder a sus propiedades, p. ej. `RuleFor(x => x.Request.Email)...` o `x.Dto...`).
3. **Orden del pipeline en `AddMediatR`:**
   - Conviene registrar `ValidationBehavior` **antes** de `TransactionBehavior` y de la ejecución del handler para que no se abran transacciones ni corra lógica si la validación falla.
4. **Commands/Queries sin validator o endpoints que no pasan por MediatR:**
   - Revisar cada `Command` y `Query` frente a su carpeta `Validators/` correspondiente.

---

## 2. Revisar `PsyLink.Application/DependencyInjection.cs`

Asegúrate de registrar **todos** los validators del assembly y conectar `ValidationBehavior` en el pipeline de MediatR:

```csharp
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PsyLink.Application.Features.Behaviors;

namespace PsyLink.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 1. Registra todos los AbstractValidator<T> del proyecto Application
        services.AddValidatorsFromAssembly(assembly);

        // 2. Registra Handlers y Behaviors de MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Orden recomendado: Logging -> Authorization -> Validation -> Transaction
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        });

        return services;
    }
}
```
