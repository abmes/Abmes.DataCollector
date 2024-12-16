using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abmes.Utils.Json;

public class MemoryStreamJsonConverter : JsonConverter<MemoryStream>
{
    public override MemoryStream Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrEmpty(value))
        {
            return new MemoryStream();
        }
        else
        {
            var bytes = Convert.FromBase64String(value);
            return new MemoryStream(bytes);
        }
    }

    public override void Write(Utf8JsonWriter writer, MemoryStream value, JsonSerializerOptions options)
    {
        if (value.Length == 0)
        {
            writer.WriteStringValue(string.Empty);
        }
        else
        {
            var bytes = value.ToArray();
            var str = Convert.ToBase64String(bytes);
            writer.WriteStringValue(str);
        }
    }
}
