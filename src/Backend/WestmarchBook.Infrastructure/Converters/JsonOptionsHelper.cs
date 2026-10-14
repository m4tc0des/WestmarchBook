namespace WestmarchBook.Infrastructure.Converters;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class JsonOptionsHelper
{
    public static JsonSerializerOptions Default => new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}