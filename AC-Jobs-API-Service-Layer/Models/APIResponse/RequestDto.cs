

using static AC_Jobs_API_Service_Layer.Models.Healper.SD;

namespace AC_Jobs_API_Service_Layer.Models.APIResponse
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
    }
}
