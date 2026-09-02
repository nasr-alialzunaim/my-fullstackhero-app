namespace FSH.Modules.Matching.Domain;

public sealed class ProfileMatchingConfiguration
{
    public Guid GeneticProfileId { get; private set; }
    public Guid CategoryId { get; private set; }
    public bool Matchable { get; private set; }
    public Guid? VictimProfileId { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private ProfileMatchingConfiguration() { }

    public static ProfileMatchingConfiguration Create(
        Guid geneticProfileId,
        Guid categoryId,
        bool matchable,
        Guid? victimProfileId)
    {
        return new ProfileMatchingConfiguration
        {
            GeneticProfileId = geneticProfileId,
            CategoryId = categoryId,
            Matchable = matchable,
            VictimProfileId = victimProfileId,
            UpdatedAtUtc = DateTime.UtcNow,
        };
    }

    public void Update(Guid categoryId, bool matchable, Guid? victimProfileId)
    {
        CategoryId = categoryId;
        Matchable = matchable;
        VictimProfileId = victimProfileId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
