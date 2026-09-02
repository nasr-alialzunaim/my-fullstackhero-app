using FSH.Modules.Cases;
using FSH.Modules.Cases.Contracts;
using NetArchTest.Rules;
using Shouldly;
using Xunit;

namespace Architecture.Tests;

public sealed class ForensicModuleBoundaryTests
{
    [Fact]
    public void Cases_Runtime_Should_Not_Depend_On_Other_Forensic_Runtime_Modules()
    {
        string[] forbiddenRuntimeNamespaces =
        [
            "FSH.Modules.Evidence",
            "FSH.Modules.Samples",
            "FSH.Modules.Genetics",
            "FSH.Modules.StrKits",
            "FSH.Modules.FrequencyTables",
            "FSH.Modules.ScientificAnalysis",
            "FSH.Modules.Matching",
            "FSH.Modules.Interpretation",
            "FSH.Modules.Kinship",
            "FSH.Modules.MissingPersons",
            "FSH.Modules.Dvi",
            "FSH.Modules.Reporting"
        ];

        var result = Types
            .InAssembly(typeof(CasesModule).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenRuntimeNamespaces)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"Cases runtime must use contracts instead of forensic runtime modules. " +
            $"Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Cases_Assemblies_Should_Not_Expose_TenantId()
    {
        Type[] types = typeof(CasesModule).Assembly.GetTypes()
            .Concat(typeof(CasesContractsMarker).Assembly.GetTypes())
            .ToArray();

        string[] tenantMembers = types
            .SelectMany(type => type.GetMembers()
                .Where(member => member.Name.Equals("TenantId", StringComparison.OrdinalIgnoreCase))
                .Select(member => $"{type.FullName}.{member.Name}"))
            .ToArray();

        tenantMembers.ShouldBeEmpty(
            $"Single-installation forensic modules must not expose TenantId. Found: " +
            string.Join(", ", tenantMembers));
    }

    [Fact]
    public void Cases_Contracts_Should_Not_Depend_On_Cases_Runtime()
    {
        string[] referencedAssemblies = typeof(CasesContractsMarker).Assembly
            .GetReferencedAssemblies()
            .Select(assembly => assembly.Name ?? string.Empty)
            .ToArray();

        referencedAssemblies.ShouldNotContain(
            "FSH.Modules.Cases",
            "Cases contracts must remain independent of the Cases runtime implementation.");
    }
}
