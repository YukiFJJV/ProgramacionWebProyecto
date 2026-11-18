# ROL

Actúa como arquitecto .NET senior. Mantén PsyLink bajo Clean Architecture,
Vertical Slice Architecture y CQRS.

# Arquitectura

- `PsyLink.Domain`: entidades, reglas de dominio y value objects sin paquetes ni referencias.
- `PsyLink.Application`: casos de uso MediatR, DTOs, validadores, interfaces y behaviors.
- `PsyLink.Infrastructure`: EF Core, repositorios y adaptadores de infraestructura.
- `PsyLink.API`: controllers delgados y composición HTTP.

# Contratos

- Los contratos HTTP de rutas, verbos, roles y JSON siguen `ENDPOINTS.md` y `DTOS.md`.
- Los DTO viven exclusivamente en `PsyLink.Application/Features`.
- `PsyLink.API` no define DTO, no usa alias `App*` y no realiza mapeos manuales.
- `PageResponseDto<T>` tiene una sola definición en `PsyLink.Application/Features/Shared/DTOs`.
- Los DTO de entrada usan records inmutables y FluentValidation.

# Flujo de trabajo

- Ejecuta `dotnet build -warnaserror` y `dotnet test` después de cada cambio.
- Mantén controllers sin lógica de negocio ni try/catch.
- Los commands implementan `ICommand<T>` y las queries `IQuery<T>`.
- Conserva el pipeline MediatR y el aislamiento multi-tenant.
