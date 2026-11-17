# Especificación de Endpoints y DTOs
## Plataforma SaaS B2B de servicios de salud mental
**Documento de diseño de contratos REST**  
*Basado en el diagrama entidad-relación y en el inventario de endpoints/DTOs entregados por el equipo.*  
**Versión:** 1.0 | **Fecha:** 9 de noviembre de 2026

---

## 1. Alcance y criterios de diseño

Este documento completa los nombres de endpoints y DTOs del inventario original. Mantiene sus rutas, métodos, roles y nombres de DTO siempre que aparecen en el material fuente. Las propiedades propuestas son un contrato de API recomendado; no todas existen literalmente como columnas en la base de datos.

* **Convenciones:** `UUID` se representa como string UUID; fechas como ISO 8601 (`YYYY-MM-DD`); horas como `HH:mm`; fecha-hora como ISO 8601 con zona horaria; importes como decimal; listas como arrays. Los campos marcados como requeridos deben validarse en el servidor.
* **Seguridad:** El backend debe obtener `tenantId` del contexto autenticado y comprobar que cada recurso pertenece a ese tenant. No confiar en un `tenantId` enviado por el cliente para decidir el ámbito de acceso. Nunca devolver `passwordHash`, tokens internos, ni datos clínicos a roles que no estén autorizados.
* **Nota de alcance:** El inventario no especifica códigos HTTP para todos los endpoints ni el mecanismo de paginación. Se recomienda `200` para consultas/actualizaciones, `201` para creaciones, `204` para eliminaciones, `400` para validación, `401`/`403` para acceso y `404` para recursos inexistentes o fuera del tenant.

### 1.1 Observaciones del modelo de datos

| Hallazgo en el diagrama | Implicación para el contrato |
| :--- | :--- |
| `subplan.plan_id` referencia una entidad `plan` que no aparece en el diagrama. | No se define `PlanResponseDto` ni endpoints de planes. Se expone `planId` como referencia hasta completar el modelo. |
| `psychologist.admin_id` aparece como FK, pero no se indica a qué tabla apunta. | Se trata como referencia opcional a un usuario administrador; confirmar FK y cardinalidad con el equipo. |
| `patient_record` contiene `patient_id`, `tenant_id` y `appointment_id`; no se indica unicidad. | Se asume un expediente por paciente dentro del tenant. Si se permiten varios, el `GET /patients/{patientId}/record` debe convertirse en listado o aceptar filtros. |
| `appointment`, `prescription` y `payment_history` no muestran estados explícitos. | Los DTOs no inventan columnas de estado persistente. Se recomienda agregar estados si el flujo los necesita. |
| No aparecen columnas `created_at`/`updated_at` en la mayoría de tablas. | No se exponen timestamps de auditoría salvo `updatedAt` en `patient_diagnosis`, que sí aparece en el diagrama. |
| No hay tabla de refresh tokens ni sesiones. | `LoginResponseDto` contempla `accessToken` y `expiresIn`. No se asume persistencia de refresh tokens. |

### 1.2 Relaciones principales

| Relación | Lectura de diseño |
| :--- | :--- |
| `Tenant` 1-N `User`, `Patient`, `Psychologist`, `Appointment`, `PatientRecord` | Los recursos operativos se aíslan por tenant. |
| `User` 1-0..1 `Patient` / 0..1 `Psychologist` | `Patient` y `Psychologist` amplían el perfil de `User`. Un usuario no debe recibir ambos perfiles salvo regla explícita. |
| `Psychologist` 1-N `WorkSchedule` y `Appointment` | Horarios y citas pertenecen a un psicólogo y a un tenant. |
| `Patient` 1-N `Appointment`; `Patient` 1-0..1 `PatientRecord` (asunción) | El expediente consolida diagnósticos, notas/riesgo y recetas del paciente. |
| `PatientRecord` N-M `DiagnosisCatalog` vía `RecordDiagnosis` | La relación lleva `notes`. |
| `Patient` N-M `DiagnosisCatalog` vía `PatientDiagnosis` | La relación incluye `statusId` y `updatedAt`. |
| `Medication` N-M `ActiveIngredient` vía `MedicationIngredients` | `MedicationResponseDto` puede incluir ingredientes. |
| `PatientRecord` 1-N `Prescription`; `Medication` 1-N `Prescription` | Cada receta vincula un medicamento con dosis y cantidad. |
| `Tenant` 1-N `Subscription`; `Subplan` 1-N `Subscription` | `Subscription` vincula tenant, plan/subplan y usuario asociado. |

