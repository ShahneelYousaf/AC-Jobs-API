using AC_Jobs_API_Domian_Layer.Models;

namespace AC_Jobs_API.DTos
{
    public class DTOCreateFolder
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public long? ParentFolderId { get; set; }
    }

    public class DTOContactEventGeneration
    {
        public long Id { get; set; }
        public string EventType { get; set; }
    }

    public class AutomationDto
    {
        public long Id { get; set; } = 0;
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
        public List<ConditionDto> Conditions { get; set; }
        public List<ActionDto> Actions { get; set; }
    }

    public class ConditionDto
    {
        public long Id { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public string Comparison { get; set; }
        public bool OnlyIfModified { get; set; } = false;
        public long AutomationEntityId { get; set; } = 0;
    }

    public class ActionDto
    {
        public long Id { get; set; }
        public string ActionType { get; set; }
        public string Name { get; set; }
        public object ActionObj { get; set; }
        public long AutomationEntityId { get; set; } = 0;
    }


}
