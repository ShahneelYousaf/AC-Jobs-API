namespace AC_Jobs_API.DTos
{
    public class DTOCreateNote
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public int? JobId { get; set; }
        public int? WorkOrderId { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
