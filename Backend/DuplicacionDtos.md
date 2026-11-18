# OBJETIVO

En la solución PsyLink (.NET 10, Clean Architecture + Vertical Slicing + CQRS con MediatR) hoy existen DTOs duplicados: unos en PsyLink.API/DTOs y están también en PsyLink.Application/Features/*/DTOs, y los controllers los copian a mano (alias using AppXxxDto = ..., bloques ToResponse, RecordDetailResponseMapper.

Debes dejar *TODOS los DTOs duplicados de PsyLink.API únicamente en PsyLink.Application, usarlos directamente desde los controllers y **eliminar por completo PsyLink.API/DTOs*. Al terminar no debe quedar ni un solo DTO, mapper de DTOs ni alias App* en PsyLink.API, y la API debe comportarse exactamente igual para el cliente (mismas rutas, mismo JSON, mismos códigos HTTP).


# AUTORIZACIÓN DE ALCANCE

AGENT.md normalmente limita el trabajo a PsyLink.API. PARA ESTA TAREA TE AUTORIZO a modificar PsyLink.Application (DTOs, Commands, Queries, handlers, validators) y PsyLink.API, y a crear tests en PsyLink.Tests. No modifiques Domain ni Infrastructure salvo que un cambio sea estrictamente indispensable para compilar, y si lo haces, menciónalo en el resumen. No cambies reglas de negocio, autorización, pipeline de MediatR ni el esquema de base de datos.

# FUENTE DE VERDAD

DTOS.md define el contrato (nombres de DTO, propiedades, tipos, requeridas/opcionales). ENDPOINTS.md define rutas, verbos, roles y códigos de estado. HU.md aporta contexto. Léelos completos ANTES de tocar código. Si el código actual contradice DTOS.md, gana DTOS.md (anótalo en el resumen y no cambies el comportamiento de ningún endpoint de forma que rompa a un cliente existente sin decirlo).

# REGLAS DE DISEÑO (obligatorias)

1. *Ubicación de los DTOs:* Application/Features/<Feature>/DTOs/ si es de un solo feature; Application/Features/<Feature>/Shared/ si lo usan varios casos del mismo feature; Application/Features/Shared/DTOs/ solo si es realmente transversal (por ejemplo PageResponseDto<T>). No muevas DTOs entre features si no es necesario.
2. *Nombres:* conserva exactamente los nombres de DTOS.md (AppointmentCreateRequestDto, PatientResponseDto, etc.). Cada nombre de DTO debe existir *una sola vez* en toda la solución. Si hay dos DTOs con el mismo nombre en features distintos de Application, consolídalos en uno compartido cuando DTOS.md los define una sola vez; si de verdad son distintos, renombra uno y justifícalo.
3. *Application no conoce ASP.NET:* los DTOs no llevan atributos de Microsoft.AspNetCore.* ([FromBody], [FromQuery], [FromRoute], [ProducesResponseType]). Esos atributos viven solo en los controllers. Los DTOs pueden usar System.ComponentModel.DataAnnotations solo si hace falta, pero la validación se mantiene en FluentValidation.
4. *Los DTOs de Application no referencian entidades de Domain.* Pueden usar enums de Domain solo si ya lo hacen hoy y DTOS.md los expone como strings (se serializan con JsonStringEnumConverter). Nunca devuelvas una entidad de Domain desde un handler ni desde un controller.
5. *Controllers delgados:* el controller recibe el DTO de Application ([FromBody]/[FromQuery]), arma el Command/Query con él (más los parámetros de ruta), hace await sender.Send(...) y devuelve el resultado *tal cual* (Ok(result), CreatedAtAction(...), NoContent()). Sin mapeo campo a campo, sin new XxxResponseDto { ... }, sin ToResponse, sin alias de using.
6. *Parámetros de ruta:* si un Command/Query necesita un id de la ruta y un body, el Command lleva ambos (UpdateTenantCommand(Guid TenantId, TenantUpdateRequestDto Body)). Si hoy el id viaja dentro del DTO, no cambies el contrato HTTP: el controller asigna el id de la ruta al Command, no se lo pide al cliente.
7. *Un solo formato de error de validación:* configura ApiBehaviorOptions.SuppressModelStateInvalidFilter = true para que la validación de entrada la haga únicamente FluentValidation vía ValidationBehavior (respuesta ValidationProblemDetails unificada por ApiExceptionHandler). Al hacerlo, *todas las propiedades requeridas según DTOS.md deben tener regla en el validator correspondiente* (NotEmpty, NotNull, rangos, longitudes, formato de email, etc.). Si falta alguna, créala. Revisa en especial que los DTOs de entrada sin validator reciban uno.
8. *Estructura del DTO:* sealed record con propiedades init, valores por defecto para string (= string.Empty), colecciones (= []) y objetos anidados no nulos requeridos por contrato, de modo que el model binding de System.Text.Json y [FromQuery] funcione con constructor sin parámetros. Los opcionales según DTOS.md son T?. Si convertir una class a record rompe un handler que muta el DTO, adapta el handler con with o inicializador, sin cambiar la lógica. Haz esta conversión en el Paso 6, no antes.

# PROCESO (por pasos; ejecuta dotnet build -warnaserror y dotnet test al terminar cada paso y no avances si falla)

## Paso 0 — Inventario (no modifiques nada todavía)

Genera y muestra en el resumen una tabla con TODOS los DTOs: nombre, ubicación en API, ubicación en Application, si coincide con DTOS.md (propiedades, tipos, nulabilidad, requeridas), y qué controller/endpoint lo usa. Detecta:

- DTOs solo en API (sin gemelo en Application) → hay que crearlos en Application.
- DTOs solo en Application (no usados por la API).
- Duplicados con propiedades distintas (diferencias de tipo o nulabilidad) → decide la versión correcta según DTOS.md.
- DTOs con el mismo nombre en dos features de Application.
- Mappers, helpers y alias en la API (RecordDetailResponseMapper, ToResponse, Map*, using App* =).
- Usos de Domain en PsyLink.API/DTOs (por ejemplo ScheduleRequestDto con WeekDay).

## Paso 1 — Reconciliar los DTOs de Application con DTOS.md

Para cada DTO de Application usado por la API, ajústalo para que coincida con DTOS.md (propiedades, tipos, nulabilidad) y con el JSON que hoy produce la API. Lleva a Application cualquier propiedad, valor por defecto o comentario XML útil que solo existiera en el DTO de la API. Mantén PageResponseDto<T> como única definición paginada en Features/Shared/DTOs, con las mismas propiedades que serializa hoy la API (incluidas HasPrevious y HasNext), y elimina TenantPageDto y UserPageDto reemplazando sus usos por PageResponseDto<T>.

## Paso 2 — Que los handlers devuelvan exactamente el contrato

Como ya no habrá capa de mapeo en la API, los handlers deben devolver DTOs de Application con la forma exacta de DTOS.md. Revisa cada handler que alimentaba un mapeo de la API (por ejemplo RecordDetailResponseMapper, los ToResponse de los controllers) y asegúrate de que su resultado ya contiene todos los campos, con las mismas transformaciones que hacía la API (por ejemplo Phone → PhoneNumberResponseDto, edad calculada, nombres concatenados). Un DTO anidado se reutiliza, no se vuelve a definir.

## Paso 3 — Reescribir los controllers

Para cada controller de PsyLink.API/Controllers:

- Elimina using PsyLink.API.DTOs; y todos los using AppXxxDto = ....
- Los parámetros [FromBody]/[FromQuery] usan el DTO de Application.
- ProducesResponseType<T>, ActionResult<T> y el valor devuelto usan los DTOs de Application.
- Elimina cualquier método privado ToResponse/Map y todo mapeo campo a campo.
- Conserva intactos: [Route], verbos, [Authorize(Policy|Roles)], [EndpointSummary], [EndpointDescription], códigos de estado, CreatedAtAction y la forma de la respuesta.
- No pongas lógica de negocio en el controller.

## Paso 4 — Eliminar los DTOs de la API

Borra todo PsyLink.API/DTOs (incluidos RecordDetailResponseMapper.cs y ScheduleRequestDto.cs). Verifica con búsquedas que en PsyLink.API no queda ningún *Dto, record/class de contrato, namespace PsyLink.API.DTOs ni alias App*. Si un tipo de la API aparece en Swagger/OpenAPI con otro nombre, corrige el nombre del esquema con [EndpointSummary] o configuración de OpenAPI, no recreando DTOs.

## Paso 5 — Configurar la API y la documentación

- En AddApiServices(): ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true), y confirma que el JsonStringEnumConverter sigue activo y que DateOnly/TimeOnly se serializan como en DTOS.md (YYYY-MM-DD, HH:mm). Si hace falta un JsonConverter para TimeOnly en formato HH:mm, regístralo para respetar el contrato.
- Actualiza PsyLink.API.http para que todos los ejemplos sigan funcionando con las rutas y cuerpos actuales.
- Restaura AGENT.md con las reglas del proyecto (no con un prompt de refactor) e incorpora la regla nueva: "Los DTOs viven solo en PsyLink.Application; la API no define DTOs; los controllers reciben y devuelven DTOs de Application sin mapeo". Actualiza la sección de DTOs del README.md.


## Paso 6 — Inmutabilidad y limpieza

Convierte los DTOs a sealed record con init según la regla 8, corrige los handlers que lo requieran, elimina using sin uso y cualquier código muerto que haya quedado (mappers, helpers, interfaces sin uso).

## Paso 7 — Pruebas automáticas que protegen el contrato

Añade en PsyLink.Tests:

1. *Test de arquitectura:* ningún tipo del ensamblado PsyLink.API tiene nombre terminado en Dto 
ni vive en un namespace que contenga DTOs; ningún tipo de PsyLink.Application referencia ensamblados de ASP.NET; todos los nombres de DTO son únicos en Application.
2. *Test de contrato contra DTOS.md:* parsea DTOS.md (encabezados #### \`NombreDto\` y la tabla de propiedades Propiedad | Tipo | Req.), localiza el tipo por nombre en el ensamblado de Application y comprueba que cada propiedad del documento existe (comparando en camelCase), con el tipo compatible y la nulabilidad coherente con "Req.". Los DTOs del documento que no existan deben aparecer en una lista explícita de pendientes, no ignorarse en silencio.
3. *Tests de serialización* de al menos LoginResponseDto, AppointmentResponseDto, PatientResponseDto, PageResponseDto<T> y RecordDetailResponseDto, comprobando nombres JSON (camelCase), enums como string, DateOnly y TimeOnly.
4. *Test de binding/validación* (con WebApplicationFactory si el proyecto ya lo soporta; si no, tests de validators): un body con campos requeridos vacíos devuelve 400 con ValidationProblemDetails, y no un 400 de ModelState con otro formato.

# CRITERIOS DE ACEPTACIÓN 

- [ ] No existe la carpeta PsyLink.API/DTOs; grep de Dto en PsyLink.API solo devuelve usos de tipos de Application.
- [ ] Cero alias using App... =, cero ToResponse y cero mapeos campo a campo en controllers.
- [ ] Cada nombre de DTO existe una sola vez en la solución; TenantPageDto y UserPageDto eliminados en favor de PageResponseDto<T>.
- [ ] Todos los DTOs de DTOS.md usados por los endpoints existen en Application con las propiedades, tipos y nulabilidad del documento.
- [ ] Las rutas, verbos, roles, códigos de estado y formas de respuesta coinciden con ENDPOINTS.md y con el comportamiento anterior.
- [ ] La validación de entrada pasa solo por FluentValidation (SuppressModelStateInvalidFilter = true) y cada campo requerido de DTOS.md tiene regla.
- [ ] Application no referencia ASP.NET; la API no referencia Domain.
- [ ] PsyLink.API.http, README.md y AGENT.md actualizados.
- [ ] dotnet build -warnaserror limpio y dotnet test en verde, incluidos los tests nuevos de contrato.

# ENTREGA

Al terminar, entrega: (1) la tabla de inventario del Paso 0 con la decisión tomada por cada DTO, (2) lista de archivos creados/movidos/eliminados, (3) diferencias detectadas entre el código anterior y DTOS.md y cómo se resolvieron, (4) cualquier cambio fuera de la API/Application, y (5) el resultado de build y tests. Haz un commit por paso. Empieza por el Paso 0 y detente a mostrar el inventario antes de seguir al Paso 1.