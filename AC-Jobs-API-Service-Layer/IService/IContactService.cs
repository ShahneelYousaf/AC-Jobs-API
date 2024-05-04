using AC_Jobs_API_Service_Layer.Models;
using AC_Jobs_API_Service_Layer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Service_Layer.IService
{
    public interface IContactService
    {
        Task<string> GetStatusNameAsync(long id);
        string GetTeamMemberNameAsync(long teamMemberId);
        IEnumerable<string> GetTeamMemberNamesAsync(IEnumerable<long> teamMemberIds);
        Task<GenericResponse<IReadOnlyList<StateResponse>>> GetStateAsync();
        Task<GenericResponse<IReadOnlyList<SearchContactsResponseDto>>> SearchContactsAsync(string? name);
        Task<IReadOnlyList<ContactBoardsResponseDto>> GetContactsByWorkflowIdAndStatusIdAsync(string workflowid, string workflowstatusid);
        Task<GenericResponse<IReadOnlyList<SalesRepresentativeDropDownResponseDto>>> GetSalesRepresentativeByIdsAsync(List<long> ids);
    }
}
