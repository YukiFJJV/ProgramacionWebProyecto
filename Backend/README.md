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