---

## 2. Catálogo de DTOs

Los DTOs se describen una sola vez aquí y se reutilizan en los endpoints. La columna **Req.** indica si el campo debe estar presente en la solicitud o respuesta.

### Autenticación y Sesión

#### `LoginRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `email` | `string` | Sí | Correo de la cuenta. |
| `password` | `string` | Sí | Contraseña en texto solo durante el transporte HTTPS; nunca se almacena ni registra en logs. |

> **Nota:** No incluir `tenantId` como autoridad de acceso. Si el login necesita resolver tenant por dominio/código, definir ese dato explícitamente.

#### `LoginResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `accessToken` | `string` | Sí | JWT o token de acceso. |
| `tokenType` | `string` | Sí | Normalmente `Bearer`. |
| `expiresIn` | `integer` | Sí | Duración del token en segundos. |
| `user` | `UserSessionDto` | Sí | Identidad mínima y rol del usuario autenticado. |

#### `UserSessionDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `userId` | `UUID` | Sí | Identificador del usuario. |
| `tenantId` | `UUID` | Condicional | Tenant al que pertenece; `null` solo para cuentas globales `SUPER_ADMIN`. |
| `email` | `string` | Sí | Correo. |
| `firstName` | `string` | Sí | Nombre. |
| `lastName` | `string` | Sí | Apellido. |
| `role` | `RoleResponseDto` | Sí | Rol y permisos generales. |

---

### Empresas (Tenants)

#### `TenantCreateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `name` | `string (1-100)` | Sí | Nombre único de la empresa. |
| `email` | `string` | No | Correo de contacto. |
| `rtn` | `string` | No | Identificación tributaria. |
| `phoneNumber` | `string (8 dígitos)` | No | Teléfono; se crea o reutiliza `PhoneNumber`. |

#### `TenantUpdateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `name` | `string (1-100)` | No | Nombre de la empresa. |
| `email` | `string \| null` | No | Correo de contacto; `null` para limpiar. |
| `rtn` | `string \| null` | No | RTN. |
| `phoneNumber` | `string \| null` | No | Teléfono. |
| `active` | `boolean` | No | Activación de la empresa. |

#### `TenantQueryDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `search` | `string` | No | Busca por nombre, correo o RTN. |
| `active` | `boolean` | No | Filtra por estado. |
| `page` | `integer >= 1` | No | Página; default `1`. |
| `pageSize` | `integer 1-100` | No | Elementos; default `20`. |
| `sortBy` | `string` | No | Campo permitido, por ejemplo `name`. |
| `sortDirection` | `asc \| desc` | No | Orden; default `asc`. |

#### `TenantResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `tenantId` | `UUID` | Sí | PK tenant. |
| `name` | `string` | Sí | Nombre. |
| `email` | `string \| null` | No | Correo. |
| `rtn` | `string \| null` | No | RTN. |
| `phone` | `PhoneNumberResponseDto \| null` | No | Teléfono asociado. |
| `active` | `boolean` | Sí | Estado de la empresa. |

---

### Comunes y Roles

#### `PhoneNumberResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `phoneId` | `UUID` | Sí | PK del teléfono. |
| `phoneNumber` | `string` | Sí | Número de ocho dígitos según el modelo. |

#### `RoleResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `roleId` | `UUID` | Sí | PK del rol. |
| `name` | `string` | Sí | Nombre del rol, por ejemplo `SUPER_ADMIN`, `ADMIN`, `PSYCHOLOGIST` o `PATIENT`. |

---

### Usuarios

#### `UserCreateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `email` | `string` | Sí | Correo único. |
| `password` | `string` | Sí | Contraseña inicial; validar política de longitud y complejidad. |
| `firstName` | `string` | Sí | Nombre. |
| `lastName` | `string` | Sí | Apellido. |
| `phoneNumber` | `string` | No | Número de teléfono. |
| `roleId` | `UUID` | Sí | Rol permitido para el `ADMIN` del tenant; bloquear asignación de `SUPER_ADMIN`. |

