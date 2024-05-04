using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class BoardStatusEntity : BaseEntity
    {

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string SortBy { get; set; }

        [MaxLength(255)]
        public string SortingOrder { get; set; }

        public string Total { get; set; }

        public List<BoardWorkFlowStatusEntity> WorkFlowStatuses { get; set; }
        public long BoardId { get; set; }

        [JsonIgnore]
        public BoardEntity Board { get; set; }
    }


}
