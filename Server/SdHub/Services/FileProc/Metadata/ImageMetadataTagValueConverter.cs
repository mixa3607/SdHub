using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SdHub.Services.FileProc.Metadata;

public class ImageMetadataTagValueConverter : JsonConverter<ImageMetadataTagValue>
{
    public override void WriteJson(JsonWriter writer, ImageMetadataTagValue? value, JsonSerializer serializer)
    {
        if (value != null)
        {
            JToken.FromObject(value).WriteTo(writer);
        }
    }

    public override ImageMetadataTagValue? ReadJson(JsonReader reader, Type objectType,
        ImageMetadataTagValue? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (JToken.ReadFrom(reader) is not JObject jObj)
            throw new Exception("Cant be null");

        var typeName = (string?)jObj.GetValue(nameof(ImageMetadataTagValue.Type),
            StringComparison.InvariantCultureIgnoreCase);
        var valObjProp =
            jObj.Property(nameof(ImageMetadataTagValue.Object), StringComparison.InvariantCultureIgnoreCase);
        valObjProp?.Remove();
        var obj = jObj.ToObject<ImageMetadataTagValue>();

        if (typeName == null || obj == null || valObjProp == null)
            return obj;

        var isArray = (bool)jObj.GetValue(nameof(ImageMetadataTagValue.IsArray),
            StringComparison.InvariantCultureIgnoreCase)!;

        var type = Type.GetType(typeName);
        if (type == null)
            return obj;

        if (isArray)
            type = type.MakeArrayType();

        //only System
        obj.Object = valObjProp.Value.ToObject(type);

        return obj;
    }
}