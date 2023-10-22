using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SdHub.Options;
using SdHub.Shared.Logging;
using static OptionsSectionDocs;

public class OptionsDocsBuilder
{
    public static string ToMarkdown(IReadOnlyList<OptionsSectionDocs> docs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("|name|default|summary|");
        sb.AppendLine("|-|-|-|");
        foreach (var doc in docs)
        {
            sb.AppendLine($"|**{doc.Name}**||{doc.Summary}|");

            for (var i = 0; i < doc.Props.Count; i++)
            {
                var prop = doc.Props[i];
                var first = i == 0;
                var last = i == doc.Props.Count - 1;
                var req = prop.Required ? "*" : "";
                var ind = string.Concat(Enumerable.Repeat("&nbsp;", (prop.NestingLevel+1) * 4));
                sb.AppendLine($"{ind}{prop.Name}{req}|{prop.DefaultValue}|{prop.Summary}");
            }
        }

        return sb.ToString();
    }
    public static IReadOnlyList<OptionsSectionDocs> Generate()
    {
        var d = new List<OptionsSectionDocs>();
        var types = new[]
        {
            typeof(AppInfoOptions),
            typeof(DatabaseOptions),
            typeof(FileProcessorOptions),
            typeof(HangfireOptions),
            typeof(MailingOptions),
            typeof(RecaptchaOptions),
            typeof(SdHubSeederOptions),
            typeof(SwaggerOptions),
            typeof(WebSecurityOptions),
            typeof(SerilogOptions),
            //typeof(),
        };
        //foreach (var type in types)
        //{
        //    var name = ServiceCollectionExtensions.GetSectionName(type, "")!;
        //    var summary = type.GetXmlDocsSummary();
        //    var props = new List<PropDoc>();
        //    CollectClass(type, 0, props);
        //    var e = new OptionsSectionDocs()
        //    {
        //        Name = name,
        //        Summary = summary,
        //        Props = props
        //    };
        //    d.Add(e);
        //}

        return d;
    }

    private static void CollectClass(Type type, int ind, List<PropDoc> list)
    {
        var sOpts = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter()
            }
        };
        var instance = Activator.CreateInstance(type);
        foreach (var prop in type.GetProperties())
        {
            if (prop.GetSetMethod() == null)
                continue;

            var name = prop.Name;
            var defaultVal = prop.GetValue(instance);
            var summary = prop.GetXmlDocsSummary();
            var required = prop.GetCustomAttribute<RequiredAttribute>() != null;

            var enumItemType = prop.PropertyType.GetEnumerableItemType();
            if (enumItemType != null)
            {
                if (enumItemType.FullName!.StartsWith("SdHub") && enumItemType.IsClass)
                {
                    var defaultValStr = defaultVal != null ? "[]" : "";
                    list.Add(new PropDoc()
                    {
                        Name = name,
                        Summary = summary,
                        NestingLevel = ind,
                        DefaultValue = defaultValStr,
                        Required = required,
                    });
                    CollectClass(enumItemType, ind + 1, list);
                }
                else
                {
                    var defaultValStr = defaultVal == null ? "" : JsonConvert.SerializeObject(defaultVal, sOpts);
                    list.Add(new PropDoc()
                    {
                        Name = name,
                        Summary = summary,
                        NestingLevel = ind,
                        DefaultValue = defaultValStr,
                        Required = required,
                    });
                }
            }
            else if (prop.PropertyType.FullName!.StartsWith("SdHub") && prop.PropertyType.IsClass)
            {
                var defaultValStr = defaultVal != null ? "{}" : "";
                list.Add(new PropDoc()
                {
                    Name = name,
                    Summary = summary,
                    NestingLevel = ind,
                    DefaultValue = defaultValStr,
                    Required = required,
                });
                CollectClass(prop.PropertyType, ind + 1, list);
            }
            else
            {
                var defaultValStr = defaultVal == null ? "" : JsonConvert.SerializeObject(defaultVal, sOpts);
                list.Add(new PropDoc()
                {
                    Name = name,
                    Summary = summary,
                    NestingLevel = ind,
                    DefaultValue = defaultValStr,
                    Required = required,
                });
            }
        }
    }
}