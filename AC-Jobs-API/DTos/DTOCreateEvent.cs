using System.ComponentModel.DataAnnotations;

namespace AC_Jobs_API.DTos
{
    public class DTOCreateEvent
    {
        public string EventType { get; set; }
        public string EventPriority { get; set; }
        [MaxLength(50)]
        public string EventName { get; set; }
        public string EventStatus { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(50)]
        public string EstimatedDuration { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        [MaxLength(200)]
        public string Tags { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
        public long? JobId { get; set; }
    }
}
