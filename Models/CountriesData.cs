using System.Text.Json.Serialization;

namespace WebAPI_GraphQL.Models
{
    public class CountriesData
    {
        [JsonPropertyName("countries")]
        public List<Country> Countries { get; set; }
    }
}
