namespace AC_Jobs_API.DTos
{
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

    public class DTOGetNote
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public int? JobId { get; set; }
        public int? WorkOrderId { get; set; }
        public List<DtoNotePhoto> Attachments { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DtoNotePhoto
    {
        public long Id { get; set; }
        public long? NoteId { get; set; }
        public string? FileName { get; set; }
        public string? ActualName { get; set; }
        public string? ContentType { get; set; }
        public string? FilePath { get; set; }
        public string? FileExtension { get; set; }
        public long? FileSize { get; set; }
        public string FileUrl { get; set; }
        public string? Description { get; set; }
    }

    public class DTOUpdateNote
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public int? JobId { get; set; }
        public int? WorkOrderId { get; set; }
        public List<IFormFile> Attachments { get; set; }
        public bool? IsDeleted { get; set; }
    }

}
