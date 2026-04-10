using System.Net.Http.Json;
using TechnicalTest.ClassLibrary.Entities.Management;
using TechnicalTest.Dtos.Entities.Management;

namespace TechnicalTest.Frontend.Services
{
    public class HotelService
    {
        private readonly HttpClient _httpClient;

        public HotelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<HotelDto>> GetHotelsAsync()
        {
            try
            {
                var hotels = await _httpClient.GetFromJsonAsync<List<HotelDto>>("http://localhost:5110/api/hotels");
                return hotels ?? new List<HotelDto>();
            }
            catch
            {
                return new List<HotelDto>();
            }
        }

        public async Task<HotelDto?> GetHotelAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<HotelDto>($"http://localhost:5110/api/hotels/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<HotelDto?> GetHotelByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<HotelDto>($"http://localhost:5110/api/hotels/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReservationDto>?> GetHotelReservationsAsync(int hotelId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<ReservationDto>>($"http://localhost:5110/api/hotels/{hotelId}/reservations");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateHotelAsync(CreateHotelRequest hotel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5110/api/hotels", hotel);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateHotelAsync(int id, UpdateHotelRequest hotel)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"http://localhost:5110/api/hotels/{id}", hotel);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteHotelAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5110/api/hotels/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    var userMessage = response.StatusCode == System.Net.HttpStatusCode.BadRequest 
                        ? $"Error de validación: {errorMessage.Replace("\"", "").Trim()}"
                        : $"Error del servidor ({response.StatusCode}): {errorMessage.Replace("\"", "").Trim()}";
                    
                    return (false, userMessage);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }
    }
}