#### `UserUpdateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `email` | `string` | No | Nuevo correo único. |
| `firstName` | `string` | No | Nombre. |
| `lastName` | `string` | No | Apellido. |
| `phoneNumber` | `string \| null` | No | Teléfono. |
| `roleId` | `UUID` | No | Nuevo rol, sujeto a permisos. |
| `active` | `boolean` | No | Estado de acceso. No aparece como columna en `user`; requiere agregarla o derivarla. |

> **Nota:** El campo `active` es una propuesta funcional y no existe en la tabla `user` actual. Eliminarlo del DTO si no se agrega al modelo.

#### `UserQueryDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `search` | `string` | No | Busca por nombre, apellido o correo. |
| `roleId` | `UUID` | No | Filtra por rol. |
| `page` | `integer >= 1` | No | Default `1`. |
| `pageSize` | `integer 1-100` | No | Default `20`. |
| `sortBy` | `string` | No | Campo permitido. |
| `sortDirection` | `asc \| desc` | No | Default `asc`. |

#### `UserResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `userId` | `UUID` | Sí | PK. |
| `tenantId` | `UUID` | Sí | Tenant. |
| `email` | `string` | Sí | Correo. |
| `firstName` | `string` | Sí | Nombre. |
| `lastName` | `string` | Sí | Apellido. |
| `phone` | `PhoneNumberResponseDto \| null` | No | Teléfono. |
| `role` | `RoleResponseDto` | Sí | Rol asignado. |

> **Nota:** Nunca incluir `password` ni `passwordHash`.

---

### Pacientes

#### `PatientCreateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `user` | `UserCreateRequestDto` | Sí | Datos de cuenta; el backend fuerza el rol `PATIENT`. |
| `birthDate` | `date` | Sí | Fecha de nacimiento; no puede ser futura. |
| `description` | `string \| null` | No | Descripción administrativa/no clínica del paciente. |

> **Nota:** El tenant se toma del usuario autenticado. No aceptar `patientId`, `userId` ni `tenantId` del cliente.

#### `PatientUpdateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `firstName` | `string` | No | Actualiza nombre del usuario vinculado. |
| `lastName` | `string` | No | Actualiza apellido. |
| `email` | `string` | No | Actualiza correo. |
| `phoneNumber` | `string \| null` | No | Actualiza teléfono. |
| `birthDate` | `date` | No | Fecha de nacimiento. |
| `description` | `string \| null` | No | Descripción del paciente. |

#### `PatientQueryDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `search` | `string` | No | Nombre, apellido o correo. |
| `psychologistId` | `UUID` | No | Filtra pacientes vinculados a citas/atención del psicólogo; requiere regla de asignación. |
| `page` | `integer >= 1` | No | Default `1`. |
| `pageSize` | `integer 1-100` | No | Default `20`. |
| `sortBy` | `string` | No | Campo permitido. |
| `sortDirection` | `asc \| desc` | No | Default `asc`. |

#### `PatientResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `patientId` | `UUID` | Sí | PK `patient`. |
| `user` | `UserResponseDto` | Sí | Cuenta asociada. |
| `birthDate` | `date` | Sí | Fecha de nacimiento. |
| `description` | `string \| null` | No | Descripción. |
| `age` | `integer` | Derivado | Edad calculada; no persistirla. |

---

### Especialidades y Psicólogos

#### `SpecialtyRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `specialtyName` | `string (1-100)` | Sí | Nombre único de la especialidad. |

#### `SpecialtyResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `specialtyId` | `UUID` | Sí | PK. |
| `specialtyName` | `string` | Sí | Nombre. |

#### `PsychologistCreateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `user` | `UserCreateRequestDto` | Sí | Datos de cuenta; backend fuerza rol `PSYCHOLOGIST`. |
| `specialtyId` | `UUID` | Sí | Especialidad existente. |
| `licenseNumber` | `string (6)` | Sí | Número de colegiación/licencia. |
| `adminId` | `UUID` | No | Referencia al administrador responsable, si aplica. |

> **Nota:** El modelo almacena `specialty_id` directamente en `psychologist`: un psicólogo tiene una especialidad en el esquema actual.

