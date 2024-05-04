using System.ComponentModel.DataAnnotations.Schema;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    [Table("Statuses")]
    public class ContactStatusEntity : ContactBaseEntity
    {
        public string StatusName { get; set; }
        public long WorkFlowId { get; set; }
        public ContactsWorkFlowEntity WorkFlow { get; set; }
    }
}
