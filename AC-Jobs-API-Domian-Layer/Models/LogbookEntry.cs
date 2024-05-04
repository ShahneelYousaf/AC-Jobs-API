namespace AC_Jobs_API_Domian_Layer.Models
{
    public class LogbookEntry : BaseEntity
    {
        public string Activity { get; set; }
        public string Type { get; set; }
        public string PerformedBy { get; set; }
        public string? Comments { get; set; }
        public long JobId { get; set; }
        public Job Job { get; set; }
    }
}
