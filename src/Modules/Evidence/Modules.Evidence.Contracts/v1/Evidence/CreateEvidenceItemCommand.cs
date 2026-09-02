using Mediator;

namespace FSH.Modules.Evidence.Contracts.v1.Evidence;

public sealed record CreateEvidenceItemCommand(
    Guid CaseId,
    string? ExternalReference = null,
    string? Description = null) : ICommand<Guid>;
