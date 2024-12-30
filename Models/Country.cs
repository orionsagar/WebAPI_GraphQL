using System.Text.Json.Serialization;

namespace WebAPI_GraphQL.Models
{
    public class Country
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("capital")]
        public string Capital { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }
}
