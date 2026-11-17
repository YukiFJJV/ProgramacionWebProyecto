# Especificación de Requisitos y Flujo Metodológico
## Historias de Usuario Ordenadas y Estructuradas
**Plataforma SaaS B2B de Salud Mental | Arquitectura por Capas Funcionales**

---

## Fase 1: Onboarding, Registro Base y Autenticación
*Establece la infraestructura del modelo Multi-Tenant y el acceso general al sistema.*

### HU-21: Registrar Empresa (Tenant)
* **Rol:** `PÚBLICO / PROSPECTO`
* **Historia:**
  * **Como** representante corporativo,
  * **quiero** registrar mi empresa completando la información básica del tenant,
  * **para** contratar e iniciar la utilización de los servicios de salud mental de la plataforma.
* **Criterios de Aceptación:**
  * Solicitar y registrar: Nombre comercial, Correo electrónico de contacto, Teléfono (8 dígitos) y RTN.
  * Validar de forma estricta que el Nombre comercial y el RTN no se encuentren previamente registrados.
  * Asignar automáticamente el estado "Activo" a la empresa registrada.

### HU-22: Iniciar Sesión en la Plataforma
* **Rol:** `USUARIO REGISTRADO`
* **Historia:**
  * **Como** usuario registrado (Administrador, Psicólogo o Paciente),
  * **quiero** autenticarme introduciendo mi correo electrónico y contraseña,
  * **para** acceder de manera segura a las funcionalidades y datos asignados a mi rol.
* **Criterios de Aceptación:**
  * Solicitar credenciales de acceso: Correo electrónico y Contraseña.
  * Validar las credenciales contra la base de datos y comprobar el estado activo del usuario.
  * Generar un token de sesión seguro (JWT) devolviendo el rol e identificador de tenant correspondiente.

---

## Fase 2: Configuración Global y Gestión de Empresas
*Administración central de cuentas corporativas y especialidades médicas del sistema.*

### HU-01: Consultar Empresas Registradas
* **Rol:** `SUPER ADMIN`
* **Historia:**
  * **Como** administrador global del sistema,
  * **quiero** visualizar y buscar dentro del listado de empresas clientes,
  * **para** monitorear las organizaciones registradas y supervisar quién utiliza la plataforma.
* **Criterios de Aceptación:**
  * Mostrar el listado paginado de empresas con sus datos: Nombre, Correo, Teléfono, RTN y Estado.
  * Permitir la búsqueda y filtrado de empresas por su identificador único (UUID), nombre o RTN.

### HU-02: Actualizar Datos de Empresa
* **Rol:** `SUPER ADMIN`
* **Historia:**
  * **Como** administrador global de la plataforma,
  * **quiero** modificar la información registrada de una empresa,
  * **para** mantener su perfil corporativo actualizado y gestionar su estado operacional.
* **Criterios de Aceptación:**
  * Permitir la edición de: Nombre, Correo electrónico, RTN, Teléfono y Estado de activación.
  * Validar la obligatoriedad de campos críticos y evitar la duplicidad de nombres entre empresas.

### HU-07: Administrar Catálogo de Especialidades
* **Rol:** `ADMINISTRADOR / PSICÓLOGO`
* **Historia:**
  * **Como** administrador de la plataforma,
  * **quiero** crear, consultar y actualizar las especialidades de atención psicológica,
  * **para** categorizar correctamente al equipo profesional y clasificar las atenciones por área.
* **Criterios de Aceptación:**
  * Registrar nuevas especialidades especificando un nombre único.
  * Consultar el catálogo completo de especialidades registradas.
  * Permitir modificar el nombre de una especialidad existente bloqueando duplicados.

---

## Fase 3: Gestión de Usuarios, Personal Clínico y Pacientes
*Administración interna de roles, expedición de licencias de psicólogos y altas de pacientes.*

### HU-03: Administrar Usuarios y Asignar Roles
* **Rol:** `ADMIN EMPRESA`
* **Historia:**
  * **Como** administrador de una empresa,
  * **quiero** registrar usuarios colaboradores y asignarles los roles del sistema,
  * **para** delegar responsabilidades y otorgar permisos de acceso adecuados dentro de la organización.
* **Criterios de Aceptación:**
  * Crear cuentas de usuario especificando nombre, correo, teléfono y contraseña inicial.
  * Asignar un rol válido (excluyendo el rol de Super Admin).

### HU-04: Registrar Paciente en la Empresa
* **Rol:** `ADMIN EMPRESA`
* **Historia:**
  * **Como** administrador de una empresa,
  * **quiero** crear el perfil de un paciente asociándolo a la organización,
  * **para** almacenar su información general y habilitar la gestión de su atención psicológica.
* **Criterios de Aceptación:**
  * Vincular el perfil de paciente con una cuenta de usuario previamente validada en el sistema.
  * Registrar obligatoriamente la fecha de nacimiento (no futura) y opcionalmente una descripción.
  * Asociar automáticamente el paciente al Tenant (empresa) del administrador autenticado.

