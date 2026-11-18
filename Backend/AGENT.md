# ROL

Actúas como arquitecto .NET senior. Vas a refactorizar la solución PsyLink (.NET 10, EF Core + Npgsql, MediatR, FluentValidation) para que cumpla de forma estricta y optimizada Clean Architecture + Vertical Slice Architecture + CQRS. Debes resolver TODOS los puntos de este documento, sin omitir ninguno.

# AUTORIZACIÓN DE ALCANCE

El `AGENT.md` limita el trabajo a PsyLink.API. PARA ESTA TAREA TE AUTORIZO EXPRESAMENTE a modificar Domain, Application, Infrastructure y API, a mover, renombrar y borrar archivos, y a crear migraciones EF. Sigue respetando el resto de reglas de `AGENT.md`: controllers delgados con `ControllerBase`, `IMediator`/`ISender`, middleware global de excepciones y cumplimiento de `HU.md`, `DTOS.md` y `ENDPOINTS.md`.

# REGLAS DE TRABAJO

1. Lee primero `README.md`, `HU.md`, `DTOS.md`, `ENDPOINTS.md` y `AGENT.md`. Los contratos HTTP (rutas, verbos, roles, códigos de estado, forma de los JSON) NO deben cambiar, salvo lo indicado en la Fase 6.
2. Trabaja por fases en el orden dado. Al terminar cada fase ejecuta `dotnet build` (0 errores y 0 warnings) y, si existen tests, `dotnet test`. No avances si falla.
3. Haz un commit por fase con mensaje descriptivo.
4. No dejes código muerto, `using` sin usar, ni comentarios del tipo "TODO" o "antes".
5. Al final entrega un resumen por fase: archivos creados, movidos y eliminados, decisiones tomadas y pendientes.

# REGLA DE DEPENDENCIAS OBJETIVO

- `Domain` → nada (sin paquetes).
- `Application` → solo `Domain`. No puede referenciar EF Core, ASP.NET ni Infrastructure.
- `Infrastructure` → `Application` + `Domain`.
- `API` → `Application` y, solo en el composition root (`Program.cs` / `DependencyInjection.cs` de la API), `Infrastructure`. Ningún controller, DTO ni middleware puede usar tipos de `Infrastructure` ni de `Domain`.

# FASE 1 — Higiene del repositorio y paquetes

1. Elimina `WeatherForecast.cs` y cualquier referencia a él.
2. Alinea TODOS los paquetes NuGet a la versión de net10.0: `Microsoft.AspNetCore.OpenApi` (hoy 9.0.23), `System.Security.Cryptography.Pkcs` (hoy 9.0.22) y `Microsoft.Extensions.Options.ConfigurationExtensions` (hoy 11.0.0, debe ser 10.x como EF Core). Considera centralizar versiones con `Directory.Packages.props` y `Directory.Build.props` (TargetFramework, Nullable, ImplicitUsings, TreatWarningsAsErrors).
3. Asegura que `.gitignore` excluye `bin/`, `obj/` y `.vs/`, y que no hay restos versionados de `net9.0`.
4. En Infrastructure, mantén el `FrameworkReference Microsoft.AspNetCore.App` solo si es imprescindible. Preferible: dejar `ICurrentUserService` con una implementación que dependa de `IHttpContextAccessor` vía `Microsoft.AspNetCore.Http.Abstractions`. Si no es viable, documenta la excepción.

# FASE 2 — Composition Root y DI automático (elimina el registro manual)

1. En `PsyLink.Application/DependencyInjection.cs` deja un único `AddApplication()` que haga `AddMediatR(RegisterServicesFromAssembly)`, `AddValidatorsFromAssembly` y registre los behaviors del pipeline (ver Fase 4).
2. ELIMINA `AddApplicationForAuthAndTenantEndpoints()` y todos los `AddTransient<IRequestHandler<...>>` / `AddScoped<IValidator<...>>` manuales.
3. Limpia `Program.cs`: borra los más de 70 `using` de `Application` y cada registro manual de handlers. `Program.cs` solo debe contener: `AddControllers` (con JSON enum converter), `AddOpenApi`, `AddProblemDetails`, `AddExceptionHandler`, `AddApplication()`, `AddInfrastructure(config)`, `AddApiServices()` (ver punto 4), autenticación/autorización y el pipeline HTTP.
4. Crea `PsyLink.API/DependencyInjection.cs` con `AddApiServices()` que concentre JWT, políticas de autorización, OpenAPI, CORS y el exception handler (cumple `AGENT.md`).
5. Mueve el registro de `IAppointmentRepository` y cualquier otro repositorio/servicio que esté fuera a `AddInfrastructure()`. `Program.cs` no debe tener `using PsyLink.Infrastructure.*` excepto el namespace raíz de la extensión.
6. Verifica que NO haya doble registro de MediatR ni de behaviors (el pipeline no debe ejecutarse dos veces).

