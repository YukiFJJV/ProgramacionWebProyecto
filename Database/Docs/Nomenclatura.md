# Las convenciones y reglas de nomenclatura recomendadas en PostgreSQL se basan en el estándar SQL y las buenas prácticas de la comunidad

## Mayúsculas y minúsculas (Sensibilidad)

• Sin comillas: PostgreSQL convierte automáticamente los identificadores (tablas, columnas, funciones) a minúsculas si no se colocan entre comillas dobles ("").
• Recomendación: Escribir siempre los nombres en minúsculas utilizando guiones bajos para separar palabras (snake_case).
• Evitar comillas dobles: Si usas comillas dobles (ej. "MiTabla"), estarás obligado a usar comillas en cada consulta que hagas, lo que suele generar errores y confusión.

## Nombres de Tablas

• Se suelen nombrar en singular o en plural (ej. usuario o usuarios), pero debes mantener la misma regla en toda la base de datos.
• Usa sustantivos claros y minúsculas con guiones bajos (snake_case).

## Nombres de Columnas

• Claves primarias: La columna principal suele llamarse simplemente id o tener el nombre de la tabla más el sufijo _id (ej. usuario_id).
• Claves foráneas (Foreign Keys): Deben coincidir con el nombre de la tabla referenciada en singular más _id (ej. rol_id).
• Columnas booleanas: Se recomienda usar prefijos como is_, has_ o can_(ej. is_activo, has_permiso).
• Auditoría / Fechas de control: Usar nombres estándar como created_at o updated_at (o created_time, updated_time).

## Esquemas y Bases de Datos

• Usa nombres descriptivos en minúsculas.
• Para calificar un objeto entre esquemas se utiliza la sintaxis esquema.objeto (ej. ventas.clientes).

## Palabras reservadas y del sistema

• No uses palabras reservadas de SQL (como date, user, table, select).
• No uses nombres de columnas reservadas por el sistema de PostgreSQL (como oid, xmin, xmax, ctid).

## Constraints

• NOT NULL -> NN_table_column
• UNIQUE -> UQ_table_column
• DEFAULT -> DF_table_column
• PRIMARY KEY -> PK_table_column
• FOREIGN KEY -> FK_table_origin_table_reference_column
• CHECK -> CK_table_column
