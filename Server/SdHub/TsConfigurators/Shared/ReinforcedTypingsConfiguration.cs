using System;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Fluent;

namespace SdHub.TsConfigurators.Shared;

public static class ReinforcedTypingsConfiguration
{
    public const string BigNumberNumberNullPropertyName = "BigNumber|number|null";
    public const string BigNumberNullPropertyName = "BigNumber|null";

    public static void Configure(ConfigurationBuilder builder)
    {
        builder.Global(x => x
            .UseModules()
            .CamelCaseForProperties()
            .CamelCaseForMethods()
            .GenerateDocumentation()
        );

        var searchAssemblies = new Assembly[]
        {
            typeof(Program).Assembly
        };
        var manualTypes = new ITsConfigurator[]
        {
        };

        var type = typeof(ITsConfigurator);
        var types = searchAssemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
            .Where(x => manualTypes.All(y => y.GetType() != x));

        var configurators = types
            .Select(Activator.CreateInstance)
            .Cast<ITsConfigurator>()
            .Concat(manualTypes).ToArray();
        Console.WriteLine($"Found {configurators.Length} configurators");
        foreach (var configurator in configurators)
        {
            configurator.Configure(builder);
        }
    }
}