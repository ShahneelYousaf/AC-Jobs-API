using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class BoardEntity : BaseEntity
    {

        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string ProjectType { get; set; }
        [Required]
        public string ProjectColor { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public List<BoardAccessUserEntity> AccessUsers { get; set; }
        [Required]
        public string CardTitle { get; set; }
        public List<BoardStatusEntity> Statuses { get; set; }
        public long? FolderId { get; set; }
    }

    public class FolderEntity : BaseEntity
    {
        public string Name { get; set; }
        public string? Color { get; set; }
        public long? ParentFolderId { get; set; }
    }

}
