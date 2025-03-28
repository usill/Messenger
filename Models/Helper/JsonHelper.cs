using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestSignalR.Models.Helper
{
    public static class JsonHelper
    {
        public static string Serialize<T>(T obj, int depth = 2)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                MaxDepth = depth,
                WriteIndented = true,
            };

            return JsonSerializer.Serialize(obj, jsonOptions);
        }
    }
}
