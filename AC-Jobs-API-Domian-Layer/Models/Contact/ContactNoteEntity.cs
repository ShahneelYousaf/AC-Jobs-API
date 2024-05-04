using System.ComponentModel.DataAnnotations.Schema;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    [Table("Notes")]
    public class ContactNoteEntity : ContactBaseEntity
    {
        public string Note { get; set; }
        public long TypeId { get; set; }
        public long ContactId { get; set; }
        public ContactEntity Contact { get; set; }
    }
}
