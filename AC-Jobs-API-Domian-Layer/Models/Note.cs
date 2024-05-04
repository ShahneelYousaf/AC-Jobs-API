namespace AC_Jobs_API_Domian_Layer.Models
{
    public class Note : BaseEntity
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public int? JobId { get; set; }
        public int? WorkOrderId { get; set; }
    }
}