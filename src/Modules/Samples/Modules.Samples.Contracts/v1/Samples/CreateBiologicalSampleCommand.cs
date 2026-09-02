using Mediator;

namespace FSH.Modules.Samples.Contracts.v1.Samples;

public sealed record CreateBiologicalSampleCommand(
    Guid EvidenceItemId,
    Guid? ParentSampleId = null,
    string? ExternalSampleCode = null,
    DateTime? CollectedAtUtc = null,
    string? CollectionNote = null) : ICommand<Guid>;
