namespace PsyLink.Domain.Clinical;

/// <summary>Niveles de importancia de la cita (risk_score, CHECK 0 &lt;= risk_score &lt; 4).</summary>
public static class RiskLevel
{
    /// <summary>Primera cita o paciente que debe ser reevaluado.</summary>
    public const int FirstVisit = 0;
    public const int Low = 1;
    public const int Medium = 2;
    public const int Urgent = 3;

    public static bool IsValid(int value) => value is >= FirstVisit and <= Urgent;

    /// <summary>Solo el psicólogo puede asignar 1, 2 o 3.</summary>
    public static bool IsAssignableByPsychologist(int value) => value is >= Low and <= Urgent;
}
