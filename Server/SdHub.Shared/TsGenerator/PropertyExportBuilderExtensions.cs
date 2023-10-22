using Reinforced.Typings.Fluent;

namespace Rb.Itsd.TsGenerator;

public static class PropertyExportBuilderExtensions
{
    public static PropertyExportBuilder ExtendNullableDetection(this PropertyExportBuilder builder)
    {
        var isNullable = IsNullableChecker.IsNullable(builder.Member);
        if (isNullable)
            builder.ForceNullable();
        return builder;
    }
}