# FASE 3 — Eliminar la fuga de Domain/Infrastructure hacia la API

1. `AuthController` usa `PsyLink.Infrastructure.Security`: reemplázalo por `LoginCommand` vía MediatR. Cualquier tipo de seguridad que necesite la API se expone como abstracción en Application.
2. Los controllers usan `PsyLink.Domain.Identity.RoleNames` solo para `[Authorize(Roles=...)]`. Crea en `PsyLink.API/Authorization/` una clase `Policies` y `Roles` (constantes de la API) o políticas con nombre (`AdminOnly`, `PsychologistOnly`, `AdminOrPsychologist`, etc.) registradas en `AddApiServices()`, y úsalas en los controllers. Verifica que ningún archivo de `PsyLink.API` tenga `using PsyLink.Domain...` ni `using PsyLink.Infrastructure...` (salvo el composition root).
3. `PsyLink.API/DTOs/ScheduleRequestDto.cs` usa `PsyLink.Domain.Clinical`: corrige esa dependencia (ver Fase 6).

# FASE 4 — CQRS completo: pipeline, Unit of Work, autorización y lectura optimizada

**4.1 Excepciones tipadas (separar validación de reglas de negocio)**

- Crea en `Application/Common/Exceptions`: `NotFoundException`, `ForbiddenException`, `ConflictException`, `BusinessRuleException` (puedes derivar de `DomainException` donde aplique). `ValidationException` de FluentValidation queda SOLO para validación de entrada.
- Reemplaza en los ~51 archivos que lanzan `ValidationException` (por reglas de negocio), `KeyNotFoundException` y `UnauthorizedAccessException` por las excepciones tipadas. `ApiExceptionHandler` debe mapearlas a ProblemDetails: 400, 401, 403, 404 y 409, con un formato JSON uniforme y sin filtrar stack traces. Cubre también `DomainException` (→ 422 o 400, define uno y documéntalo).

**4.2 Unit of Work y transacciones**

- Crea `IUnitOfWork` (`SaveChangesAsync`) en Application, implementado por `ApplicationDbContext`.
- QUITA todo `SaveChangesAsync` de los 13 repositorios. Los repositorios solo añaden, actualizan, eliminan y consultan. El guardado lo hace el handler (o, mejor, un `TransactionBehavior`).
- Crea `TransactionBehavior<TRequest,TResponse>` que se aplique solo a Commands (marcador `ICommand<T> : IRequest<T>`; `IQuery<T> : IRequest<T>`), abra transacción, llame `SaveChangesAsync` y haga commit/rollback. Los comandos multi-repositorio (`RegisterPatient`, `RegisterPsychologist`, `CreateUser`, etc.) deben ser atómicos.
- Los métodos `AddIfNoOverlapAsync` / `UpdateIfNoOverlapAsync` deben mantener su garantía de atomicidad (usa transacción serializable, restricción de exclusión de PostgreSQL con `tstzrange`/`EXCLUDE USING gist`, o bloqueo optimista). Prefiere la restricción en BD + captura de la violación → `ConflictException`.

**4.3 Marcadores y convención CQRS**

- Todos los Commands implementan `ICommand<TResponse>`, todas las Queries `IQuery<TResponse>`. Convierte los 27 commands y 22 queries. Los commands sin retorno usan `Unit`.
- Los Queries no deben modificar estado ni llamar a `IUnitOfWork`.

**4.4 Lado de lectura sin cargar agregados**

- Para listados y detalles (queries) crea interfaces de lectura en `Application/Features/<Feature>/` (por ejemplo `IPatientReadRepository`, `IAppointmentReadRepository`, `IPsychologistReadRepository`) que DEVUELVAN DTOs de lectura proyectados en SQL con `Select(...)` + `AsNoTracking()`, paginación y orden en base de datos. Prohibido `Include` + mapeo en memoria en queries.
- Mantén los repositorios de escritura (`IAppointmentRepository`, etc.) solo con operaciones sobre agregados: `GetById`, `Add`, `Update`, `Remove` y chequeos de invariantes. Separa lo que hoy mezcla `IAppointmentRepository` (lectura paginada vs escritura).
- Implementa las lecturas en `Infrastructure/Persistence/Queries/` (o `Repositories/<Contexto>/Read`).

