using FSH.Modules.DNA;
using FSH.Modules.DNA.Contracts;
using FSH.Modules.DNA.Contracts.Authorization;
using FSH.Modules.DNA.Data;
using FSH.Modules.DNA.Domain;
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
    public void Case_Should_Start_As_Draft_And_Open_When_Requested()
    {
        var entity = DnaCase.Create("DNA-0001", "Initial case");

        entity.Status.ShouldBe(DnaCaseStatus.Draft);
        entity.Open();
        entity.Status.ShouldBe(DnaCaseStatus.Open);
    }

    [Fact]
    public void Case_Should_Reject_Changes_After_Closing()
    {
        var entity = DnaCase.Create("DNA-0002", "Initial case");
        entity.Open();
        entity.Close();

        Should.Throw<InvalidOperationException>(() => entity.UpdateDetails("Changed", null));
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
