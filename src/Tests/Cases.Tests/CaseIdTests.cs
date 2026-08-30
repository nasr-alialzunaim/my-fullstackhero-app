using FSH.Modules.Cases.Contracts;
using Shouldly;
using Xunit;

namespace Cases.Tests;

public sealed class CaseIdTests
{
    [Fact]
    public void New_Should_Create_NonEmpty_InstallationLocal_Identity()
    {
        CaseId caseId = CaseId.New();

        caseId.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_Should_Reject_Empty_Identity()
    {
        Should.Throw<ArgumentException>(() => new CaseId(Guid.Empty));
    }

    [Fact]
    public void Identity_Should_Not_Expose_Tenant_Component()
    {
        typeof(CaseId).GetProperties()
            .ShouldNotContain(property => property.Name.Contains("Tenant", StringComparison.OrdinalIgnoreCase));
    }
}
