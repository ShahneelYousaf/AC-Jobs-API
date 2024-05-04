namespace AC_Jobs_API_Domian_Layer.Models
{
    public class BaseEntity
    {

        public long Id { get; set; }
        public long? UserId { get; set; }
        public long? CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
