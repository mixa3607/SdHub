using System;
using System.Linq;
using System.Reflection;
using Rb.Itsd.TsGenerator;
using Reinforced.Typings.Fluent;

namespace SdHub;

public static class ReinforcedTypingsConfiguration
{
    public const string ModelsRoot = "../../Client16/apps/SdHub/src/app/models/autogen";

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
        var manualTypes = new Type[]
        {
        };
        
        var types = ReinforcedTypingsHelper.GetTsConfiguratorTypes(searchAssemblies).Concat(manualTypes).Distinct();

        var configurators = types
            .Select(Activator.CreateInstance)
            .Cast<ITsConfigurator>()
            .ToArray();
        Console.WriteLine($"Found {configurators.Length} configurators");
        foreach (var configurator in configurators)
        {
            configurator.Configure(builder);
        }
    }
}
