using System.ComponentModel.DataAnnotations.Schema;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    [Table("RelatedContacts")]
    public class ContactsRelatedContactEntity : ContactBaseEntity
    {
        public long ContactId { get; set; }
        public ContactEntity Contact { get; set; }
        public long RelatedContactId { get; set; }
    }
}
