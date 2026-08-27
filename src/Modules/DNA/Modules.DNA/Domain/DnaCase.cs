using FSH.Framework.Core.Domain;

namespace FSH.Modules.DNA.Domain;

public sealed class DnaCase : AggregateRoot<Guid>, ISoftDeletable
{
    public string CaseNumber { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public DnaCaseStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedOnUtc { get; private set; }
    public string? DeletedBy { get; private set; }

    private DnaCase() { }

    public static DnaCase Create(string caseNumber, string title, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caseNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new DnaCase
        {
            Id = Guid.CreateVersion7(),
            CaseNumber = caseNumber.Trim(),
            Title = title.Trim(),
            Description = description?.Trim(),
            Status = DnaCaseStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string title, string? description)
    {
        if (Status is DnaCaseStatus.Closed or DnaCaseStatus.Archived)
        {
            throw new InvalidOperationException("A closed or archived case cannot be modified.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        Title = title.Trim();
        Description = description?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Open()
    {
        if (Status != DnaCaseStatus.Draft)
        {
            throw new InvalidOperationException("Only draft cases can be opened.");
        }

        Status = DnaCaseStatus.Open;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void StartReview()
    {
        if (Status != DnaCaseStatus.Open)
        {
            throw new InvalidOperationException("Only open cases can enter review.");
        }

        Status = DnaCaseStatus.UnderReview;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Close()
    {
        if (Status is not (DnaCaseStatus.Open or DnaCaseStatus.UnderReview))
        {
            throw new InvalidOperationException("Only open or under-review cases can be closed.");
        }

        Status = DnaCaseStatus.Closed;
        ClosedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = ClosedAtUtc;
    }

    public void Archive()
    {
        if (Status != DnaCaseStatus.Closed)
        {
            throw new InvalidOperationException("Only closed cases can be archived.");
        }

        Status = DnaCaseStatus.Archived;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted) return;
        IsDeleted = false;
        DeletedOnUtc = null;
        DeletedBy = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
