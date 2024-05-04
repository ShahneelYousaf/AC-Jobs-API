using System.ComponentModel.DataAnnotations.Schema;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    [Table("PhoneNumbers")]
    public class ContactPhoneNumberEntity : ContactBaseEntity
    {
        public string PhoneNumber { get; set; }
        public long TypeId { get; set; }
        public long ContactId { get; set; }
        public ContactEntity Contact { get; set; }
    }
}
