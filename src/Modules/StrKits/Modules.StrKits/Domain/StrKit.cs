using FSH.Framework.Core.Domain;

namespace FSH.Modules.StrKits.Domain;

public sealed class StrKit : AggregateRoot<Guid>
{
    public string KitCode { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public int AnalysisTypeId { get; private set; }
    public int RepresentativeParameter { get; private set; }
    public int VersionNumber { get; private set; }
    public Guid? SupersedesKitId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private StrKit() { }

    public static StrKit Create(
        string kitCode,
        string name,
        int analysisTypeId,
        int representativeParameter,
        int versionNumber,
        Guid? supersedesKitId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(kitCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new StrKit
        {
            Id = Guid.CreateVersion7(),
            KitCode = kitCode.Trim(),
            Name = name.Trim(),
            AnalysisTypeId = analysisTypeId,
            RepresentativeParameter = representativeParameter,
            VersionNumber = versionNumber,
            SupersedesKitId = supersedesKitId,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
