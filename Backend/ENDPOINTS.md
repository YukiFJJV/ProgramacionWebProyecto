# Especificiones de los Endpoints para API

## 1. Especificación de endpoints por rol

En los endpoints `GET`, los DTOs de consulta se envían como query string, no como body. En `POST`/`PUT`/`PATCH`, los DTOs de entrada se envían como JSON. Las rutas y roles proceden del inventario del equipo.

### 1.1 Público (sin sesión)

| Método | Ruta | HU | Entrada | Salida | Propósito | Rol |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `POST` | `/auth/login` | HU-22 | `LoginRequestDto` | `LoginResponseDto` | Autenticar una cuenta y emitir token de acceso. | Público |
| `POST` | `/tenants` | HU-21 | `TenantCreateRequestDto` | `TenantResponseDto` | Registrar una empresa/tenant. Debe aplicar validación de nombre único y crear relación de teléfono si se envía. | Público |

### 1.2 `SUPER_ADMIN`

| Método | Ruta | HU | Entrada | Salida | Propósito | Rol |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/tenants` | HU-01 | `TenantQueryDto` | `PageResponseDto<TenantResponseDto>` | Listar empresas con búsqueda, filtros y paginación. | `SUPER_ADMIN` |
| `GET` | `/tenants/{tenantId}` | HU-01 | — | `TenantResponseDto` | Obtener una empresa por identificador. | `SUPER_ADMIN` |
| `PUT` | `/tenants/{tenantId}` | HU-02 | `TenantUpdateRequestDto` | `TenantResponseDto` | Actualizar datos y estado de una empresa. | `SUPER_ADMIN` |

### 1.3 `ADMIN`

| Método | Ruta | HU | Entrada | Salida | Propósito | Rol |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/roles` | HU-03 | — | `RoleResponseDto[]` | Listar roles que el administrador puede asignar. Excluir `SUPER_ADMIN`. | `ADMIN` |
| `POST` | `/users` | HU-03 | `UserCreateRequestDto` | `UserResponseDto` | Crear usuario dentro del tenant autenticado. | `ADMIN` |
| `GET` | `/users` | HU-03 | `UserQueryDto` | `PageResponseDto<UserResponseDto>` | Listar usuarios del tenant. | `ADMIN` |
| `GET` | `/users/{userId}` | HU-03 | — | `UserResponseDto` | Consultar un usuario del mismo tenant. | `ADMIN` |
| `PUT` | `/users/{userId}` | HU-03 | `UserUpdateRequestDto` | `UserResponseDto` | Actualizar datos/rol del usuario. | `ADMIN` |
| `POST` | `/patients` | HU-04 | `PatientCreateRequestDto` | `PatientResponseDto` | Crear cuenta y perfil de paciente en una operación transaccional. | `ADMIN` |
| `GET` | `/patients` | HU-13 | `PatientQueryDto` | `PageResponseDto<PatientResponseDto>` | Listar pacientes del tenant. | `ADMIN` |
| `GET` | `/patients/{patientId}` | HU-14 | — | `PatientResponseDto` | Consultar el perfil de un paciente. | `ADMIN` |
| `POST` | `/psychologists` | HU-05 | `PsychologistCreateRequestDto` | `PsychologistDetailResponseDto` | Crear usuario y perfil de psicólogo; validar licencia y especialidad. | `ADMIN` |
| `GET` | `/psychologists` | HU-06 | `PsychologistQueryDto` | `PageResponseDto<PsychologistSummaryResponseDto>` | Listar psicólogos del tenant. | `ADMIN` |
| `GET` | `/psychologists/{psychologistId}` | HU-15 | — | `PsychologistDetailResponseDto` | Consultar el perfil detallado del psicólogo. | `ADMIN` |
| `GET` | `/psychologists/{psychologistId}/availability` | HU-15 | `AvailabilityQueryDto` | `AvailabilityResponseDto` | Consultar slots libres calculados desde horario y citas existentes. | `ADMIN` |
| `POST` | `/specialties` | HU-07 | `SpecialtyRequestDto` | `SpecialtyResponseDto` | Crear una especialidad; nombre único. | `ADMIN` |
| `GET` | `/specialties` | HU-07 | — | `SpecialtyResponseDto[]` | Listar especialidades. | `ADMIN` |
| `PUT` | `/specialties/{specialtyId}` | HU-07 | `SpecialtyRequestDto` | `SpecialtyResponseDto` | Actualizar nombre de especialidad. | `ADMIN` |
| `POST` | `/psychologists/{psychologistId}/schedules` | HU-08 | `ScheduleRequestDto` | `ScheduleResponseDto` | Agregar un bloque semanal al horario del psicólogo. | `ADMIN` |
| `GET` | `/psychologists/{psychologistId}/schedules` | HU-08 | — | `ScheduleResponseDto[]` | Listar horario semanal del psicólogo. | `ADMIN` |
| `PUT` | `/schedules/{scheduleId}` | HU-08 | `ScheduleRequestDto` | `ScheduleResponseDto` | Actualizar un bloque de horario; validar pertenencia al tenant. | `ADMIN` |
| `GET` | `/appointments` | HU-13 | `AppointmentQueryDto` | `PageResponseDto<AppointmentResponseDto>` | Consultar citas del tenant con filtros. | `ADMIN` |
| `GET` | `/appointments/{appointmentId}` | HU-13 | — | `AppointmentDetailResponseDto` | Consultar detalle de una cita. | `ADMIN` |
| `PUT` | `/appointments/{appointmentId}` | HU-09 | `AppointmentUpdateRequestDto` | `AppointmentResponseDto` | Reprogramar una cita; validar disponibilidad y conflictos. | `ADMIN` |
| `GET` | `/diagnoses` | HU-10 | `DiagnosisQueryDto` | `PageResponseDto<DiagnosisResponseDto>` | Consultar catálogo de diagnósticos. | `ADMIN` |
| `GET` | `/diagnoses/{diagnosisId}` | HU-10 | — | `DiagnosisDetailResponseDto` | Consultar detalle del diagnóstico. | `ADMIN` |
| `POST` | `/diagnoses` | HU-10 | `DiagnosisRequestDto` | `DiagnosisDetailResponseDto` | Crear diagnóstico en el catálogo. | `ADMIN` |
| `PUT` | `/diagnoses/{diagnosisId}` | HU-10 | `DiagnosisRequestDto` | `DiagnosisDetailResponseDto` | Actualizar diagnóstico del catálogo. | `ADMIN` |
| `DELETE` | `/diagnoses/{diagnosisId}` | HU-10 | — | Sin cuerpo (`204`) | Eliminar diagnóstico si no tiene referencias; de lo contrario, rechazar o desactivar según política. | `ADMIN` |
| `POST` | `/medications` | HU-11 | `MedicationCreateRequestDto` | `MedicationResponseDto` | Crear medicamento y sus vínculos con principios activos. | `ADMIN` |
| `GET` | `/medications` | HU-10 | `MedicationQueryDto` | `PageResponseDto<MedicationResponseDto>` | Consultar catálogo de medicamentos. | `ADMIN` |
| `GET` | `/laboratories` | HU-11 | — | `LaboratoryResponseDto[]` | Listar laboratorios. | `ADMIN` |
| `GET` | `/active-ingredients` | HU-11 | — | `IngredientResponseDto[]` | Listar principios activos. | `ADMIN` |

