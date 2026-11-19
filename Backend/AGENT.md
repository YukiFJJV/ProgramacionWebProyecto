# Tarea: Actualizar regla de negocio de creación de usuarios (`POST /users`) para teléfonos múltiples y rol automático `PATIENT`

## Contexto
Se requiere actualizar el flujo de creación de usuarios (`POST /users` / `CreateUserCommand` y su handler, DTOs, validadores y mapeos/repositorios en `PsyLink.API`, `PsyLink.Application` y `PsyLink.Infrastructure`) con las siguientes reglas de negocio:

1. **Registro público sin exigir administrador:**
   - Permitir crear el usuario sin requerir un administrador autenticado (manteniendo lo acordado anteriormente sobre no bloquear con `403` por falta de admin).
2. **Teléfonos ingresados por el usuario (uno o varios) sin requerir `phone_id`:**
   - El usuario/cliente envía uno o varios números telefónicos en el payload de creación (por ejemplo, una lista/colección de números, o manteniendo compatibilidad si se envían varios).
   - No se debe exigir un `phone_id` previo de la tabla `phone_numbers`.
   - Cada número debe insertarse/guardarse en la tabla `phone_numbers` (`id: uuid`, `number: char(8)`), respetando la longitud de 8 caracteres (`char(8)`).
   - Asociar el o los registros de `phone_numbers` creados al usuario según corresponda en el modelo/relación de `PsyLink`.
3. **Asignación automática de rol `PATIENT` sin hardcodear:**
   - Si el `role_id` viene `null` (o no se especifica en la solicitud), asignar automáticamente el rol `PATIENT`.
   - **RESTRICCIÓN ESTRICTA:** El GUID del rol `PATIENT` (`15949e08-c62c-4529-a2a1-f41bf96b59dd`) **NO debe ir hardcodeado en el código bajo ninguna circunstancia** (ni como constante, ni como literal en `.cs`, ni como fallback quemado en handlers/servicios/tests).
   - Debe obtenerse de forma configurable (por ejemplo, vía `IConfiguration` / `IOptions` desde `appsettings.json` / `appsettings.Development.json` / variables de entorno, como `Roles:PatientRoleId` o `Identity:PatientRoleId`) y verificarse que exista en la base de datos (`roles`).

## Cambios requeridos

### 1. Configuración (`appsettings.json`, `appsettings.Development.json`, etc.)
- Agregar una clave de configuración para el rol de paciente, por ejemplo:
  ```json
  "RoleSettings": {
    "PatientRoleId": "15949e08-c62c-4529-a2a1-f41bf96b59dd"
  }