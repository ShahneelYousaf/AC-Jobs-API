using AC_Jobs_API_Service_Layer.Models;
using System.ComponentModel.DataAnnotations;

namespace AC_Jobs_API.DTos
{
    public class DTOJobs
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public long? StateId { get; set; }
        public string? FaxNo { get; set; }
        public string? MobileNo { get; set; }
        public string? HomeNo { get; set; }
        public string? OfficeNo { get; set; }
        public string? PhoneNo { get; set; }
        public string? JobType { get; set; }
        public string? StateName { get; set; }
        public SalesRepresentativeDropDownResponseDto? SalesReps { get; set; }
        public List<DetailDto>? TeamMembers { get; set; } = new List<DetailDto> { };

        public string? Note { get; set; }
        public long? Zip { get; set; }
        public long JobStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public long LeadSourceId { get; set; }
        public long SalesRepsentativeId { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
        public long? PrimaryContactId { get; set; }
        public long? OfficeLocationId { get; set; }
        public long? WorkFlowId { get; set; }
        public long SubContractorId { get; set; }
        public List<long>? RelatedContactId { get; set; }
        public List<long>? TeamMememberId { get; set; }
    }
    public class WorkOrderBoardDTO
    {
        public long Id { get; set; }
        public string WorkOrderPriority { get; set; }
        public string Name { get; set; }
        public long? WorkflowId { get; set; }
        public long? WorkOrderStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }
        public string? StateName { get; set; }
        public long? Zip { get; set; }
        public SalesRepresentativeDropDownResponseDto? SalesReps { get; set; }
        public List<DetailDto>? TeamMembers { get; set; } = new List<DetailDto> { };


        public DateTime? LastStatusChangeDate { get; set; }
        public int? ContactId { get; set; }
        public int? JobId { get; set; }
        public long? SalesRepsentativeId { get; set; }
        public long? OfficeLocationId { get; set; }
        public long? SubContractorId { get; set; }
        public List<long>? TeamMememberId { get; set; }
        public long? PrimaryContactId { get; set; }
        public List<long>? RelatedContactId { get; set; }
    }
    public class StateResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
