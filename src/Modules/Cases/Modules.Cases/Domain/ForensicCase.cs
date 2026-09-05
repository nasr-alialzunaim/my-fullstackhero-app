using FSH.Framework.Core.Domain;

namespace FSH.Modules.Cases.Domain;

public enum ForensicCaseStatus
{
    Open = 1,
    UnderAnalysis = 2,
    Closed = 3,
    Archived = 4,
}

public sealed class ForensicCase : AggregateRoot<Guid>
{
    public string Number { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? CaseType { get; private set; }
    public ForensicCaseStatus Status { get; private set; }
    public string? Priority { get; private set; }
    public string? JurisdictionCode { get; private set; }
    public DateTime? IncidentAtUtc { get; private set; }
    public DateTime OpenedAtUtc { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private ForensicCase()
    {
    }

    public static ForensicCase Create(string number, string title, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        DateTime now = DateTime.UtcNow;
        return new ForensicCase { Id = Guid.CreateVersion7(), Number = number.Trim(), Title = title.Trim(), Description = NormalizeOptional(description), Status = ForensicCaseStatus.Open, OpenedAtUtc = now, CreatedAtUtc = now };
    }

    public void Update(string number, string title, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number); ArgumentException.ThrowIfNullOrWhiteSpace(title);
        Number = number.Trim(); Title = title.Trim(); Description = NormalizeOptional(description); UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ConfigureMetadata(string? caseType, string? priority, string? jurisdictionCode, DateTime? incidentAtUtc)
    {
        CaseType = NormalizeOptional(caseType); Priority = NormalizeOptional(priority); JurisdictionCode = NormalizeOptional(jurisdictionCode); IncidentAtUtc = incidentAtUtc; UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(ForensicCaseStatus status, DateTime changedAtUtc)
    {
        Status = status; ClosedAtUtc = status is ForensicCaseStatus.Closed or ForensicCaseStatus.Archived ? changedAtUtc : null; UpdatedAtUtc = changedAtUtc;
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class CaseAssignment
{
    public Guid Id { get; private set; }
    public Guid CaseId { get; private set; }
    public Guid UserId { get; private set; }
    public string AssignmentRole { get; private set; } = default!;
    public Guid AssignedByUserId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }
    public DateTime? ReleasedAtUtc { get; private set; }
    private CaseAssignment() { }
}

public sealed class CaseStatusHistory
{
    public Guid Id { get; private set; }
    public Guid CaseId { get; private set; }
    public ForensicCaseStatus? FromStatus { get; private set; }
    public ForensicCaseStatus ToStatus { get; private set; }
    public string? Reason { get; private set; }
    public Guid ChangedByUserId { get; private set; }
    public DateTime ChangedAtUtc { get; private set; }
    private CaseStatusHistory() { }
}
