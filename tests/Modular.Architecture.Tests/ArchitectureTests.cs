using System.Reflection;
using NetArchTest.Rules;

namespace Modular.Architecture.Tests;

/// <summary>
/// The architecture rules of the Module Reference Map, one test per rule.
/// When you add a module, add its name to <see cref="_modules"/>.
/// </summary>
public sealed class ArchitectureTests
{
    private static readonly string[] _modules = ["Identity", "Tenancy"];
    private static readonly string[] _layers = ["Domain", "Contracts", "Application", "Infrastructure", "Endpoints"];
    private static readonly string[] _frameworks = ["Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore"];

    public static TheoryData<string> Modules => [.. _modules];

    // R1: inside a module, dependencies point inward to Domain.

    [Theory]
    [MemberData(nameof(Modules))]
    public void Domain_does_not_depend_on_other_layers(string module)
    {
        AssertNoDependency(
            Layer(module, "Domain"),
            $"Modular.{module}.Contracts",
            $"Modular.{module}.Application",
            $"Modular.{module}.Infrastructure",
            $"Modular.{module}.Endpoints");
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Application_does_not_depend_on_Infrastructure_or_Endpoints(string module)
    {
        AssertNoDependency(
            Layer(module, "Application"),
            $"Modular.{module}.Infrastructure",
            $"Modular.{module}.Endpoints");
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Endpoints_depend_only_on_Contracts(string module)
    {
        AssertNoDependency(
            Layer(module, "Endpoints"),
            $"Modular.{module}.Domain",
            $"Modular.{module}.Application",
            $"Modular.{module}.Infrastructure");
    }

    // R2: Contracts is the module's public API, so it never exposes domain types.

    [Theory]
    [MemberData(nameof(Modules))]
    public void Contracts_do_not_expose_domain_types(string module)
    {
        AssertNoDependency(
            Layer(module, "Contracts"),
            $"Modular.{module}.Domain",
            $"Modular.{module}.Application",
            "Modular.Shared.Domain");
    }

    // R3: Shared building blocks never depend on a module.

    [Fact]
    public void Shared_does_not_depend_on_any_module()
    {
        string[] modules = [.. _modules.Select(module => $"Modular.{module}.")];

        foreach (string layer in _layers)
        {
            AssertNoDependency(Layer("Shared", layer), modules);
        }
    }

    // R4: modules talk to each other only through Contracts.

    [Theory]
    [MemberData(nameof(Modules))]
    public void Module_uses_other_modules_only_through_their_Contracts(string module)
    {
        string[] otherModulesInternals = [.. _modules
            .Where(other => other != module)
            .SelectMany(other => new[]
            {
                $"Modular.{other}.Domain", $"Modular.{other}.Application",
                $"Modular.{other}.Infrastructure", $"Modular.{other}.Endpoints",
            })];

        foreach (string layer in _layers)
        {
            AssertNoDependency(Layer(module, layer), otherModulesInternals);
        }
    }

    // Domain and Contracts are plain C#: no database or web framework.

    [Theory]
    [MemberData(nameof(Modules))]
    public void Domain_and_Contracts_do_not_depend_on_frameworks(string module)
    {
        AssertNoDependency(Layer(module, "Domain"), _frameworks);
        AssertNoDependency(Layer(module, "Contracts"), _frameworks);
    }

    private static Types Layer(string module, string layer)
    {
        return Types.InAssembly(Assembly.Load($"Modular.{module}.{layer}"));
    }

    private static void AssertNoDependency(Types types, params string[] forbidden)
    {
        TestResult result = types.ShouldNot().HaveDependencyOnAny(forbidden).GetResult();

        Assert.True(
            result.IsSuccessful,
            $"These types use a forbidden namespace ({string.Join(", ", forbidden)}):{Environment.NewLine}"
            + $"  {string.Join($"{Environment.NewLine}  ", result.FailingTypeNames ?? [])}");
    }
}
