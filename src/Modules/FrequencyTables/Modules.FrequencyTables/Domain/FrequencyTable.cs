using FSH.Framework.Core.Domain;

namespace FSH.Modules.FrequencyTables.Domain;

public sealed class FrequencyTable : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public double Theta { get; private set; }
    public int VersionNumber { get; private set; }
    public Guid? SupersedesTableId { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private FrequencyTable() { }

    public static FrequencyTable Create(
        string name,
        string model,
        double theta,
        int versionNumber,
        Guid? supersedesTableId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        return new FrequencyTable
        {
            Id = Guid.CreateVersion7(),
            Name = name.Trim(),
            Model = model.Trim(),
            Theta = theta,
            VersionNumber = versionNumber,
            SupersedesTableId = supersedesTableId,
            IsActive = true,
            IsDefault = false,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public void SetDefault(bool value) => IsDefault = value;
    public void ToggleActive()
    {
        IsActive = !IsActive;
        if (!IsActive)
        {
            IsDefault = false;
        }
    }
}
