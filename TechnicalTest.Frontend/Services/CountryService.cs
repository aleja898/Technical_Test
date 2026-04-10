using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace TechnicalTest.Frontend.Services
{
    public class CountryService
    {
        private readonly HttpClient _httpClient;

        public CountryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetCountriesAsync()
        {
            try
            {
                var countries = await _httpClient.GetFromJsonAsync<List<string>>("http://localhost:5110/api/hotels/countries");
                return countries ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener países: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
