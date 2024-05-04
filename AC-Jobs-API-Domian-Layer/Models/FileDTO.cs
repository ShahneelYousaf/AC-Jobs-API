using System.ComponentModel.DataAnnotations.Schema;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class FileDTO
    {
        public string? FileName { get; set; }
        public string? ActualName { get; set; }
        public string? ContentType { get; set; }
        public string? FilePath { get; set; }
        public string? FileExtension { get; set; }
        public long? FileSize { get; set; }
        public string FileUrl { get; set; }
    }
}

