using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WebAPI_GraphQL.Models;
using WebAPI_GraphQL.DAL;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;

namespace WebAPI_GraphQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private static readonly HttpClient HttpClient = new HttpClient();


        /// <summary>
        /// Fetch Data from GraphQL API
        /// </summary>
        /// <returns></returns>
        [HttpGet("fetchcountries")]
        public async Task<IActionResult> FetchCountries()
        {
            try
            {
                // Fetch data from GraphQL API
                var countries = await FetchCountriesFromGraphQL();

                if (countries == null || !countries.Any())
                    return NotFound("No countries found.");

                // Select one country to post
                var selectedCountry = countries.First();

                // Post one country details to REST API
                var response = await PostCountryDetailsAsync(selectedCountry);

                if (response == null)
                    return StatusCode(500, "Failed to post country details.");

                // Save data to CSV
                SaveCountriesToCsv(countries);

                // Provide a download link for the CSV file
                //var filePath = "countries.csv";
                //var fileBytes = System.IO.File.ReadAllBytes(filePath);
                //return File(fileBytes, "text/csv", "countries.csv");

                return Ok(countries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        private static async Task<List<Country>> FetchCountriesFromGraphQL()
        {
            var graphqlQuery = new
            {
                query = "query { countries { name capital currency } }"
            };

            try
            {
                var requestContent = new StringContent(JsonSerializer.Serialize(graphqlQuery), Encoding.UTF8, "application/json");
                var response = await HttpClient.PostAsync(AppConfig.GraphQLAPI_Link, requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching countries: {response.StatusCode}");
                    return null;
                }

                //response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var graphqlResponse = JsonSerializer.Deserialize<ResponseData>(jsonResponse);

                return graphqlResponse?.Data.Countries ?? new List<Country>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }


        }


        /// <summary>
        /// Sava Data into CSV Files
        /// </summary>
        /// <param name="countries"></param>
        private static void SaveCountriesToCsv(List<Country> countries)
        {
            const string filePath = "countries.csv";

            try
            {
                using var writer = new StreamWriter(filePath);

                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                // Header Name
                csv.WriteField("Country Name");
                csv.WriteField("Capital");
                csv.WriteField("Currency");
                csv.NextRecord();

                // Write rows
                foreach (var country in countries)
                {
                    csv.WriteField(country.Name);
                    csv.WriteField(country.Capital);
                    csv.WriteField(country.Currency);
                    csv.NextRecord();
                }

                //csv.WriteRecords(countries);

                Console.WriteLine("Countries saved to CSV successfully.");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error saving countries to CSV: {ex.Message}");
            }
            
        }




        /// <summary>
        /// Post Single Country 
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        [HttpPost("postcountry")]
        public async Task<IActionResult> PostCountry([FromBody] Country country)
        {
            try
            {
                await PostCountryDetailsAsync(country);
                return Ok(new { message = "Country posted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        private async Task<int?> PostCountryDetailsAsync(Country country)
        {
            var postData = new
            {
                title = $"Country: {country.Name}",
                body = $"Capital: {country.Capital}, Currency: {country.Currency}",
                userId = 1
            };

            try
            {
                var requestContent = new StringContent(JsonSerializer.Serialize(postData), Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                int retryCount = 0;

                do
                {
                    response = await HttpClient.PostAsync(AppConfig.CountryPost_Link + "posts", requestContent);

                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        Console.WriteLine("403 Forbidden error. Skipping request.");
                        return null;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        Console.WriteLine("500 Internal Server Error. Retrying...");
                        await Task.Delay((int)Math.Pow(2, retryCount) * 1000);
                        retryCount++;
                    }
                    else
                    {
                        break;
                    }
                } while (retryCount < 3);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to post country details: {response.StatusCode}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var postResponse = JsonSerializer.Deserialize<PostResponse>(responseContent);

                return postResponse?.Id;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error posting country details: {ex.Message}");
                return null;
            }
        }
    }
}