#### `PsychologistQueryDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `search` | `string` | No | Nombre del psicólogo. |
| `specialtyId` | `UUID` | No | Filtra por especialidad. |
| `availableOn` | `date` | No | Filtra por disponibilidad en una fecha; complementa el endpoint `availability`. |
| `page` | `integer >= 1` | No | Default `1`. |
| `pageSize` | `integer 1-100` | No | Default `20`. |
| `sortBy` | `string` | No | Campo permitido. |
| `sortDirection` | `asc \| desc` | No | Default `asc`. |

#### `PsychologistSummaryResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `psychologistId` | `UUID` | Sí | PK. |
| `firstName` | `string` | Sí | Nombre desde `User`. |
| `lastName` | `string` | Sí | Apellido desde `User`. |
| `specialty` | `SpecialtyResponseDto` | Sí | Especialidad. |
| `licenseNumber` | `string` | Sí | Licencia. |

#### `PsychologistDetailResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `psychologistId` | `UUID` | Sí | PK. |
| `user` | `UserResponseDto` | Sí | Perfil de cuenta; omitir `role` si no es necesario. |
| `specialty` | `SpecialtyResponseDto` | Sí | Especialidad. |
| `licenseNumber` | `string` | Sí | Licencia. |
| `schedules` | `ScheduleResponseDto[]` | No | Horarios semanales. |

---

### Horarios y Disponibilidad

#### `ScheduleRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `weekDay` | `string enum` | Sí | Día de la semana; valores acordados por backend, por ejemplo `MONDAY`-`SUNDAY`. |
| `startsAt` | `time` | Sí | Hora inicial. |
| `endsAt` | `time` | Sí | Hora final; debe ser posterior a `startsAt`. |

> **Nota:** `psychologistId` y `tenantId` vienen de la ruta/contexto, no del body. Validar solapamientos.

#### `ScheduleResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `scheduleId` | `UUID` | Sí | PK. |
| `psychologistId` | `UUID` | Sí | Psicólogo dueño. |
| `weekDay` | `string enum` | Sí | Día. |
| `startsAt` | `time` | Sí | Inicio. |
| `endsAt` | `time` | Sí | Fin. |

#### `AvailabilityQueryDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `dateFrom` | `date` | Sí | Inicio del rango. |
| `dateTo` | `date` | Sí | Fin del rango; máximo recomendado `31` días. |
| `durationMinutes` | `integer` | No | Duración solicitada; default según configuración de citas. |
| `timezone` | `string` | No | Zona IANA para interpretar horas; recomendable `America/Tegucigalpa`. |

#### `AvailabilityResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `psychologistId` | `UUID` | Sí | Psicólogo consultado. |
| `date` | `date` | Sí | Fecha del bloque. |
| `slots` | `AvailabilitySlotDto[]` | Sí | Horarios libres. |

#### `AvailabilitySlotDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `startsAt` | `datetime` | Sí | Inicio del slot disponible. |
| `endsAt` | `datetime` | Sí | Fin del slot disponible. |

---

### Citas

#### `AppointmentCreateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `psychologistId` | `UUID` | Sí | Psicólogo elegido. |
| `dateAt` | `date` | Sí | Fecha de la cita. |
| `startsAt` | `time` | Sí | Hora de inicio. |
| `endsAt` | `time` | Sí | Hora de fin. |

> **Nota:** `patientId` y `tenantId` se derivan de la sesión cuando el paciente reserva. Para `ADMIN`/`PSYCHOLOGIST`, permitir `patientId` solo si el caso de uso lo autoriza. Validar horario, disponibilidad y solapamiento en transacción.

#### `AppointmentUpdateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `dateAt` | `date` | No | Nueva fecha. |
| `startsAt` | `time` | No | Nueva hora inicial. |
| `endsAt` | `time` | No | Nueva hora final. |

> **Nota:** No incluye `patientId` ni `tenantId`. La reprogramación debe volver a validar disponibilidad.

#### `AppointmentQueryDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `patientId` | `UUID` | No | Filtro por paciente. |
| `psychologistId` | `UUID` | No | Filtro por psicólogo. |
| `dateFrom` | `date` | No | Desde. |
| `dateTo` | `date` | No | Hasta. |
| `page` | `integer >= 1` | No | Default `1`. |
| `pageSize` | `integer 1-100` | No | Default `20`. |
| `sortDirection` | `asc \| desc` | No | Orden por fecha/hora; default `asc`. |