### 1.4 `PSYCHOLOGIST`

| Método | Ruta | HU | Entrada | Salida | Propósito | Rol |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/patients` | HU-13 | `PatientQueryDto` | `PageResponseDto<PatientResponseDto>` | Listar solo pacientes que el psicólogo está autorizado a atender. | `PSYCHOLOGIST` |
| `GET` | `/patients/{patientId}` | HU-14 | — | `PatientResponseDto` | Consultar perfil de paciente autorizado. | `PSYCHOLOGIST` |
| `PUT` | `/patients/{patientId}` | HU-14 | `PatientUpdateRequestDto` | `PatientResponseDto` | Actualizar datos permitidos del paciente. | `PSYCHOLOGIST` |
| `GET` | `/patients/{patientId}/diagnoses` | HU-14 | — | `PatientDiagnosisResponseDto[]` | Consultar diagnósticos asociados al paciente. | `PSYCHOLOGIST` |
| `PUT` | `/patients/{patientId}/diagnoses/{diagnosisId}` | HU-14 | `PatientDiagnosisUpsertRequestDto` | `PatientDiagnosisResponseDto` | Crear o actualizar el estado del diagnóstico del paciente (upsert). | `PSYCHOLOGIST` |
| `GET` | `/diagnosis-statuses` | HU-14 | — | `DiagnosisStatusResponseDto[]` | Listar estados disponibles para diagnósticos del paciente. | `PSYCHOLOGIST` |
| `GET` | `/specialties` | HU-07 | — | `SpecialtyResponseDto[]` | Listar especialidades. | `PSYCHOLOGIST` |
| `GET` | `/psychologists/{psychologistId}/availability` | HU-20 | `AvailabilityQueryDto` | `AvailabilityResponseDto` | Consultar disponibilidad para reservar una cita. | `PSYCHOLOGIST` |
| `POST` | `/appointments` | HU-20 | `AppointmentCreateRequestDto` | `AppointmentResponseDto` | Crear cita; el paciente objetivo debe estar autorizado por la regla de negocio. | `PSYCHOLOGIST` |
| `GET` | `/appointments` | HU-13 | `AppointmentQueryDto` | `PageResponseDto<AppointmentResponseDto>` | Listar citas accesibles al psicólogo autenticado. | `PSYCHOLOGIST` |
| `GET` | `/appointments/{appointmentId}` | HU-13 | — | `AppointmentDetailResponseDto` | Consultar detalle de cita autorizada. | `PSYCHOLOGIST` |
| `GET` | `/diagnoses` | HU-12 | `DiagnosisQueryDto` | `PageResponseDto<DiagnosisResponseDto>` | Consultar catálogo de diagnósticos. | `PSYCHOLOGIST` |
| `GET` | `/diagnoses/{diagnosisId}` | HU-12 | — | `DiagnosisDetailResponseDto` | Consultar diagnóstico del catálogo. | `PSYCHOLOGIST` |
| `GET` | `/medications` | HU-19 | `MedicationQueryDto` | `PageResponseDto<MedicationResponseDto>` | Consultar catálogo de medicamentos. | `PSYCHOLOGIST` |
| `POST` | `/records` | HU-17 | `RecordCreateRequestDto` | `RecordDetailResponseDto` | Crear expediente del paciente; validar relación paciente/cita/tenant. | `PSYCHOLOGIST` |
| `GET` | `/patients/{patientId}/record` | HU-18 | — | `RecordDetailResponseDto` | Obtener expediente del paciente autorizado. | `PSYCHOLOGIST` |
| `GET` | `/records/{recordId}` | HU-18 | — | `RecordDetailResponseDto` | Obtener expediente por ID, comprobando tenant y autorización. | `PSYCHOLOGIST` |
| `PATCH` | `/records/{recordId}` | HU-19 | `RecordUpdateRequestDto` | `RecordDetailResponseDto` | Actualizar parcialmente campos admitidos del expediente. | `PSYCHOLOGIST` |
| `PUT` | `/records/{recordId}/diagnoses/{diagnosisId}` | HU-19 | `RecordDiagnosisRequestDto` | `RecordDiagnosisResponseDto` | Asociar diagnóstico al expediente o actualizar sus notas. | `PSYCHOLOGIST` |
| `DELETE` | `/records/{recordId}/diagnoses/{diagnosisId}` | HU-19 | — | Sin cuerpo (`204`) | Eliminar la asociación diagnóstico-expediente. | `PSYCHOLOGIST` |
| `POST` | `/records/{recordId}/prescriptions` | HU-19 | `PrescriptionRequestDto` | `PrescriptionResponseDto` | Crear receta dentro del expediente. | `PSYCHOLOGIST` |
| `PUT` | `/records/{recordId}/prescriptions/{prescriptionId}` | HU-19 | `PrescriptionUpdateRequestDto` | `PrescriptionResponseDto` | Actualizar receta perteneciente al expediente. | `PSYCHOLOGIST` |
| `DELETE` | `/records/{recordId}/prescriptions/{prescriptionId}` | HU-19 | — | Sin cuerpo (`204`) | Eliminar receta perteneciente al expediente. | `PSYCHOLOGIST` |

### 1.5 `PATIENT`

| Método | Ruta | HU | Entrada | Salida | Propósito | Rol |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/psychologists` | HU-15 | `PsychologistQueryDto` | `PageResponseDto<PsychologistSummaryResponseDto>` | Buscar psicólogos visibles para el paciente. | `PATIENT` |
| `GET` | `/psychologists/{psychologistId}` | HU-15 | — | `PsychologistDetailResponseDto` | Consultar perfil público del psicólogo dentro del tenant permitido. | `PATIENT` |
| `GET` | `/psychologists/{psychologistId}/schedules` | HU-15 | — | `ScheduleResponseDto[]` | Consultar horario semanal publicado. | `PATIENT` |
| `GET` | `/psychologists/{psychologistId}/availability` | HU-20 | `AvailabilityQueryDto` | `AvailabilityResponseDto` | Consultar slots libres. | `PATIENT` |
| `GET` | `/specialties` | HU-15 | — | `SpecialtyResponseDto[]` | Listar especialidades disponibles. | `PATIENT` |
| `POST` | `/appointments` | HU-20 | `AppointmentCreateRequestDto` | `AppointmentResponseDto` | Reservar cita para el paciente autenticado; `patientId` se deriva de su sesión. | `PATIENT` |
| `GET` | `/appointments` | HU-13 | `AppointmentQueryDto` | `PageResponseDto<AppointmentResponseDto>` | Consultar únicamente las citas propias. | `PATIENT` |
| `GET` | `/appointments/{appointmentId}` | HU-13 | — | `AppointmentDetailResponseDto` | Consultar detalle de una cita propia. | `PATIENT` |
| `PUT` | `/appointments/{appointmentId}` | HU-09 | `AppointmentUpdateRequestDto` | `AppointmentResponseDto` | Reprogramar una cita propia, sujeto a política de cancelación/reprogramación. | `PATIENT` |

