using System.ComponentModel.DataAnnotations.Schema;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    [Table("CustomFields")]
    public class ContactCustomFieldEntity : ContactBaseEntity
    {
        public string CustomFieldName { get; set; }
        public string Value { get; set; }
        public long ContactId { get; set; }
        public ContactEntity Contact { get; set; }
    }
}