#### `AppointmentResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `appointmentId` | `UUID` | Sí | PK. |
| `patientId` | `UUID` | Sí | Paciente. |
| `patientName` | `string` | No | Nombre resumido. |
| `psychologistId` | `UUID` | Sí | Psicólogo. |
| `psychologistName` | `string` | No | Nombre resumido. |
| `dateAt` | `date` | Sí | Fecha. |
| `startsAt` | `time` | Sí | Inicio. |
| `endsAt` | `time` | Sí | Fin. |

#### `AppointmentDetailResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `appointmentId` | `UUID` | Sí | PK. |
| `patient` | `PatientResponseDto` | Sí | Paciente asociado; filtrar según rol. |
| `psychologist` | `PsychologistSummaryResponseDto` | Sí | Psicólogo. |
| `dateAt` | `date` | Sí | Fecha. |
| `startsAt` | `time` | Sí | Inicio. |
| `endsAt` | `time` | Sí | Fin. |

---

### Diagnósticos y Medicamentos

#### `DiagnosisRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `name` | `string` | Sí | Nombre del diagnóstico. |
| `description` | `string \| null` | No | Descripción. |

#### `DiagnosisQueryDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `search` | `string` | No | Busca por nombre. |
| `page` | `integer >= 1` | No | Default `1`. |
| `pageSize` | `integer 1-100` | No | Default `20`. |
| `sortDirection` | `asc \| desc` | No | Default `asc`. |

#### `DiagnosisResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `diagnosisId` | `UUID` | Sí | PK `diagnosis_catalog`. |
| `name` | `string` | Sí | Nombre. |

#### `DiagnosisDetailResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `diagnosisId` | `UUID` | Sí | PK. |
| `name` | `string` | Sí | Nombre. |
| `description` | `string \| null` | No | Descripción. |

#### `DiagnosisStatusResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `statusId` | `UUID` | Sí | PK `diagnosis_status`. |
| `name` | `string` | Sí | Nombre del estado. |

#### `PatientDiagnosisUpsertRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `statusId` | `UUID` | Sí | Estado del diagnóstico. |
| `notes` | `string \| null` | No | Observaciones del vínculo; el esquema `patient_diagnosis` no contiene `notes`, por lo que no se debe persistir aquí sin ampliar el modelo. |

> **Nota:** La tabla `patient_diagnosis` no tiene `notes`. Si se conserva este campo, agregar columna o retirarlo del contrato.

#### `PatientDiagnosisResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `patientId` | `UUID` | Sí | Paciente. |
| `diagnosis` | `DiagnosisDetailResponseDto` | Sí | Diagnóstico del catálogo. |
| `status` | `DiagnosisStatusResponseDto` | Sí | Estado actual. |
| `updatedAt` | `datetime` | Sí | Última actualización. |

#### `MedicationCreateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `laboratoryId` | `UUID` | No | Laboratorio asociado. |
| `name` | `string` | Sí | Nombre comercial/genérico. |
| `content` | `integer > 0` | Sí | Contenido/cantidad por presentación. |
| `presentation` | `string` | Sí | Presentación, por ejemplo tabletas o solución. |
| `ingredientIds` | `UUID[]` | No | Principios activos existentes. |

#### `MedicationQueryDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `search` | `string` | No | Nombre del medicamento. |
| `laboratoryId` | `UUID` | No | Filtra por laboratorio. |
| `ingredientId` | `UUID` | No | Filtra por principio activo. |
| `page` | `integer >= 1` | No | Default `1`. |
| `pageSize` | `integer 1-100` | No | Default `20`. |

#### `MedicationResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `medicationId` | `UUID` | Sí | PK. |
| `name` | `string` | Sí | Nombre. |
| `content` | `integer` | Sí | Contenido. |
| `presentation` | `string` | Sí | Presentación. |
| `laboratory` | `LaboratoryResponseDto \| null` | No | Laboratorio. |
| `ingredients` | `IngredientResponseDto[]` | No | Principios activos. |

#### `LaboratoryResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `laboratoryId` | `UUID` | Sí | PK. |
| `name` | `string` | Sí | Nombre. |
| `phone` | `PhoneNumberResponseDto \| null` | No | Teléfono asociado. |

