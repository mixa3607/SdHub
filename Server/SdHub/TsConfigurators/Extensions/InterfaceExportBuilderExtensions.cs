using System;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;

namespace SdHub.TsConfigurators.Extensions;

public static class InterfaceExportBuilderExtensions
{
    public static InterfaceExportBuilder SubsDatetimeOffsetToStr(this InterfaceExportBuilder builder)
    {
        return builder
                .Substitute(typeof(DateTimeOffset), new RtSimpleTypeName("string"))
                .Substitute(typeof(DateTimeOffset?), new RtSimpleTypeName("string|null"))
            ;
    }

    public static InterfaceExportBuilder SubsTimespanToStr(this InterfaceExportBuilder builder)
    {
        return builder
                .Substitute(typeof(TimeSpan), new RtSimpleTypeName("string"))
                .Substitute(typeof(TimeSpan?), new RtSimpleTypeName("string|null"))
            ;
    }

    public static InterfaceExportBuilder SubsGuidToStr(this InterfaceExportBuilder builder)
    {
        return builder
                .Substitute(typeof(Guid), new RtSimpleTypeName("string"))
                .Substitute(typeof(Guid?), new RtSimpleTypeName("string|null"))
            ;
    }
}
