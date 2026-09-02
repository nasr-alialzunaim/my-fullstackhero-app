using FSH.Framework.Core.Domain;

namespace FSH.Modules.Matching.Domain;

public sealed class MatchHit : AggregateRoot<Guid>
{
    public Guid MatchSearchId { get; private set; }
    public Guid MatchResultId { get; private set; }
    public Guid QueryProfileId { get; private set; }
    public Guid CandidateProfileId { get; private set; }
    public string Status { get; private set; } = default!;
    public string? ReviewNote { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }

    private MatchHit() { }

    public static MatchHit Create(
        Guid matchSearchId,
        Guid matchResultId,
        Guid queryProfileId,
        Guid candidateProfileId)
    {
        return new MatchHit
        {
            Id = Guid.CreateVersion7(),
            MatchSearchId = matchSearchId,
            MatchResultId = matchResultId,
            QueryProfileId = queryProfileId,
            CandidateProfileId = candidateProfileId,
            Status = "PendingReview",
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public void Review(string status, string? note, Guid reviewerUserId)
    {
        Status = status;
        ReviewNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        ReviewedByUserId = reviewerUserId;
        ReviewedAtUtc = DateTime.UtcNow;
    }
}
