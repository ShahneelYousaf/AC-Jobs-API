using System.ComponentModel.DataAnnotations.Schema;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    [Table("WorkFlows")]
    public class ContactsWorkFlowEntity : ContactBaseEntity
    {
        public string WorkFlowName { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsAccessable { get; set; }
        public WorkFlowType WorkFlowType { get; set; }
    }

    [Table("Tasks")]
    public class TaskEntity : ContactBaseEntity
    {
        public string TaskName { get; set; }
        public string TaskType { get; set; }
        public string Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? EstimatedDuration { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public string? EstimatedType { get; set; }
    }

    [Table("TaskRelatedContacts")]
    public class TaskContactEntity : ContactBaseEntity
    {
        public long TaskId { get; set; }
        public long ContactId { get; set; }
    }

    [Table("TaskJobs")]
    public class TaskJobEntity : ContactBaseEntity
    {
        public long TaskId { get; set; }
        public long JobId { get; set; }
    }

    [Table("TaskSubContractors")]
    public class TaskSubContractorEntity : ContactBaseEntity
    {
        public long TaskId { get; set; }
        public long SubContractorId { get; set; }
    }

    [Table("TaskTeamMembers")]
    public class TaskTeamMemberEntity : ContactBaseEntity
    {
        public long TaskId { get; set; }
        public long TeamMemberId { get; set; }
    }
}