---

## 2. Ejemplos JSON de contratos clave

*Los ejemplos son ilustrativos y muestran la forma del JSON. Los valores UUID y fechas son ficticios. No deben interpretarse como datos reales.*

### 2.1 Login

**Request:** `POST /auth/login`

```json
{
  "email": "usuario@empresa.com",
  "password": "********"
}
```

**Response:** `200 OK`

```json
{
  "accessToken": "eyJ...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "userId": "uuid",
    "tenantId": "uuid",
    "email": "usuario@empresa.com",
    "firstName": "Ana",
    "lastName": "Pérez",
    "role": {
      "roleId": "uuid",
      "name": "PSYCHOLOGIST"
    }
  }
}
```

### 2.2 Crear una cita

**Request:** `POST /appointments`

```json
{
  "psychologistId": "uuid",
  "dateAt": "2026-11-20",
  "startsAt": "10:00",
  "endsAt": "10:50"
}
```

**Response:** `201 Created`

```json
{
  "appointmentId": "uuid",
  "patientId": "uuid-del-paciente-autenticado",
  "psychologistId": "uuid",
  "dateAt": "2026-11-20",
  "startsAt": "10:00",
  "endsAt": "10:50"
}
```

### 2.3 Crear un expediente

**Request:** `POST /records`

