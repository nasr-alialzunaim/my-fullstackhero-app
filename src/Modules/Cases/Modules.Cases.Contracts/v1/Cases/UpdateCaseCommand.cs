using Mediator;

namespace FSH.Modules.Cases.Contracts.v1.Cases;

public sealed record UpdateCaseCommand(
    Guid CaseId,
    string Number,
    string Title,
    string? Description = null) : ICommand<Guid>;