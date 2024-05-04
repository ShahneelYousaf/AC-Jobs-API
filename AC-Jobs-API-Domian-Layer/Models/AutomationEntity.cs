using System.Text.Json.Serialization;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class AutomationEntity : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string TriggerType { get; set; }
        public string? TriggerRecord { get; set; }
        public string? WhenEntityIs { get; set; }
        public int? Duration { get; set; }
        public string? TimeUnit { get; set; }
        public string? BeforeAfter { get; set; }
        public bool? IsSpecificDay { get; set; }
        public string? SelectedDay { get; set; }
        public bool? IsSpecificTime { get; set; }
        public string? SelectedTime { get; set; }
        public string? AutomationTriggerDateField { get; set; }
        public bool RequireAllConditionsToBeTrue { get; set; } = false;
        public DateTime? LastExecutionTime { get; set; }
        public DateTime? LastSuccessfulExecutionTime { get; set; }
        public DateTime? NextExecutionTime { get; set; }

        // Navigation properties
        public List<ConditionEntity> Conditions { get; set; }
        public List<ActionEntity> Actions { get; set; }
    }

    public class ConditionEntity : BaseEntity
    {
        public string Field { get; set; }
        public string Value { get; set; }
        public string Comparison { get; set; }
        public bool OnlyIfModified { get; set; } = false;

        // Foreign key property
        public long AutomationEntityId { get; set; }

        [JsonIgnore]
        public AutomationEntity AutomationEntity { get; set; }
    }

    public class ActionEntity : BaseEntity
    {
        public string ActionType { get; set; }
        public string Name { get; set; }
        public string ActionObj { get; set; }

        // Foreign key property
        public long AutomationEntityId { get; set; }

        [JsonIgnore]
        public AutomationEntity AutomationEntity { get; set; }
    }


    public class SMSTemplateEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
    public class EmailTemplateEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class TemplateDto<T> where T : class
    {
        public T Template { get; set; }
        public List<string> Recipients { get; set; }
    }
    public class WebHookDTO
    {
        public string TargetedUrl { get; set; }
    }

}
