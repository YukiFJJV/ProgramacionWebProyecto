# Reglas del Proyecto - Consultorio (.NET 10)

## Alcance de Desarrollo

- Trabaja ÚNICAMENTE dentro del proyecto `PsyLink/Psylink.Application`.
- La capa `Domain` ya está completamente implementada y cerrada. **NO modifiques `Psylink.Domain`, `Psylink.Infrastructure` ni `Psylink.Api`** bajo ninguna circunstancia.
- Utiliza directamente las entidades, enums y objetos de valor ya existentes en `Psylink/Psylink.Domain`.

## Arquitectura y Estructura de Carpetas Obligatoria

Todos los archivos creados en `PsyLink.Application` deben seguir estrictamente esta estructura de directorios y espacios de nombres (*namespaces*):

```text
PsyLink.Application/
├── DependencyInjection.cs
├── Interfaces/
│   └── I<Entidad>Repository.cs (y demás interfaces de servicios/repositorios)
└── Features/
    ├── Behaviors/
    │   └── ValidationBehavior.cs
    └── <Modulo>/ (ej. Empresas, Pacientes, Psicologos, Citas, Expedientes, Catalogos)
        ├── DTOs/
        │   └── <Nombre>Dto.cs
        ├── Services/
        │   ├── Commands/
        │   │   └── <Accion><Entidad>Command.cs
        │   ├── Queries/
        │   │   └── <Consulta><Entidad>Query.cs
        │   └── Handlers/
        │       └── <CasoDeUso>/ (ej. CreateEmpresa, GetEmpresas)
        │           └── <CasoDeUso>Handler.cs
        └── Validators/
            └── <CasoDeUso>Validator.cs

## Stack y Patrones en Application

- **Arquitectura:** CQRS usando `MediatR` (`IRequest`, `IRequestHandler`).
- **Validaciones:** `FluentValidation` (`AbstractValidator<T>`) obligatorio por cada Command/Query que reciba parámetros.
- **Commands y Queries:** Usa `record` de C# para definir los comandos y consultas de entrada.
- **DTOs (Regla Estricta):** Todos los DTOs deben implementarse como **clases normales** (`public class`), nunca como `record`. Todas sus propiedades deben llevar su tipo explícito y descriptores de acceso `{ get; set; }`:
  ```csharp
  public class NombreDto
  {
      public TipoDato Propiedad1 { get; set; }
      public TipoDato Propiedad2 { get; set; }
  }

## Flujo Obligatorio por Historia de Usuario (HU)

Cada vez que se te solicite implementar un caso de uso o Historia de Usuario, debes leer primero la HU correspondiente dentro del archivo HU.md para extraer sus requisitos y criterios de aceptación. Además en cada ejecución del respectivo HU deberás de leer también su respectivo DTO para saber qué contiene ese DTO de ese HU en el markdown de DTOS.md.

Una vez leída la HU, ejecuta la creación de archivos en Application siguiendo estrictamente este orden:

### Paso 1 — Interfaces

Crea o actualiza primero las interfaces de repositorio o servicios necesarios dentro de Application/Common/Interfaces/ (o la carpeta de interfaces establecida en Application) basándote en las operaciones que exige la HU y las entidades de Domain.

### Paso 2 — DTOs

Crea los DTOs de entrada/salida necesarios dentro de Application/Features/<Modulo>/DTOs/ respetando estrictamente el formato public class con propiedades { get; set; }.

### Paso 3 — Casos de Uso (CQRS + Validaciones)

Finalmente, crea dentro de Application/Features/<Modulo>/ los componentes del caso de uso:

Command o Query (record que implementa IRequest<T>)

Validator (AbstractValidator<T> con las reglas de validación de la HU).

Handler (IRequestHandler<TRequest, TResponse> que inyecta las interfaces del Paso 1 y retorna los DTOs del Paso 2).

Verificación
Al terminar los 3 pasos de la HU, ejecuta dotnet build en la terminal para comprobar que la capa Application compila sin errores ni advertencias de referencia
