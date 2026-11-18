namespace PsyLink.API.Authorization;

public static class Policies
{
    public const string SuperAdminOnly = "SuperAdminOnly";
    public const string AdminOnly = "AdminOnly";
    public const string PsychologistOnly = "PsychologistOnly";
    public const string PatientOnly = "PatientOnly";
    public const string AdminOrPsychologist = "AdminOrPsychologist";
    public const string AdminOrPatient = "AdminOrPatient";
    public const string AdminPsychologistOrPatient = "AdminPsychologistOrPatient";
}
