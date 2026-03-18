using System.Text.Json;

namespace Orders.Infrastructure.Serialization;

public static class JsonOptionsProvider
{
    public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };
}