#### `IngredientResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `ingredientId` | `UUID` | Sí | PK. |
| `name` | `string` | Sí | Nombre del principio activo. |

---

### Expedientes Clínicos y Recetas

#### `RecordCreateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `patientId` | `UUID` | Sí | Paciente al que pertenece el expediente. |
| `appointmentId` | `UUID` | No | Cita que origina el expediente. |
| `riskScore` | `integer 0-3` | Sí | Puntaje de riesgo; cumple `CHECK 0 <= risk_score < 4`. |

> **Nota:** `psychologistId` y `tenantId` se obtienen del contexto o de la cita relacionada. Validar que el profesional tenga acceso al paciente.

#### `RecordUpdateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `riskScore` | `integer 0-3` | No | Actualiza el puntaje de riesgo. |

> **Nota:** `PATCH` parcial. El diagrama no muestra otros campos de expediente; no inventar notas clínicas generales sin agregar columnas/tablas.

#### `RecordDetailResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `recordId` | `UUID` | Sí | PK. |
| `patient` | `PatientResponseDto` | Sí | Paciente. |
| `appointment` | `AppointmentResponseDto \| null` | No | Cita relacionada. |
| `riskScore` | `integer 0-3` | Sí | Puntaje de riesgo. |
| `diagnoses` | `RecordDiagnosisResponseDto[]` | Sí | Diagnósticos asociados. |
| `prescriptions` | `PrescriptionResponseDto[]` | Sí | Recetas asociadas. |

#### `RecordDiagnosisRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `notes` | `string \| null` | No | Notas del diagnóstico en el expediente. |

> **Nota:** `diagnosisId` y `recordId` provienen de la ruta. La relación `record_diagnosis` contiene `notes`.

#### `RecordDiagnosisResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `recordId` | `UUID` | Sí | Expediente. |
| `diagnosis` | `DiagnosisDetailResponseDto` | Sí | Diagnóstico. |
| `notes` | `string \| null` | No | Notas específicas de este diagnóstico en el expediente. |

#### `PrescriptionRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `medicationId` | `UUID` | Sí | Medicamento existente. |
| `dosage` | `string` | Sí | Dosis e instrucciones; texto estructurado de forma consistente. |
| `quantity` | `integer > 0` | Sí | Cantidad prescrita. |

> **Nota:** `recordId` se toma de la ruta.

#### `PrescriptionUpdateRequestDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `medicationId` | `UUID` | No | Nuevo medicamento, si se permite cambiarlo. |
| `dosage` | `string` | No | Nueva dosis/instrucciones. |
| `quantity` | `integer > 0` | No | Nueva cantidad. |

#### `PrescriptionResponseDto`
| Propiedad | Tipo | Req. | Descripción / relación |
| :--- | :--- | :--- | :--- |
| `prescriptionId` | `UUID` | Sí | PK `prescription`. |
| `recordId` | `UUID` | Sí | Expediente. |
| `medication` | `MedicationResponseDto` | Sí | Medicamento. |
| `dosage` | `string \| null` | No | Dosis. |
| `quantity` | `integer \| null` | No | Cantidad. |

#### `PatientDiagnosisResponseDto[]`
> **Nota:** Es una colección de `PatientDiagnosisResponseDto`; no es un DTO independiente. Referencia explicativa para el tipo de salida.

---

### 2.1 Especializaciones de paginación

`PageResponseDto<T>` es el único contrato genérico. En cada endpoint se sustituye `T` por el DTO indicado. No es necesario crear clases distintas si el lenguaje permite genéricos.

#### `PageResponseDto<T>` (Contenedor genérico)
| Propiedad | Tipo | Req. | Descripción |
| :--- | :--- | :--- | :--- |
| `items` | `T[]` | Sí | Resultados de la página. |
| `page` | `integer` | Sí | Página actual. |
| `pageSize` | `integer` | Sí | Tamaño solicitado. |
| `totalItems` | `integer` | Sí | Total de registros filtrados. |
| `totalPages` | `integer` | Sí | Total de páginas. |
| `hasPrevious` | `boolean` | Sí | Indica si existe página anterior. |
| `hasNext` | `boolean` | Sí | Indica si existe página siguiente. |

