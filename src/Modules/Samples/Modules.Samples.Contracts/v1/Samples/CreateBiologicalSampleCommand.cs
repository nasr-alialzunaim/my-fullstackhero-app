using Mediator;

namespace FSH.Modules.Samples.Contracts.v1.Samples;

public sealed record CreateBiologicalSampleCommand(
    string SampleCode,
    string SampleContext,
    Guid? CaseId = null,
    Guid? SubjectId = null,
    Guid? ParentSampleId = null,
    string? ExternalSampleCode = null,
    string? SampleType = null,
    string? Matrix = null,
    string? CollectionLocation = null,
    DateTime? CollectedAtUtc = null,
    string? CollectionNote = null,
    string? ContainerCode = null,
    string? SealNumber = null) : ICommand<Guid>;
