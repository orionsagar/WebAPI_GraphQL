using System.Text.Json.Serialization;

namespace WebAPI_GraphQL.Models
{
    public class ResponseData
    {
        [JsonPropertyName("data")]
        public CountriesData Data { get; set; }
    }
}
