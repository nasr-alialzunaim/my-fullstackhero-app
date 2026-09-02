using FSH.Framework.Core.Domain;

namespace FSH.Modules.Genetics.Domain;

public sealed class GeneticProfile : AggregateRoot<Guid>
{
    public Guid SampleId { get; private set; }
    public string? ExternalProfileCode { get; private set; }
    public Guid? StrKitId { get; private set; }
    public int? Contributors { get; private set; }
    public int VersionNumber { get; private set; }
    public Guid? SupersedesProfileId { get; private set; }
    public int? AnalysisTypeId { get; private set; }
    public bool IsReference { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private GeneticProfile()
    {
    }

    public static GeneticProfile Create(
        Guid sampleId,
        string? externalProfileCode,
        Guid? strKitId,
        int? contributors,
        int versionNumber,
        Guid? supersedesProfileId,
        int? analysisTypeId,
        bool isReference)
    {
        if (sampleId == Guid.Empty)
        {
            throw new ArgumentException("Sample identity cannot be empty.", nameof(sampleId));
        }

        if (versionNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(versionNumber),
                "Profile version must be at least 1.");
        }

        return new GeneticProfile
        {
            Id = Guid.CreateVersion7(),
            SampleId = sampleId,
            ExternalProfileCode = Normalize(externalProfileCode),
            StrKitId = strKitId,
            Contributors = contributors,
            VersionNumber = versionNumber,
            SupersedesProfileId = supersedesProfileId,
            AnalysisTypeId = analysisTypeId,
            IsReference = isReference,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}