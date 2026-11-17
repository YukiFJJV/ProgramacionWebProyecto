namespace SaludMental.Domain.Identity;

/// <summary>
/// Plan de suscripción. El ER referencia subplan.plan_id pero no define esta tabla;
/// se agrega mínima para completar el modelo. Confirmar con el equipo.
/// </summary>
public class Plan
{
    public Guid PlanId { get; set; }
    public string Name { get; set; } = string.Empty;
}
