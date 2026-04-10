using System.Net.Http.Json;
using TechnicalTest.ClassLibrary.Entities.Users;
using TechnicalTest.Dtos.Entities.Users;
using TechnicalTest.Dtos.Entities.Management;

namespace TechnicalTest.Frontend.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<UserDto>>("http://localhost:5110/api/users");
                return users ?? new List<UserDto>();
            }
            catch
            {
                return new List<UserDto>();
            }
        }

        public async Task<UserDto?> GetUserAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserDto>($"http://localhost:5110/api/users/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserDto>($"http://localhost:5110/api/users/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReservationDto>?> GetUserReservationsAsync(int userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<ReservationDto>>($"http://localhost:5110/api/users/{userId}/reservations");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(CreateUserRequest user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5110/api/users", user);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserRequest user)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"http://localhost:5110/api/users/{id}", user);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5110/api/users/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
