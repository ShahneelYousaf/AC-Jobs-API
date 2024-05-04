using System.ComponentModel.DataAnnotations;

namespace AC_Jobs_API_Service_Layer.Models
{
    public class DTOCreateWorkOrder
    {
        public string Name { get; set; }
        public long? WorkflowId { get; set; }
        public long? WorkOrderStatus { get; set; }
        public List<long> TeamMemberId { get; set; }
        public List<long> SubContractorId { get; set; }
        public string WorkOrderPriority { get; set; } 
        public string TimeUnit { get; set; } 
        public string Duration { get; set; } 
        public int? JobId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<LineItems> LineItems { get; set; }
        public List<DTONote> Notes { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
        //----------------Relational Fields-------------------------
        public int? ContactId { get; set; }
    }
    public class DTOCreateAutomationWorkOrder
    {
        public string Name { get; set; }
        public long? WorkflowId { get; set; }
        public long? WorkOrderStatus { get; set; }
        public List<string> TeamMemberId { get; set; }
        public List<string> SubContractorId { get; set; }
        public string WorkOrderPriority { get; set; }
        public string TimeUnit { get; set; }
        public string Duration { get; set; }
        public int? JobId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public List<LineItems>? LineItems { get; set; }
        public List<DTONote>? Notes { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
        //----------------Relational Fields-------------------------
        public int? ContactId { get; set; }
    }

    public class DTONote
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public int? JobId { get; set; }
        public int? WorkOrderId { get; set; }
        public List<DTOAttachment> Attachments { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DTOAttachment
    {
        public long Id { get; set; }
        public string FilePath { get; set; }
    }

    public class CreateAutomationTaskDto
    {
        [Required]
        public string TaskName { get; set; }
        public long? Id { get; set; }
        public string TaskType { get; set; }
        public string Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string>? RelatedSubcontractors { get; set; }
        public List<string>? RelatedContacts { get; set; }
        public List<string>? RelatedJobs { get; set; }
        public List<string>? AssignedTeamMembers { get; set; }
        public string? EstimatedDuration { get; set; }
        public string TimeUnit { get; set; }
        public string Duration { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public string? EstimatedType { get; set; }
    }

    public class CreateTaskDto
    {
        [Required]
        public string TaskName { get; set; }
        public string TaskType { get; set; }
        public string Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<long>? RelatedSubcontractors { get; set; }
        public List<long>? RelatedContacts { get; set; }
        public List<long>? RelatedJobs { get; set; }
        public List<long>? AssignedTeamMembers { get; set; }
        public string? EstimatedDuration { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public string? EstimatedType { get; set; }
    }


}
