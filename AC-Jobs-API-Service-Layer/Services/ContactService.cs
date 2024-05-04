using AC_Contact_Services.BaseService;
using AC_Jobs_API_Service_Layer.IService;
using AC_Jobs_API_Service_Layer.Models;
using AC_Jobs_API_Service_Layer.Models.Healper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace AC_Jobs_API_Service_Layer.Services
{
    public class ContactService : BaseService<SearchContactsResponseDto>, IContactService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _authApiBaseUrl;
        public string Token;

        public ContactService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                Token = authHeader.Substring("Bearer ".Length).Trim();
            }
            _apiBaseUrl = _configuration.GetSection("ServiceUrls:ContactUrl").Value;
            _authApiBaseUrl = _configuration.GetSection("ServiceUrls:IdentityAPI").Value;
        }
        public async Task<GenericResponse<IReadOnlyList<SearchContactsResponseDto>>> SearchContactsAsync(string? name)
        {
            GenericResponse<IReadOnlyList<SearchContactsResponseDto>> resp = new GenericResponse<IReadOnlyList<SearchContactsResponseDto>>();
            var baseApiUrl = _configuration.GetSection("ServiceUrls:ContactUrl").Value;
            var apiUrl = $"{baseApiUrl}/Contact/SearchContactsAsync?name={name}";
            var authToken = Token; // Replace with actual token

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("Authorization", "Bearer " + authToken);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response using JSON.NET or System.Text.Json
                var genericResponse = JsonConvert.DeserializeObject<GenericResponse<IReadOnlyList<SearchContactsResponseDto>>>(responseData);
                resp = genericResponse;
                return resp;
            }
            else
            {
                return resp;
            }

        }

        public async Task<IReadOnlyList<ContactBoardsResponseDto>> GetContactsByWorkflowIdAndStatusIdAsync(string workflowid,string workflowstatusid)
        {
            var baseApiUrl = _configuration.GetSection("ServiceUrls:ContactUrl").Value;
            var apiUrl = $"{baseApiUrl}/Contact/getContactsByWorkflowIdAndStatusId?workflowId={workflowid}&statusId={workflowstatusid}";
            var authToken = Token; // Replace with actual token

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Headers.Add("Authorization", "Bearer " + authToken);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response using JSON.NET or System.Text.Json
                return JsonConvert.DeserializeObject<IReadOnlyList<ContactBoardsResponseDto>>(responseData);
            }
            else
            {
                return new List<ContactBoardsResponseDto>();
            }

        }

        public async Task<GenericResponse<IReadOnlyList<SalesRepresentativeDropDownResponseDto>>> GetSalesRepresentativeByIdsAsync(List<long> ids)
        {
            var baseApiUrl = _configuration.GetSection("ServiceUrls:IdentityAPI").Value;

            return await this.SendAsync<GenericResponse<IReadOnlyList<SalesRepresentativeDropDownResponseDto>>>(new Models.APIResponse.ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = baseApiUrl + "SalesRepresentative/getsalesrepresentative",
                Data = ids,
                AccessToken = Token
            });
        }

        // Common
        public string GetTeamMemberNameAsync(long teamMemberId)
        {
            var body = new List<long> { teamMemberId };
            string apiUrl = $"{_apiBaseUrl}/TeamMember/getteammebersbyids";
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            request.Headers.Add("Authorization", $"Bearer {Token}");
            var client = new HttpClient();
            var response = client.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var contact = JsonConvert.DeserializeObject<dynamic>(content);
                var payload = contact?.payload;

                if (payload != null)
                {
                    return payload[0].name;
                }  
            }
            // Handle the case when payload is null or both firstName and lastName are null
            return $"Unknown Team Member ({teamMemberId})";

        }

        public async Task<string> GetStatusNameAsync(long id)
        {
            string apiUrl = $"{_apiBaseUrl}/Status/getStatusById?id={id}";
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("Authorization", $"Bearer {Token}");
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var contact = JsonConvert.DeserializeObject<dynamic>(content);
                return contact?.payload?.statusName ?? $"Unknown Status ({id})";
            }
            return $"Unknown Status ({id})";
        }

        public IEnumerable<string> GetTeamMemberNamesAsync(IEnumerable<long> teamMemberIds)
        {
            var names = new List<string>();
            foreach (var item in teamMemberIds)
            {
                var name = GetTeamMemberNameAsync((long)item);
                names.Add(name);
            }
            return names;
        }

        public async Task<GenericResponse<IReadOnlyList<StateResponse>>> GetStateAsync()
        {
            var baseApiUrl = _configuration.GetSection("ServiceUrls:IdentityAPI").Value;

            return await this.SendAsync<GenericResponse<IReadOnlyList<StateResponse>>>(new Models.APIResponse.ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = baseApiUrl + "States",
                AccessToken = Token
            });
        }
    }
    public class StateResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

}
