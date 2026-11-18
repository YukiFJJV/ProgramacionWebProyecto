<!-- CITAS DE PACIENTES -->
# Algoritmo de las citas

Debemos desarrollar un algoritmo el cual; en base al tiempo disponible de los psicologos, y la importancia de la cita; calcule de manera óptima a qué paciente y en qué horario se le debe atender.

Regla de negocio: Hay 4 niveles de importancia (risk_score)-> 0, 1, 2, 3
0: Primera cita
1: Nivel de atención necesitado -> bajo
2: Nivel de atención necesitado -> medio
3: Nivel de atención necesitado -> urgente
Los niveles 1, 2, y 3 tienen que ser estrictamente asignados por el psicologo que atiende al paciente en la primera cita. Cuando el paciente agenda la cita por primera vez se le asigna automaticamente el valor 0. Luego de que el paciente no asista a citas psicologicas, después de 10 meses su nivel de atención baja automaticamente a 0 de nuevo, debido a que tiene que ser reevaluado.

Regla de Negocio: El tiempo disponible de los psicologos se calcula en base a la tabla work_schedule; la cual indica qué horario de trabajo tiene el respectivo psicologo; y también, en base a la tabla appointmet; la cual es un registro de todas las citas realizadas con anterioridad y de las citas ocupadas actualmente.


*La creación de usuarios afiliados a una empresa a través un token
*La empresa se guarda hasta que ya se haya realizado el pago
*Funcionalidad para preguntar a un independiente si quiere cambiar a cuenta de empresa
*Solo se puede posponer la cita una vez
*Crear contrato para los psicólogos para que trabajen en el horario de la empresa
*Para cambiar de psicólogo debe haber completado al menos una cita con él.

## Multi-tenancy

Las entidades operativas que pertenecen a una empresa implementan `ITenantScoped` y contienen `TenantId`.
`ApplicationDbContext` aplica filtros globales de EF Core con el `TenantId` obtenido de `ICurrentUserService`.
Al insertar una entidad nueva, el contexto asigna automáticamente el tenant del usuario autenticado cuando el `TenantId` está vacío.

Las operaciones públicas de login y registro de tenant deben usar `IgnoreQueryFilters()` explícitamente.
Los métodos de repositorio no deben aceptar un `tenantId` redundante para recursos operativos: el aislamiento se garantiza por el contexto y el filtro global.

## Límites verticales

Cada caso de uso vive en su feature y expone un único `Command` o `Query`, su handler y su validator.
Los DTO compartidos por varios casos se mantienen en `Features/<Feature>/Shared`; los DTO realmente transversales viven en `Features/Shared`.

`Patients` contiene el perfil y registro de pacientes. `Diagnoses` contiene el catálogo, diagnósticos del expediente y diagnósticos asociados al paciente con sus estados.
Esta decisión evita duplicar reglas clínicas y mantiene el acceso a diagnósticos dentro del feature que posee el catálogo clínico.
