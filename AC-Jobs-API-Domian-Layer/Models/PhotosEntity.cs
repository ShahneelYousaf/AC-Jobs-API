namespace AC_Jobs_API_Domian_Layer.Models
{
    public class PhotosEntity : BaseEntity
    {
        public long? JobId { get; set; }     
        public long? WorkOrderId { get; set; }     
        public long? NoteId { get; set; }    
        public string? FileName { get; set; }
        public string? ActualName { get; set; }
        public string? ContentType { get; set; }
        public string? FilePath { get; set; }
        public string? FileExtension { get; set; }
        public long? FileSize { get; set; }
        public string FileUrl { get; set; }
        public string? Description { get; set; }
        public string? UploadedBy { get; set; }
    }
}

