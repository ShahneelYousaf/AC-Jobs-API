namespace AC_Jobs_API_Domian_Layer.Models
{
    public class Attachments: BaseEntity
    {
        public string FilePath { get; set; }
        public long? NoteId { get; set; }
        public long? JobId { get; set; }
        public long? WorkOrderId { get; set; }
        public string? UploadedBy { get; set; }

    }
}