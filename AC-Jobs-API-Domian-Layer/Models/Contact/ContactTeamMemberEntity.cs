using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    [Table("TeamMembers")]
    public class ContactTeamMemberEntity : ContactBaseEntity
    {
        [Required]
        public long TeamMemberId { get; set; }
        [Required]
        public long ContactId { get; set; }
        public ContactEntity Contact { get; set; }
    }
}