```json
{
  "patientId": "uuid",
  "appointmentId": "uuid",
  "riskScore": 2
}
```

**Response:** `201 Created`

```json
{
  "recordId": "uuid",
  "patient": {
    "patientId": "uuid",
    "...": "..."
  },
  "appointment": {
    "appointmentId": "uuid",
    "...": "..."
  },
  "riskScore": 2,
  "diagnoses": [],
  "prescriptions": []
}
```

### 2.4 Asociar diagnóstico a expediente

**Request:** `PUT /records/{recordId}/diagnoses/{diagnosisId}`

```json
{
  "notes": "Observaciones clínicas vinculadas a este diagnóstico."
}
```

**Response:** `200 OK`

```json
{
  "recordId": "uuid",
  "diagnosis": {
    "diagnosisId": "uuid",
    "name": "..."
  },
  "notes": "Observaciones clínicas vinculadas a este diagnóstico."
}
```

---

## 5. Reglas transversales recomendadas

### 5.1 Autenticación y autorización

| Regla | Aplicación |
| :--- | :--- |
| **Autenticación** | Todos los endpoints excepto `POST /auth/login` y `POST /tenants` requieren token válido. |
| **Roles** | Validar el rol en el servidor. Ocultar botones en frontend no constituye autorización. |
| **Multi-tenancy** | Aplicar `tenantId` del principal autenticado en todas las consultas y escrituras de datos empresariales. |
| **`SUPER_ADMIN`** | Puede operar sobre tenants; no debe recibir acceso automático al contenido clínico de cada tenant. |
| **`PSYCHOLOGIST`** | Acceso a expedientes y diagnósticos únicamente cuando exista una relación asistencial/autorización válida. |
| **`PATIENT`** | Solo puede consultar y modificar sus propios datos y citas permitidas. |
| **`ADMIN`** | Administra usuarios y catálogos operativos del tenant. El acceso a contenido clínico debe definirse explícitamente y aplicar mínimo privilegio. |

