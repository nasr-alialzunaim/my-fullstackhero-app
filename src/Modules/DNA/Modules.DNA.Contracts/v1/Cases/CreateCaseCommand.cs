using Mediator;

namespace FSH.Modules.DNA.Contracts.v1.Cases;

public sealed record CreateCaseCommand(
    string CaseNumber,
    string Title,
    string? Description = null) : ICommand<Guid>;
