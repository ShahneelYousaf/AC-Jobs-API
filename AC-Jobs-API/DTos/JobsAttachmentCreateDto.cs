namespace AC_Jobs_API.DTos
{
    public class JobsAttachmentCreateDto
    {
        public long? Id { get; set; }
        public long JobId { get; set; }
        public string? Description { get; set; }
        public IFormFile File { get; set; }
    }
}
