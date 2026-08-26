using FSH.Modules.DNA;
using FSH.Modules.DNA.Contracts;
using FSH.Modules.DNA.Contracts.Authorization;
using FSH.Modules.DNA.Data;
using Shouldly;
using Xunit;

namespace Architecture.Tests;

public sealed class DnaSkeletonArchitectureTests
{
    [Fact]
    public void DnaModule_Should_Expose_Expected_Module_Assemblies()
    {
        typeof(DnaModule).Assembly.GetName().Name.ShouldBe("FSH.Modules.DNA");
        typeof(DnaContractsMarker).Assembly.GetName().Name.ShouldBe("FSH.Modules.DNA.Contracts");
    }

    [Fact]
    public void DnaDbContext_Should_Use_Dedicated_Dna_Schema()
    {
        DnaDbContext.Schema.ShouldBe("dna");
    }

    [Fact]
    public void DnaPermissions_Should_Expose_Basic_Module_View_Permission()
    {
        DnaPermissions.All.ShouldContain(permission =>
            permission.Description == "View DNA module" &&
            permission.IsBasic &&
            permission.Name == DnaPermissions.ModuleAccess.View);
    }
}