### 5.2 Validaciones de dominio

| Entidad/operación | Validaciones mínimas |
| :--- | :--- |
| **Tenant** | `name` único; `email` válido si se informa; `active` boolean. |
| **User** | `email` único; contraseña con hash seguro; rol permitido; no exponer `passwordHash`. |
| **Patient** | `birthDate` válida y no futura; perfil y usuario pertenecen al mismo tenant. |
| **Psychologist** | `licenseNumber` de 6 caracteres; `specialtyId` existente; licencia única si la regla del negocio lo exige. |
| **Schedule** | `endsAt > startsAt`; `weekDay` válido; no permitir bloques solapados para el mismo psicólogo. |
| **Appointment** | `endsAt > startsAt`; slot dentro del horario; no solapamiento con otra cita; validar zona horaria. |
| **PatientRecord** | `riskScore` entre `0` y `3`; paciente y cita pertenecen al tenant; no duplicar expediente si se mantiene la asunción 1:1. |
| **Diagnosis** | No borrar físicamente si está referenciado por expedientes o pacientes; considerar desactivación lógica. |
| **Prescription** | `quantity > 0`; `medicationId` existente; receta pertenece al expediente indicado. |

### 5.3 Privacidad y auditoría clínica

Los expedientes, diagnósticos, puntajes de riesgo y recetas son información de alta sensibilidad. Aplicar control de acceso por rol y relación asistencial, HTTPS, cifrado en reposo, auditoría de lecturas/escrituras clínicas, retención definida y registros de aplicación sin datos clínicos ni contraseñas. En respuestas para listados, devolver únicamente los campos necesarios.

### 5.4 Consistencia de nombres

El inventario original contiene pequeñas variaciones tipográficas (por ejemplo, `Diagnosis RequestDto` frente a `DiagnosisRequestDto` ya normalizado, o `Prescription` como nombre de PK en la tabla). Este documento normaliza los nombres de DTO a `PascalCase` y recomienda usar `prescriptionId` en la API aunque la columna del diagrama aparezca como `prescription`. Antes de implementar, el equipo debe fijar estos nombres en un único contrato OpenAPI.

---
