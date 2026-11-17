namespace PsyLink.Domain.Clinical;

/// <summary>
/// Regla de negocio del README: si el paciente no asiste a citas durante 10 meses,
/// su nivel de atención vuelve a 0 y debe ser reevaluado. Se calcula al consultar,
/// sin necesidad de un proceso programado.
/// </summary>
public static class RiskPolicy
{
    public const int ExpirationMonths = 10;

    /// <param name="storedRiskScore">Valor guardado en el expediente; null si el paciente no tiene expediente.</param>
    /// <param name="lastAttendedAt">Fecha de la última cita completada; null si nunca asistió.</param>
    /// <param name="now">Fecha y hora actuales.</param>
    public static int EffectiveRisk(int? storedRiskScore, DateTime? lastAttendedAt, DateTime now)
    {
        if (storedRiskScore is null || lastAttendedAt is null)
            return RiskLevel.FirstVisit;

        return now > lastAttendedAt.Value.AddMonths(ExpirationMonths)
            ? RiskLevel.FirstVisit
            : storedRiskScore.Value;
    }
}
