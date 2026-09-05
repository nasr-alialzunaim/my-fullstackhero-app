using FSH.Framework.Core.Domain;

namespace FSH.Modules.Subjects.Domain;

public enum SubjectType
{
    Person = 1,
    MissingPerson = 2,
    UnidentifiedRemains = 3,
    UnknownPerson = 4,
}

public enum SubjectStatus
{
    Active = 1,
    Inactive = 2,
    Archived = 3,
}

public sealed class Subject : AggregateRoot<Guid>
{
    public string SubjectCode { get; private set; } = default!;
    public SubjectType SubjectType { get; private set; }
    public SubjectStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private Subject()
    {
    }

    public static Subject Create(string subjectCode, SubjectType subjectType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subjectCode);
        return new Subject
        {
            Id = Guid.CreateVersion7(),
            SubjectCode = subjectCode.Trim(),
            SubjectType = subjectType,
            Status = SubjectStatus.Active,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public void SetStatus(SubjectStatus status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

public sealed class PersonIdentity
{
    public Guid SubjectId { get; private set; }
    public string? NationalIdProtected { get; private set; }
    public string? NationalIdHash { get; private set; }
    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? Sex { get; private set; }
    public string? NationalityCode { get; private set; }
    public bool IdentityVerified { get; private set; }
    public Guid? VerifiedByUserId { get; private set; }
    public DateTime? VerifiedAtUtc { get; private set; }

    private PersonIdentity()
    {
    }

    public static PersonIdentity Create(
        Guid subjectId,
        string? nationalIdProtected,
        string? nationalIdHash,
        string? firstName,
        string? middleName,
        string? lastName,
        DateOnly? dateOfBirth,
        string? sex,
        string? nationalityCode,
        bool identityVerified,
        Guid? verifiedByUserId)
    {
        if (subjectId == Guid.Empty)
        {
            throw new ArgumentException("Subject identity cannot be empty.", nameof(subjectId));
        }

        PersonIdentity entity = new() { SubjectId = subjectId };
        entity.Update(nationalIdProtected, nationalIdHash, firstName, middleName, lastName, dateOfBirth, sex, nationalityCode, identityVerified, verifiedByUserId);
        return entity;
    }

    public void Update(
        string? nationalIdProtected,
        string? nationalIdHash,
        string? firstName,
        string? middleName,
        string? lastName,
        DateOnly? dateOfBirth,
        string? sex,
        string? nationalityCode,
        bool identityVerified,
        Guid? verifiedByUserId)
    {
        NationalIdProtected = Normalize(nationalIdProtected);
        NationalIdHash = Normalize(nationalIdHash);
        FirstName = Normalize(firstName);
        MiddleName = Normalize(middleName);
        LastName = Normalize(lastName);
        DateOfBirth = dateOfBirth;
        Sex = Normalize(sex);
        NationalityCode = Normalize(nationalityCode)?.ToUpperInvariant();
        IdentityVerified = identityVerified;
        VerifiedByUserId = identityVerified ? verifiedByUserId : null;
        VerifiedAtUtc = identityVerified ? DateTime.UtcNow : null;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class SubjectAlias
{
    public Guid Id { get; private set; }
    public Guid SubjectId { get; private set; }
    public string AliasType { get; private set; } = default!;
    public string AliasValue { get; private set; } = default!;
    public DateTime CreatedAtUtc { get; private set; }

    private SubjectAlias()
    {
    }

    public static SubjectAlias Create(Guid subjectId, string aliasType, string aliasValue)
    {
        if (subjectId == Guid.Empty) throw new ArgumentException("Subject identity cannot be empty.", nameof(subjectId));
        ArgumentException.ThrowIfNullOrWhiteSpace(aliasType);
        ArgumentException.ThrowIfNullOrWhiteSpace(aliasValue);
        return new SubjectAlias { Id = Guid.CreateVersion7(), SubjectId = subjectId, AliasType = aliasType.Trim(), AliasValue = aliasValue.Trim(), CreatedAtUtc = DateTime.UtcNow };
    }
}

public sealed class SubjectExternalIdentifier
{
    public Guid Id { get; private set; }
    public Guid SubjectId { get; private set; }
    public string IdentifierType { get; private set; } = default!;
    public string ValueProtected { get; private set; } = default!;
    public string ValueHash { get; private set; } = default!;
    public string? Issuer { get; private set; }
    public bool IsPrimary { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private SubjectExternalIdentifier()
    {
    }

    public static SubjectExternalIdentifier Create(Guid subjectId, string identifierType, string valueProtected, string valueHash, string? issuer, bool isPrimary)
    {
        if (subjectId == Guid.Empty) throw new ArgumentException("Subject identity cannot be empty.", nameof(subjectId));
        ArgumentException.ThrowIfNullOrWhiteSpace(identifierType);
        ArgumentException.ThrowIfNullOrWhiteSpace(valueProtected);
        ArgumentException.ThrowIfNullOrWhiteSpace(valueHash);
        return new SubjectExternalIdentifier { Id = Guid.CreateVersion7(), SubjectId = subjectId, IdentifierType = identifierType.Trim(), ValueProtected = valueProtected.Trim(), ValueHash = valueHash.Trim(), Issuer = string.IsNullOrWhiteSpace(issuer) ? null : issuer.Trim(), IsPrimary = isPrimary, CreatedAtUtc = DateTime.UtcNow };
    }
}

public sealed class SubjectLegalReference
{
    public Guid Id { get; private set; }
    public Guid SubjectId { get; private set; }
    public string ReferenceType { get; private set; } = default!;
    public string? ReferenceNumber { get; private set; }
    public string? Authority { get; private set; }
    public DateTime? IssuedAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public string? Description { get; private set; }
    public Guid? FileAssetId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private SubjectLegalReference()
    {
    }

    public static SubjectLegalReference Create(Guid subjectId, string referenceType, string? referenceNumber, string? authority, DateTime? issuedAtUtc, DateTime? expiresAtUtc, string? description, Guid? fileAssetId, Guid createdByUserId)
    {
        if (subjectId == Guid.Empty) throw new ArgumentException("Subject identity cannot be empty.", nameof(subjectId));
        if (createdByUserId == Guid.Empty) throw new ArgumentException("Creator identity cannot be empty.", nameof(createdByUserId));
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceType);
        if (expiresAtUtc.HasValue && issuedAtUtc.HasValue && expiresAtUtc < issuedAtUtc) throw new ArgumentException("Legal reference expiry cannot precede issue date.", nameof(expiresAtUtc));
        return new SubjectLegalReference { Id = Guid.CreateVersion7(), SubjectId = subjectId, ReferenceType = referenceType.Trim(), ReferenceNumber = Normalize(referenceNumber), Authority = Normalize(authority), IssuedAtUtc = issuedAtUtc, ExpiresAtUtc = expiresAtUtc, Description = Normalize(description), FileAssetId = fileAssetId, CreatedByUserId = createdByUserId, CreatedAtUtc = DateTime.UtcNow };
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
