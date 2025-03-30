using RestfulApiWrapper.Exceptions;
using RestfulApiWrapper.Models;
using System.Net;

namespace RestfulApiWrapper.Services
{
    public class RestfulApiService : IRestfulApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RestfulApiService> _logger;

        public RestfulApiService(HttpClient httpClient, ILogger<RestfulApiService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.restful-api.dev/");
            _logger = logger;
        }

        public async Task<PagedResult<ApiObject>> GetObjectsAsync(string nameFilter = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync("objects");
                response.EnsureSuccessStatusCode();

                var objects = await response.Content.ReadFromJsonAsync<List<ApiObject>>();

                // Apply filtering
                if (!string.IsNullOrEmpty(nameFilter))
                {
                    objects = objects?
                        .Where(o => o.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Apply paging
                var pagedObjects = objects?
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PagedResult<ApiObject>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = objects?.Count ?? 0,
                    Items = pagedObjects ?? new List<ApiObject>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting objects");
                throw;
            }
        }

        public async Task<ApiObject> GetObjectByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiObject>($"objects/{id}");
                return response;

            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"Product with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting object by ID");
                throw;
            }
        }

        public async Task<ApiObject> CreateObjectAsync(CreateObjectRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("objects", request);
                response.EnsureSuccessStatusCode();
                var objects = await response.Content.ReadFromJsonAsync<ApiObject>();
                return objects;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating object");
                throw;
            }
        }

        public async Task<ApiObject> UpdateObjectAsync(string id, ApiObject updatedObject)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"objects/{id}", updatedObject);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiObject>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating object with ID {id}");
                throw;
            }
        }

        public async Task<bool> DeleteObjectAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"objects/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting object");
                throw;
            }
        }
    }
}
