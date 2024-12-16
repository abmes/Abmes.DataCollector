using System.Text.Json;
using System.Text.Json.Serialization;

namespace Abmes.Utils.Json;

public class BoolJsonConverter : JsonConverter<bool>
{
    private static readonly string[] _trueValues = ["true", "yes", "1"];
    private static readonly string[] _falseValues = ["false", "no", "0"];

    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (_trueValues.Contains(value, StringComparer.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (_falseValues.Contains(value, StringComparer.InvariantCultureIgnoreCase))
        {
            return false;
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        if (value)
        {
            writer.WriteStringValue("true");
        }
        else
        {
            writer.WriteStringValue("false");
        }
    }
}
