# Tarea: Permitir creación pública de usuarios sin requerir administrador

Modifica el flujo de creación de usuarios (`POST /users` en `UsersController` y su handler / servicio / validadores / behaviors en `PsyLink.Application` y `PsyLink.Infrastructure`) para que un usuario pueda registrarse **sin necesidad de estar autenticado ni tener rol de administrador**.

## Contexto del problema actual

- `POST /users` ya tiene `[AllowAnonymous]`, pero al enviarlo sin token de administrador devuelve `403 Forbidden` con el mensaje `"La operación requiere un administrador de empresa autenticado"` (o una regla equivalente en la capa de aplicación/dominio).
- El sistema debe permitir crear el usuario de forma pública sin depender de un usuario administrador autenticado en el contexto actual.

## Cambios requeridos

1. **Localizar y eliminar/adaptar la validación de administrador en creación de usuario:**
   - Busca dónde se lanza el error `"La operación requiere un administrador de empresa autenticado"` (o cualquier verificación de rol `ADMIN` / `SUPER_ADMIN` / usuario autenticado) dentro del flujo de `CreateUserCommand` (handler, validator, authorization/pipeline behavior o domain service).
   - Permite que el comando se ejecute cuando no haya un usuario autenticado.
2. **Resolver `TenantId` y `RoleId` en registro público:**
   - Si no hay un administrador autenticado en el contexto actual, asigna un `TenantId` por defecto válido (definido por configuración o constante/valor por defecto del sistema) en lugar de fallar por falta de admin.
   - Si no hay un administrador autenticado, asigna un rol seguro por defecto (por ejemplo `USER` o `PSYCHOLOGIST`, evitando elevar privilegios a `ADMIN` o `SUPER_ADMIN` desde una petición anónima).
3. **Mantener integridad de datos y seguridad:**
   - Conserva las validaciones de formato/unicidad (por ejemplo, correo duplicado -> `409 Conflict` o datos inválidos -> `400 Bad Request`).
   - Asegúrate de que la contraseña se guarde hasheada (`password_hash`) usando el mismo hasheador que consume el login (`/auth/login`).
   - No rompas los demás endpoints de `UsersController` (`GET /users`, `GET /users/{userId:guid}`, `PUT /users/{userId:guid}`), que deben seguir protegidos con `[Authorize]`.

## Verificación esperada

- Enviar `POST /users` sin token de autorización crea el usuario y devuelve `201 Created` (en vez de `403 Forbidden`).
- Hacer `POST /auth/login` con el correo y contraseña del usuario recién creado devuelve `200 OK` y su token/sesión.
