using System.ComponentModel.DataAnnotations;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    public class ContactBaseEntity
    {
        [Key]
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
