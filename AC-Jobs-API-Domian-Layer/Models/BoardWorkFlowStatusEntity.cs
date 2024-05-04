using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class BoardWorkFlowStatusEntity : BaseEntity
    {

        [Required]
        public long WorkFlowId { get; set; }

        [Required]
        public long WorkFlowStatusId { get; set; }
        public long StatusId { get; set; }
        public string? workFlowName { get; set; }
        public string? statusName { get; set; }
        [JsonIgnore]
        public BoardStatusEntity Status { get; set; }
    }


}
