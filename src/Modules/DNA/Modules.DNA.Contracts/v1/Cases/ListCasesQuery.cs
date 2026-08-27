using Mediator;

namespace FSH.Modules.DNA.Contracts.v1.Cases;

public sealed record ListCasesQuery : IQuery<IReadOnlyList<CaseListItem>>;

public sealed record CaseListItem(
    Guid Id,
    string CaseNumber,
    string Title,
    string Status,
    DateTime CreatedAtUtc);
