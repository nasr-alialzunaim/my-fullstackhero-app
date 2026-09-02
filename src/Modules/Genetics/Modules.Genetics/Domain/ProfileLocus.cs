namespace FSH.Modules.Genetics.Domain;

public sealed class ProfileLocus
{
    public Guid Id { get; private set; }
    public Guid GeneticProfileId { get; private set; }
    public string Marker { get; private set; } = default!;

    private ProfileLocus()
    {
    }

    public static ProfileLocus Create(Guid profileId, string marker)
    {
        if (profileId == Guid.Empty)
        {
            throw new ArgumentException("Profile identity cannot be empty.", nameof(profileId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(marker);

        return new ProfileLocus
        {
            Id = Guid.CreateVersion7(),
            GeneticProfileId = profileId,
            Marker = marker.Trim(),
        };
    }
}
