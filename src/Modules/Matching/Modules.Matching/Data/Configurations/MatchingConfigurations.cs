using FSH.Modules.Matching.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Modules.Matching.Data.Configurations;

public sealed class ProfileCategoryConfiguration : IEntityTypeConfiguration<ProfileCategory>
{
    public void Configure(EntityTypeBuilder<ProfileCategory> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("ProfileCategories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class MatchingRuleConfiguration : IEntityTypeConfiguration<MatchingRule>
{
    public void Configure(EntityTypeBuilder<MatchingRule> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("MatchingRules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CategoryRelated).IsRequired().HasMaxLength(128);
        builder.Property(x => x.MinimumStringency).IsRequired().HasMaxLength(32);
        builder.Property(x => x.MatchingAlgorithm).IsRequired().HasMaxLength(32);
        builder.HasIndex(x => x.SourceCategoryId);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class ProfileMatchingConfigurationConfiguration
    : IEntityTypeConfiguration<ProfileMatchingConfiguration>
{
    public void Configure(EntityTypeBuilder<ProfileMatchingConfiguration> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("ProfileConfigurations");
        builder.HasKey(x => x.GeneticProfileId);
        builder.Property(x => x.GeneticProfileId).ValueGeneratedNever();
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.Matchable);
    }
}

public sealed class AutosomalMatchSearchConfiguration : IEntityTypeConfiguration<AutosomalMatchSearch>
{
    public void Configure(EntityTypeBuilder<AutosomalMatchSearch> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("AutosomalMatchSearches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasIndex(x => x.QueryProfileId);
        builder.HasIndex(x => x.CreatedAtUtc);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class AutosomalMatchResultConfiguration : IEntityTypeConfiguration<AutosomalMatchResult>
{
    public void Configure(EntityTypeBuilder<AutosomalMatchResult> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("AutosomalMatchResults");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.RawOverall).IsRequired().HasMaxLength(32);
        builder.Property(x => x.DetailedJson).IsRequired().HasColumnType("text");
        builder.HasIndex(x => x.MatchSearchId);
        builder.HasIndex(x => x.CandidateProfileId);
    }
}

public sealed class MatchHitConfiguration : IEntityTypeConfiguration<MatchHit>
{
    public void Configure(EntityTypeBuilder<MatchHit> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("MatchHits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Status).IsRequired().HasMaxLength(32);
        builder.Property(x => x.ReviewNote).HasMaxLength(4096);
        builder.HasIndex(x => x.MatchSearchId);
        builder.HasIndex(x => x.QueryProfileId);
        builder.HasIndex(x => x.CandidateProfileId);
        builder.Ignore(x => x.DomainEvents);
    }
}
