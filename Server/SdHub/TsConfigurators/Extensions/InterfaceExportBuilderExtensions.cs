using System;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;

namespace SdHub.TsConfigurators.Extensions;

public static class InterfaceExportBuilderExtensions
{
    public static T SubsDatetimeOffsetToStr<T>(this T builder) where T : TypeExportBuilder
    {
        return builder
                .Substitute(typeof(DateTimeOffset), new RtSimpleTypeName("string"))
                .Substitute(typeof(DateTimeOffset?), new RtSimpleTypeName("string|null"))
            ;
    }

    public static T SubsTimespanToStr<T>(this T builder) where T : TypeExportBuilder
    {
        return builder
                .Substitute(typeof(TimeSpan), new RtSimpleTypeName("string"))
                .Substitute(typeof(TimeSpan?), new RtSimpleTypeName("string|null"))
            ;
    }

    public static T SubsGuidToStr<T>(this T builder) where T : TypeExportBuilder
    {
        return builder
                .Substitute(typeof(Guid), new RtSimpleTypeName("string"))
                .Substitute(typeof(Guid?), new RtSimpleTypeName("string|null"))
            ;
    }
}