#### Especializaciones implementadas
* `PageResponseDto<TenantResponseDto>`: `items` contiene `TenantResponseDto[]`.
* `PageResponseDto<UserResponseDto>`: `items` contiene `UserResponseDto[]`.
* `PageResponseDto<PatientResponseDto>`: `items` contiene `PatientResponseDto[]`.
* `PageResponseDto<PsychologistSummaryResponseDto>`: `items` contiene `PsychologistSummaryResponseDto[]`.
* `PageResponseDto<AppointmentResponseDto>`: `items` contiene `AppointmentResponseDto[]`.
* `PageResponseDto<DiagnosisResponseDto>`: `items` contiene `DiagnosisResponseDto[]`.
* `PageResponseDto<MedicationResponseDto>`: `items` contiene `MedicationResponseDto[]`.

---

## 3. Especificación de endpoints por rol

En los endpoints `GET`, los DTOs de consulta se envían como query string, no como body. En `POST`/`PUT`/`PATCH`, los DTOs de entrada se envían como JSON. Las rutas y roles proceden del inventario del equipo.

### 3.1 Público (sin sesión)

| Método | Ruta | HU | Entrada | Salida | Propósito | Rol |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `POST` | `/auth/login` | HU-22 | `LoginRequestDto` | `LoginResponseDto` | Autenticar una cuenta y emitir token de acceso. | Público |
| `POST` | `/tenants` | HU-21 | `TenantCreateRequestDto` | `TenantResponseDto` | Registrar una empresa/tenant. Debe aplicar validación de nombre único y crear relación de teléfono si se envía. | Público |

### 3.2 `SUPER_ADMIN`

| Método | Ruta | HU | Entrada | Salida | Propósito | Rol |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/tenants` | HU-01 | `TenantQueryDto` | `PageResponseDto<TenantResponseDto>` | Listar empresas con búsqueda, filtros y paginación. | `SUPER_ADMIN` |
| `GET` | `/tenants/{tenantId}` | HU-01 | — | `TenantResponseDto` | Obtener una empresa por identificador. | `SUPER_ADMIN` |
| `PUT` | `/tenants/{tenantId}` | HU-02 | `TenantUpdateRequestDto` | `TenantResponseDto` | Actualizar datos y estado de una empresa. | `SUPER_ADMIN` |

### 3.3 `ADMIN`

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

### 3.4 `PSYCHOLOGIST`

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

### 3.5 `PATIENT`

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

## 4. Ejemplos JSON de contratos clave

*Los ejemplos son ilustrativos y muestran la forma del JSON. Los valores UUID y fechas son ficticios. No deben interpretarse como datos reales.*

### 4.1 Login

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

### 4.2 Crear una cita

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

### 4.3 Crear un expediente

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

### 4.4 Asociar diagnóstico a expediente

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

## 6. Decisiones pendientes de confirmar con el equipo

| Tema | Pregunta / decisión |
| :--- | :--- |
| **Tenant de registro** | ¿`POST /tenants` crea solo la empresa o también crea al primer `ADMIN`? Si crea al administrador, ampliar `TenantCreateRequestDto` con `adminUser`. |
| **Plan y suscripción** | ¿Dónde se define la tabla `plan` y cómo se consultan/contratan planes? El ER muestra `subplan.plan_id` sin entidad `plan`. |
| **Expediente** | ¿Existe un expediente único por paciente o pueden existir varios expedientes/versiones? |
| **Asignación paciente-psicólogo** | El ER no contiene una tabla explícita de asignación. ¿La relación asistencial se deduce de citas históricas o debe agregarse `patient_psychologist`? |
| **Estados de citas y recetas** | ¿Se necesitan estados como `PENDING`, `CONFIRMED`, `CANCELLED`, `COMPLETED`? No aparecen en las tablas actuales. |
| **Notas clínicas** | El modelo solo contiene `notes` en `record_diagnosis`. ¿Se requiere evolución/notas de sesión generales? De ser así, agregar entidad/tabla específica. |
| **Notas de `patient_diagnosis`** | `PatientDiagnosisUpsertRequestDto` no debería persistir `notes` mientras la tabla no tenga esa columna. |
| **Borrado de diagnósticos** | Definir si `DELETE` realiza borrado físico, soft delete o se bloquea cuando existen relaciones. |