### HU-05: Registrar Psicólogo
* **Rol:** `ADMIN EMPRESA`
* **Historia:**
  * **Como** administrador de una empresa,
  * **quiero** registrar a nuevos profesionales de la psicología en el sistema,
  * **para** habilitar su perfil clínico y permitir la prestación de servicios a pacientes.
* **Criterios de Aceptación:**
  * Crear la cuenta de usuario con rol Psicólogo y validar el número de colegiación/licencia (6 caracteres).
  * Seleccionar obligatoriamente una especialidad existente en el catálogo.
  * Vincular al psicólogo con la empresa solicitante.

### HU-06: Consultar Psicólogos de la Empresa
* **Rol:** `ADMIN EMPRESA`
* **Historia:**
  * **Como** administrador de empresa,
  * **quiero** consultar y filtrar la lista de psicólogos registrados,
  * **para** evaluar el personal activo, sus especialidades y revisar su disponibilidad de atención.
* **Criterios de Aceptación:**
  * Mostrar únicamente los psicólogos pertenecientes a la empresa del usuario en sesión.
  * Desplegar Nombre completo, Número de licencia y Especialidad.
  * Permitir realizar búsquedas por nombre del psicólogo o por especialidad.

---

## Fase 4: Gestor de Catálogos Clínicos (Diagnósticos y Medicamentos)
*Mantenimiento de la base de conocimientos clínicos, manuales diagnósticos y farmacología.*

### HU-10: Gestionar Catálogo de Diagnósticos
* **Rol:** `ADMINISTRADOR`
* **Historia:**
  * **Como** administrador del sistema,
  * **quiero** administrar la lista maestra de diagnósticos psicológicos y sus tratamientos asociados,
  * **para** mantener actualizada la base diagnóstica utilizada en las evaluaciones clínicas.
* **Criterios de Aceptación:**
  * Añadir, modificar o eliminar registros en el catálogo de diagnósticos.
  * Agregar nombre, descripción detallada y asociar medicamentos sugeridos para cada diagnóstico.
  * Impedir borrado físico si el diagnóstico está referenciado en expedientes activos.

### HU-11: Registrar Medicamento en el Catálogo
* **Rol:** `ADMINISTRADOR`
* **Historia:**
  * **Como** administrador del sistema,
  * **quiero** agregar un nuevo medicamento especificando su fabricante y presentación,
  * **para** disponibilizarlo al momento de recetar tratamientos dentro de un expediente.
* **Criterios de Aceptación:**
  * Registrar: Nombre comercial/genérico, Laboratorio, Contenido (cantidad) y Presentación.
  * Permitir la asociación con principios activos existentes.

### HU-12: Consultar Catálogo de Diagnósticos
* **Rol:** `PSICÓLOGO`
* **Historia:**
  * **Como** psicólogo,
  * **quiero** consultar el catálogo de diagnósticos durante la atención al paciente,
  * **para** identificar y seleccionar con precisión las patologías correspondientes a su cuadro clínico.
* **Criterios de Aceptación:**
  * Visualizar los diagnósticos disponibles mostrando nombre y descripción completa.
  * Permitir búsqueda rápida mediante coincidencia de texto por nombre de diagnóstico.

---

## Fase 5: Horarios, Disponibilidad y Programación de Citas
*Módulo de agenda operacional para psicólogos y reserva de citas presenciales o virtuales.*

### HU-08: Registrar Horarios de Trabajo
* **Rol:** `ADMIN EMPRESA`
* **Historia:**
  * **Como** administrador de empresa,
  * **quiero** definir y actualizar los bloques de horario laboral de los psicólogos,
  * **para** establecer la disponibilidad en la que los profesionales ofrecerán consulta.
* **Criterios de Aceptación:**
  * Seleccionar un psicólogo de la empresa y un día específico de la semana (Lunes a Domingo).
  * Establecer hora de inicio y finalización del bloque (validando que la hora final sea posterior a la inicial).
  * Evitar el solapamiento o traslape de bloques de horario para el mismo psicólogo.

### HU-15: Consultar Perfil y Disponibilidad del Psicólogo
* **Rol:** `PACIENTE`
* **Historia:**
  * **Como** paciente,
  * **quiero** consultar la ficha profesional, especialidad y horarios de un psicólogo,
  * **para** tomar una decisión informada y elegir al profesional idóneo para mi atención.
* **Criterios de Aceptación:**
  * Mostrar la especialidad asignada, datos de contacto profesional y biografía.
  * Visualizar los bloques de horarios libres y slots de citas disponibles para reserva.

### HU-20: Programar Cita Médica
* **Rol:** `PACIENTE / PSICÓLOGO`
* **Historia:**
  * **Como** paciente (o psicólogo en caso de paciente con nivel de riesgo 3),
  * **quiero** agendar una cita fijando la fecha y la hora de atención,
  * **para** asegurar un espacio en la agenda y recibir atención psicológica oportunamente.
