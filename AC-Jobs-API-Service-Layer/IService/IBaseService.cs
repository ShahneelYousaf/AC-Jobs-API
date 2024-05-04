using AC_Jobs_API_Service_Layer.Models;
using AC_Jobs_API_Service_Layer.Models.APIResponse;

namespace AC_Jobs_API_Service_Layer.IService
{
    public interface IBaseService<T> : IDisposable where T : class 
    {
        GenericResponse<T> responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
