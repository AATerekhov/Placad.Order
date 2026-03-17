using System.Text.Json;

namespace OrderService.Infrastructure.Serialization
{
    public static class JsonOptionsProvider
    {
        public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };
    }
}