**4.5 Autorización centralizada**

- Crea un `AuthorizationBehavior` y un atributo `[Authorize(Roles = ...)]` propio sobre el request (`AuthorizeAttribute` de Application, sin depender de ASP.NET), que use `ICurrentUserService`. Mueve ahí las comprobaciones de rol y de pertenencia al tenant que hoy están repetidas en 47 handlers. Las reglas contextuales (ej.: "psicólogo solo ve sus pacientes") pueden quedar en el handler pero vía un servicio de política reutilizable (`PatientAccess`, `PatientRecordAccess` convertidos a servicios inyectables, no estáticos).
- Orden del pipeline: Logging → Authorization → Validation → Transaction (solo commands) → Handler.
- Añade un `LoggingBehavior` y un `PerformanceBehavior` ligeros (ILogger) si no existen.

**4.6 `ICurrentUserService`**

- Expón `UserId`, `TenantId`, `RoleName`, `IsAuthenticated` ya parseados desde los claims. Deben ser fáciles de mockear en tests y lanzar `ForbiddenException` clara si el usuario no tiene tenant cuando se requiere.

# FASE 5 — Multi-tenancy seguro

1. Añade a las entidades con `TenantId` una interfaz marcadora `ITenantScoped` en Domain.
2. En `ApplicationDbContext` configura `HasQueryFilter` global por `TenantId` usando `ICurrentUserService` (con `IgnoreQueryFilters()` explícito solo donde sea necesario, por ejemplo Login y registro de Tenant), y asigna `TenantId` automáticamente al insertar en `SaveChanges` (override o interceptor).
3. Simplifica los métodos de repositorio quitando los parámetros `tenantId` redundantes (`GetByIdForTenantAsync` → `GetByIdAsync`), manteniendo el aislamiento garantizado por el filtro global. Documenta en el README cómo funciona.
4. Añade tests (o al menos un test de integración con SQLite/Testcontainers) que demuestren que un usuario del tenant A no puede leer ni modificar datos del tenant B.

# FASE 6 — DTOs sin duplicación y contrato HTTP único

1. Hoy 41 DTOs están duplicados entre `PsyLink.API/DTOs` y `Application/Features/*/DTOs`, y los controllers los copian a mano con alias `App*`. ELIGE y APLICA UNA sola estrategia:
   - **Estrategia recomendada:** elimina `PsyLink.API/DTOs` y usa directamente los DTOs de Application en los controllers (como pide `AGENT.md`). Los atributos de OpenAPI se mantienen en los controllers. Si algún DTO de la API necesita `[FromRoute]`/`[FromQuery]`, crea el binding en el Command/Query o usa `[AsParameters]`.
2. Elimina TODOS los alias `AppXxxDto` y los mapeos manuales campo a campo de los controllers. El controller solo construye el Command/Query, hace `Send` y devuelve el resultado.
3. Los DTOs de entrada deben ser `record` inmutables con DataAnnotations mínimas o validadores FluentValidation. Verifica contra `DTOS.md` que nombres, tipos y reglas coinciden exactamente.
4. Elimina la dependencia de `Domain.Clinical` en el DTO de horarios (`ScheduleRequestDto`): usa un tipo de Application (ej. `WeekDay` expuesto en Application o un enum/cadena validada) o un mapeo explícito dentro de Application.
5. Unifica `PageResponseDto<T>`: debe existir UNA sola definición (Application/Features/Shared/DTOs). Elimina `TenantPageDto` y `UserPageDto` si son redundantes y úsala en todos los listados.

# FASE 7 — Rutas y controllers

1. Elimina la doble ruta `[Route("api/[controller]")]` + `[Route("patients")]` de todos los controllers. Define UNA convención según `ENDPOINTS.md` (si dice `/patients`, usa `[Route("patients")]` y asegúrate de que el controller usa `[Route("[controller]")]` en minúscula o un `ControllerNameConvention`). Cada endpoint debe existir exactamente una vez.
2. Reemplaza las URLs hardcodeadas en `Created(...)` por `CreatedAtAction(nameof(GetById), new { id }, result)`.
3. Controllers sin lógica: sin `try/catch`, sin `if` de negocio, sin mapeos. Cada acción: recibir → `mediator.Send` → retornar. Usa `ISender`.
4. Usa `[ProducesResponseType]` coherentes con las excepciones mapeadas (400/401/403/404/409/422) y con `ENDPOINTS.md`.
5. Mantén `PsyLink.API.http` actualizado con todos los endpoints finales.

