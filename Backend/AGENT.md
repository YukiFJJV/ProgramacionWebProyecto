# Tarea: Habilitar auto-registro público de usuarios en `POST /users` sin requerir administrador

## Contexto y diagnóstico

En `CreateUserHandler` (`PsyLink.Application.Features.Users.Services.Handlers.CreateUser`), el método `EnsureCompanyAdmin()` bloquea la petición con `ForbiddenException("La operación requiere un administrador de empresa autenticado.")` cuando no hay un administrador de empresa autenticado, y además `Handle` obtiene `tenantId` exclusivamente desde `currentUser.TenantId`.

El objetivo es permitir que cualquier usuario pueda crear su cuenta de forma pública (`POST /users` con `[AllowAnonymous]`) sin depender de un usuario autenticado ni de un rol de administrador, conservando la seguridad y la integridad de datos en PostgreSQL (`PsyLinkDB`).

## Cambios requeridos

### 1. `CreateUserHandler.cs`

- **Eliminar o condicionar `EnsureCompanyAdmin()`:**
  - No lanzar `ForbiddenException` cuando el request sea anónimo / no haya un administrador autenticado.
  - Si `currentUser` SÍ corresponde a un administrador de empresa autenticado (`currentUser.RoleName == RoleNames.Admin && currentUser.TenantId is not null`), mantener el comportamiento actual de crear el usuario dentro de su `TenantId` con el rol solicitado (excluyendo `SUPER_ADMIN`).
- **Resolver `TenantId` cuando no haya administrador autenticado:**
  - Obtener un `TenantId` por defecto desde configuración (por ejemplo `IConfiguration` o una opción tipada como `DefaultTenantId` en `appsettings.json` / variables de entorno) o resolver un tenant activo por defecto si no se configura uno explícito, y validar que el `tenant` exista y esté `Active`.
  - No lanzar `ConflictException("El usuario autenticado no tiene una empresa asignada.")` cuando la creación sea pública/anónima.
- **Resolver `RoleId` de forma segura cuando no haya administrador autenticado:**
  - Si no hay un administrador autenticado, **ignorar** cualquier `RoleId` privilegiado enviado en el request (como `ADMIN` o `SUPER_ADMIN`) y asignar un rol seguro por defecto (por ejemplo `USER` o `PSYCHOLOGIST`, consultándolo desde `userRepository`).
  - Validar que el rol exista y esté activo antes de crear el `User`.
- **Mantener validaciones y hashing:**
  - Conservar la validación de correo duplicado (`userRepository.ExistsByEmailAsync` -> `BusinessRuleException`).
  - Mantener `PasswordHash = passwordHasher.Hash(request.User.Password)` para que el usuario creado pueda iniciar sesión luego en `POST /auth/login`.

### 2. `UsersController.cs`

- Confirmar que `POST /users` tenga `[AllowAnonymous]`.
- Mantener protegidos con `[Authorize]` los endpoints de consulta y actualización (`GET /users`, `GET /users/{userId:guid}`, `PUT /users/{userId:guid}`).

### 3. Configuración (`appsettings.json` / `appsettings.Development.json`)

- Agregar una sección o clave clara para el tenant por defecto del registro público (por ejemplo `Registration:DefaultTenantId`), documentando en un comentario o en el README/respuesta qué GUID configurar según la base de datos.

## Criterios de aceptación

1. Enviar `POST /users` sin token de autorización ni rol `ADMIN` crea el usuario en PostgreSQL y responde `201 Created` (nunca `403 Forbidden` por falta de admin).
2. Un usuario anónimo no puede auto-asignarse `ADMIN` ni `SUPER_ADMIN` vía `roleId`.
3. El `password_hash` guardado en la base de datos es un hash generado por `passwordHasher.Hash(...)`, permitiendo hacer `POST /auth/login` y recibir `200 OK`.
4. Los endpoints `GET /users`, `GET /users/{userId:guid}` y `PUT /users/{userId:guid}` siguen requiriendo autenticación/autorización.
