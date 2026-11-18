using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface IRecordDiagnosisRepository
{
    Task<RecordDiagnosis?> GetByRecordAndDiagnosisAsync(
        Guid recordId,
        Guid diagnosisId,
        CancellationToken cancellationToken = default);

    Task AddAsync(RecordDiagnosis recordDiagnosis, CancellationToken cancellationToken = default);

    Task UpdateAsync(RecordDiagnosis recordDiagnosis, CancellationToken cancellationToken = default);

    Task DeleteAsync(RecordDiagnosis recordDiagnosis, CancellationToken cancellationToken = default);
}
