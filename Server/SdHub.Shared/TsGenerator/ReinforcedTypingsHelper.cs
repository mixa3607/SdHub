using System.Reflection;

namespace Rb.Itsd.TsGenerator;

public static class ReinforcedTypingsHelper
{
    public static IEnumerable<Type> GetTsConfiguratorTypes(params Assembly[] assemblies)
    {
        var type = typeof(ITsConfigurator);
        var types = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
        return types;
    }
}