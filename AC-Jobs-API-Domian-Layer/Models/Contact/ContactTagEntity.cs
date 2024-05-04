using System.ComponentModel.DataAnnotations.Schema;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    [Table("Tags")]
    public class ContactTagEntity : ContactBaseEntity
    {
        public string Tag { get; set; }
        public long ContactId { get; set; }
        public ContactEntity Contact { get; set; }
    }
}
