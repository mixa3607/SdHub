using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Fluent;

namespace Rb.Itsd.TsGenerator;

public static class InterfaceExportBuilderExtensions
{
    public static InterfaceExportBuilder SubsDecimalToNumAndBigNum(this InterfaceExportBuilder builder)
    {
        return builder
                .AddImport("BigNumber", "bignumber.js")
                .Substitute(typeof(decimal), new RtSimpleTypeName("number|BigNumber"))
                //.Substitute(typeof(decimal?), new RtSimpleTypeName("number|BigNumber|null"))
            ;
    }

    public static InterfaceExportBuilder SubsDatetimeOffsetToStr(this InterfaceExportBuilder builder)
    {
        return builder
                .Substitute(typeof(DateTimeOffset), new RtSimpleTypeName("string"))
                //.Substitute(typeof(DateTimeOffset?), new RtSimpleTypeName("string|null"))
            ;
    }

    public static InterfaceExportBuilder SubsDatetimeToStr(this InterfaceExportBuilder builder)
    {
        return builder
                .Substitute(typeof(DateTime), new RtSimpleTypeName("string"))
                //.Substitute(typeof(DateTime?), new RtSimpleTypeName("string|null"))
            ;
    }

    public static InterfaceExportBuilder SubsTimespanToStr(this InterfaceExportBuilder builder)
    {
        return builder
                .Substitute(typeof(TimeSpan), new RtSimpleTypeName("string"))
                //.Substitute(typeof(TimeSpan?), new RtSimpleTypeName("string|null"))
            ;
    }

    public static InterfaceExportBuilder SubsGuidToStr(this InterfaceExportBuilder builder)
    {
        return builder
                .Substitute(typeof(Guid), new RtSimpleTypeName("string"))
                //.Substitute(typeof(Guid?), new RtSimpleTypeName("string|null"))
            ;
    }
}