* **Criterios de Aceptación:**
  * Seleccionar psicólogo, fecha, hora inicial y hora final de la sesión.
  * Validar que la cita programada se encuentre dentro del horario laboral registrado del psicólogo.
  * Bloquear la creación de citas simultáneas o solapadas para el mismo profesional.
  * Si el paciente posee un score de riesgo alto (`riskScore = 3`), permitir que el psicólogo programe la cita directamente.

### HU-09: Modificar o Reprogramar Cita
* **Rol:** `PACIENTE / ADMIN`
* **Historia:**
  * **Como** cliente o administrador de la empresa,
  * **quiero** cambiar la fecha, hora o el psicólogo asignado a una cita agendada,
  * **para** ajustar la atención ante imprevistos o cambios de disponibilidad.
* **Criterios de Aceptación:**
  * Permitir modificar la fecha y el horario validando disponibilidad previa.
  * En el caso del cliente/paciente, restringir la reprogramación a un máximo de 1 cambio por cita.

---

## Fase 6: Atenciones Clínicas, Expediente y Tratamientos
*Módulo central de salud: seguimiento de sesiones, expedientes médicos y prescripción de fármacos.*

### HU-13: Consultar Citas del Paciente
* **Rol:** `PSICÓLOGO`
* **Historia:**
  * **Como** psicólogo,
  * **quiero** revisar el historial de citas que he atendido o tengo programadas,
  * **para** acceder a los detalles del paciente antes y después de cada sesión.
* **Criterios de Aceptación:**
  * Mostrar únicamente los pacientes asignados o citados con el psicólogo en sesión.
  * Permitir búsqueda por nombre o correo electrónico del paciente.
  * Desplegar la información completa de la cita seleccionada.

### HU-14: Actualizar Notas e Historial de Cita
* **Rol:** `PSICÓLOGO`
* **Historia:**
  * **Como** psicólogo,
  * **quiero** registrar notas clínicas y avances tras una consulta,
  * **para** mantener el seguimiento de la evolución del paciente en el sistema.
* **Criterios de Aceptación:**
  * Añadir y editar notas descriptivas específicas de la sesión realizada.
  * Actualizar el estado del historial general del paciente.

### HU-17: Crear Expediente Clínico Inicial
* **Rol:** `PSICÓLOGO`
* **Historia:**
  * **Como** psicólogo,
  * **quiero** aperturar el expediente clínico durante la primera cita del paciente,
  * **para** consolidar su historial médico oficial y registrar la evaluación de riesgo inicial.
* **Criterios de Aceptación:**
  * Asociar de manera unívoca el expediente clínico con el ID del paciente.
  * Registrar la evaluación del nivel de riesgo dentro de la escala entera de `0` a `3` (`CHECK: risk_score < 4`).

### HU-18: Consultar Expediente del Paciente
* **Rol:** `PSICÓLOGO`
* **Historia:**
  * **Como** psicólogo autorizador,
  * **quiero** acceder a la información médica y antecedentes guardados en el expediente del paciente,
  * **para** fundamentar el diagnóstico y tratamiento con base en su historial clínico previo.
* **Criterios de Aceptación:**
  * Validar que el psicólogo que consulta mantenga la relación asistencial activa (verificando que no exista cambio de psicólogo en `appointments`).
  * Mostrar datos del paciente, historial acumulado de citas y nivel de riesgo actual.

### HU-19: Actualizar Expediente Clínico y Recetas
* **Rol:** `PSICÓLOGO`
* **Historia:**
  * **Como** psicólogo a cargo,
  * **quiero** actualizar el puntaje de riesgo y prescribir medicamentos dentro del expediente,
  * **para** ajustar el plan de tratamiento según la evolución clínica alcanzada.
* **Criterios de Aceptación:**
  * Modificar el valor de nivel de riesgo (`0` a `3`).
  * Añadir o remover prescripciones de medicamentos especificando dosis estructurada e instrucciones.
  * Modificar las dosis y cantidades prescritas de medicamentos activos.

---

## Matriz de Secuencia y Cobertura de Roles

| Fase Metodológica | Historias de Usuario | Rol Principal | Dependencia Requerida |
| :--- | :--- | :--- | :--- |
| **1. Registro & Auth** | HU-21, HU-22 | Público / Usuario | Ninguna (Base de Inicio) |
| **2. Configuración SaaS** | HU-01, HU-02, HU-07 | Super Admin | Fase 1 (Empresa creada) |
| **3. Usuarios & Roles** | HU-03, HU-04, HU-05, HU-06 | Admin Empresa | Fase 2 (Especialidades creadas) |
| **4. Catálogos Médicos** | HU-10, HU-11, HU-12 | Admin / Psicólogo | Fase 3 (Acceso configurado) |
| **5. Agenda & Citas** | HU-08, HU-15, HU-20, HU-09 | Paciente / Psicólogo | Fase 3 (Psicólogos y Pacientes) |
| **6. Atención Clínica** | HU-13, HU-14, HU-17, HU-18, HU-19 | Psicólogo | Fase 5 (Cita existente) |