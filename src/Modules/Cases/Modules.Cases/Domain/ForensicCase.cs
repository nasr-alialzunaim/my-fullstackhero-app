using FSH.Framework.Core.Domain;

namespace FSH.Modules.Cases.Domain;

public enum ForensicCaseStatus
{
    None = 0,
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
        return new ForensicCase
        {
            Id = Guid.CreateVersion7(),
            Number = number.Trim(),
            Title = title.Trim(),
            Description = NormalizeOptional(description),
            Status = ForensicCaseStatus.Open,
            OpenedAtUtc = now,
            CreatedAtUtc = now,
        };
    }

    public void Update(string number, string title, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Number = number.Trim();
        Title = title.Trim();
        Description = NormalizeOptional(description);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ConfigureMetadata(
        string? caseType,
        string? priority,
        string? jurisdictionCode,
        DateTime? incidentAtUtc)
    {
        CaseType = NormalizeOptional(caseType);
        Priority = NormalizeOptional(priority);
        JurisdictionCode = NormalizeOptional(jurisdictionCode);
        IncidentAtUtc = incidentAtUtc;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(ForensicCaseStatus status, DateTime changedAtUtc)
    {
        if (status is ForensicCaseStatus.None)
        {
            throw new ArgumentOutOfRangeException(nameof(status), "A case status must be specified.");
        }

        Status = status;
        ClosedAtUtc = status is ForensicCaseStatus.Closed or ForensicCaseStatus.Archived
            ? changedAtUtc
            : null;
        UpdatedAtUtc = changedAtUtc;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
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

    private CaseAssignment()
    {
    }

    public static CaseAssignment Create(
        Guid caseId,
        Guid userId,
        string assignmentRole,
        Guid assignedByUserId,
        DateTime assignedAtUtc)
    {
        if (caseId == Guid.Empty)
        {
            throw new ArgumentException("Case identity cannot be empty.", nameof(caseId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("Assigned user identity cannot be empty.", nameof(userId));
        }

        if (assignedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Assigning user identity cannot be empty.", nameof(assignedByUserId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(assignmentRole);

        return new CaseAssignment
        {
            Id = Guid.CreateVersion7(),
            CaseId = caseId,
            UserId = userId,
            AssignmentRole = assignmentRole.Trim(),
            AssignedByUserId = assignedByUserId,
            AssignedAtUtc = assignedAtUtc,
        };
    }

    public void Release(DateTime releasedAtUtc)
    {
        if (releasedAtUtc < AssignedAtUtc)
        {
            throw new ArgumentOutOfRangeException(
                nameof(releasedAtUtc),
                "Release time cannot precede assignment time.");
        }

        ReleasedAtUtc = releasedAtUtc;
    }
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

    private CaseStatusHistory()
    {
    }

    public static CaseStatusHistory Create(
        Guid caseId,
        ForensicCaseStatus? fromStatus,
        ForensicCaseStatus toStatus,
        string? reason,
        Guid changedByUserId,
        DateTime changedAtUtc)
    {
        if (caseId == Guid.Empty)
        {
            throw new ArgumentException("Case identity cannot be empty.", nameof(caseId));
        }

        if (changedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Changing user identity cannot be empty.", nameof(changedByUserId));
        }

        if (toStatus is ForensicCaseStatus.None)
        {
            throw new ArgumentOutOfRangeException(nameof(toStatus), "A target case status must be specified.");
        }

        return new CaseStatusHistory
        {
            Id = Guid.CreateVersion7(),
            CaseId = caseId,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
            ChangedByUserId = changedByUserId,
            ChangedAtUtc = changedAtUtc,
        };
    }
}
