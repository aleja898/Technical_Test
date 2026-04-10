using System.Net.Http.Json;
using TechnicalTest.ClassLibrary.Entities.Management;
using TechnicalTest.Dtos.Entities.Management;

namespace TechnicalTest.Frontend.Services
{
    public class ReservationService
    {
        private readonly HttpClient _httpClient;

        public ReservationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ReservationDetailDto>> GetReservationsAsync()
        {
            try
            {
                var reservations = await _httpClient.GetFromJsonAsync<List<ReservationDetailDto>>("http://localhost:5110/api/reservations");
                return reservations ?? new List<ReservationDetailDto>();
            }
            catch
            {
                return new List<ReservationDetailDto>();
            }
        }

        public async Task<ReservationDto?> GetReservationByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ReservationDto>($"http://localhost:5110/api/reservations/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReservationDetailDto?> GetReservationAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ReservationDetailDto>($"http://localhost:5110/api/reservations/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateReservationAsync(CreateReservationRequest reservation)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5110/api/reservations", reservation);
                
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

        public async Task<bool> CancelReservationAsync(int id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"http://localhost:5110/api/reservations/{id}/cancel", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5110/api/reservations/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateCheckoutDateAsync(int id, DateTime checkoutDate)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"http://localhost:5110/api/reservations/{id}/checkout-date", checkoutDate);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateReservationAsync(int id, CreateReservationRequest reservation)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"http://localhost:5110/api/reservations/{id}", reservation);
                
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