# FASE 8 — Vertical Slicing real

1. Reestructura cada feature a slices por caso de uso. Estructura objetivo:

``` Application/Features/<Feature>/<UseCase>/
    <UseCase>Command.cs | <UseCase>Query.cs
    <UseCase>Handler.cs
    <UseCase>Validator.cs
    <UseCase>Response.cs (o DTOs específicos del caso)
Application/Features/<Feature>/Shared/   (mappers, DTOs y servicios usados por 2+ casos del mismo feature)
Application/Common/                      (behaviors, excepciones, interfaces transversales, marcadores)
Application/Features/Shared/             (PageResponseDto, extensiones realmente globales)
```

   Elimina el nivel intermedio `Services/Commands|Handlers|Queries|Validators`. Mueve namespaces para que coincidan con la carpeta.

1. **Cero acoplamiento entre slices:** ningún handler puede referenciar a otro handler ni a clases internas de otro caso de uso. Corrige el caso de `CreateUserHandler.ToResponse(...)` usado en `RegisterPatientHandler`, `GetPatientsHandler`, `RegisterPsychologistHandler` y `GetPsychologistProfileHandler`: extrae `UserDtoMapper` a `Features/Users/Shared/` (o proyección en la query de lectura) y úsalo desde ahí. Elimina `ToResponse` `internal static` de los handlers (p. ej. `CreateAppointmentHandler`) y usa los mappers del feature (`AppointmentDtoMapper`, etc.) o proyecciones.
2. **Límites de feature:** `Patients` contiene diagnósticos de paciente y estados de diagnóstico que se solapan con `Diagnoses`. Redefine límites: `Patients` (perfil y registro), `Diagnoses` (catálogo + diagnóstico de paciente + estados + diagnósticos del expediente) o crea `PatientDiagnoses` como feature propio. Justifica la decisión en el README y mueve los casos de uso (`UpsertPatientDiagnosis`, `GetPatientDiagnoses`, `GetDiagnosisStatuses`).
3. **Completar slices faltantes:** `Prescriptions` no tiene Queries (añade `GetPrescriptions` / `GetPrescriptionById` si `HU.md` o `ENDPOINTS.md` lo requieren) y asegura que TODO command/query tiene su validator (aunque sea vacío con la regla `NotNull`/`NotEmpty` de IDs). Verifica por script que cada `*Command`/`*Query` tiene su `*Validator` y su `*Handler`.
4. Un feature puede depender de `Domain`, de `Application/Common` y de `Features/Shared`. NO de otro feature. Añade un test de arquitectura (NetArchTest o ArchUnitNET) que lo verifique.

# FASE 9 — Dominio rico (evitar modelo anémico)

1. Encapsula todas las entidades: setters `private`/`init`, constructores o factories estáticas (`Appointment.Schedule(...)`, `Patient.Register(...)`, `Psychologist.Create(...)`, `WorkSchedule.Create(...)`, `Prescription.Issue(...)`, etc.) que validen invariantes. Configura EF para que siga mapeando (constructor privado sin parámetros o `Property` con backing field).
2. Mueve al dominio las reglas que hoy están en los handlers, como en `CreateAppointmentHandler`: `EndsAt > StartsAt`, cita dentro del horario laboral, paciente/psicólogo activos, solapamiento (`OverlapsWith` + chequeo de BD), regla de riesgo nivel 3 para psicólogos (usa `RiskPolicy`), y límite de reprogramaciones (`CanBeRescheduledByPatient` / `MaxPatientReschedules`). El handler solo orquesta: carga agregados, llama al método del dominio, usa UoW.
3. Los handlers deben usar `Appointment.Reschedule/Confirm/Cancel/Complete` en vez de modificar propiedades. Todas las violaciones lanzan `DomainException` (mapeada por el middleware).
4. Introduce value objects donde aporte: `PhoneNumber` (validación E.164), `TimeRange`/`WorkingHours` (inicio/fin con validación y `Overlaps`), `RiskLevel` (0–3 con `Urgent`), `Email`. Configúralos con `OwnsOne` o `HasConversion`.
5. Mantén Domain sin paquetes ni dependencias externas. Si necesitas eventos, usa eventos de dominio simples (`IDomainEvent`) y despáchalos desde el `DbContext` al guardar (solo si aportan; no sobre-ingenierices).

