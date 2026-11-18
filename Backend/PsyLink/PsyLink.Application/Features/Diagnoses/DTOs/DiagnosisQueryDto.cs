namespace PsyLink.Application.Features.Diagnoses.DTOs;

public class DiagnosisQueryDto
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortDirection { get; set; } = "asc";
}
