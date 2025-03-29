using RestfulApiWrapper.Models;

namespace RestfulApiWrapper.Services
{
    public interface IRestfulApiService
    {
        Task<PagedResult<ApiObject>> GetObjectsAsync(string nameFilter = null, int pageNumber = 1, int pageSize = 10);
        Task<ApiObject> GetObjectByIdAsync(string id);
        Task<ApiObject> CreateObjectAsync(CreateObjectRequest request);
        Task<ApiObject> UpdateObjectAsync(string id, ApiObject updatedObject);
        Task<bool> DeleteObjectAsync(string id);
    }
}