# FASE 10 — Infraestructura y persistencia

1. Reorganiza `Infrastructure` por contexto, igual que Domain (Billing, Clinical, Identity, Pharmacy), con carpetas `Persistence/Configurations`, `Persistence/Repositories`, `Persistence/Queries`, `Security`, `Services`.
2. Revisa índices: `TenantId`, `(PsychologistId, DateAt, StartsAt)`, `PatientId`, `UserId`, emails únicos por tenant, etc. Crea la migración EF correspondiente a los cambios de modelo (query filters no la requieren; value objects y restricciones sí).
3. Evita N+1 y sobrecarga de `Include`: en escritura incluye solo lo necesario; en lectura proyecta con `Select`.
4. Pasa `CancellationToken` en todas las llamadas asíncronas.

# FASE 11 — Calidad, pruebas y documentación

1. Tests unitarios de dominio (reglas de `Appointment`, `WorkSchedule`, `RiskPolicy`, value objects), tests de handlers críticos (CreateAppointment, RescheduleAppointment, RegisterPatient, Login) con repositorios mockeados, y tests de arquitectura que garanticen:
   - Domain no referencia nada; Application no referencia Infrastructure ni EF ni ASP.NET.
   - API no usa Domain ni Infrastructure salvo en el composition root.
   - Los features no se referencian entre sí.
   - Toda clase `*Handler` implementa `IRequestHandler`, es `sealed` y vive en su slice.
2. Actualiza `README.md` con: diagrama de capas, convención de slices, pipeline de MediatR, estrategia multi-tenant, cómo agregar un nuevo caso de uso paso a paso, y cómo correr tests y migraciones.
3. Actualiza `AGENT.md` para reflejar la nueva estructura y el nuevo flujo de trabajo (ya no solo API), de modo que futuras tareas con Copilot respeten la arquitectura.
4. Verificación final: `dotnet build -warnaserror`, `dotnet test`, comprobar que Swagger/OpenAPI lista cada endpoint una sola vez y que las rutas coinciden con `ENDPOINTS.md`.

# CRITERIOS DE ACEPTACIÓN (checklist final; responde con ✅/❌ y evidencia para cada punto)

- [ ] Domain sin paquetes ni referencias; Application sin EF/ASP.NET/Infrastructure.
- [ ] API sin `using` de Domain ni Infrastructure fuera del composition root.
- [ ] Un único `AddApplication()`; cero registros manuales de handlers/validators; `Program.cs` mínimo; `API/DependencyInjection.cs` creado.
- [ ] Repositorios registrados solo en `AddInfrastructure()`.
- [ ] Paquetes alineados a net10.0; `WeatherForecast.cs` y restos de `bin/obj` fuera del repo.
- [ ] Excepciones tipadas; `ValidationException` solo para validación; middleware mapea todo a ProblemDetails consistente.
- [ ] `IUnitOfWork` + `TransactionBehavior`; sin `SaveChanges` en repositorios; comandos multi-repositorio atómicos.
- [ ] `ICommand`/`IQuery` aplicados a todos los mensajes; pipeline Logging → Authorization → Validation → Transaction.
- [ ] Queries con proyección directa en SQL y `AsNoTracking`; repositorios de lectura separados de los de escritura.
- [ ] Autorización centralizada en behavior; handlers sin `if` de rol repetidos.
- [ ] Query filter global por `TenantId` + auto-asignación; tests de aislamiento entre tenants.
- [ ] Cero DTOs duplicados API/Application; cero alias `App*`; un solo `PageResponseDto<T>`.
- [ ] Una sola ruta por endpoint, acorde a `ENDPOINTS.md`; `CreatedAtAction`; controllers sin lógica.
- [ ] Estructura por slice de caso de uso, sin carpeta `Services/` intermedia; cero acoplamiento entre handlers/features; `UserDtoMapper` extraído; límites de `Patients`/`Diagnoses` resueltos; todos los commands/queries con handler y validator.
- [ ] Dominio encapsulado con factories y value objects; reglas de negocio movidas al dominio; `DomainException` en uso real.
- [ ] Migración EF e índices revisados.
- [ ] Tests de dominio, handlers y arquitectura en verde; `dotnet build -warnaserror` limpio.
- [ ] README y AGENT.md actualizados.

Empieza por la Fase 1 y detente a reportar al terminar cada fase antes de continuar con la siguiente.