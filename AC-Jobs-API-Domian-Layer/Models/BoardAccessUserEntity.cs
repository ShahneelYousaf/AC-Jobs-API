namespace AC_Jobs_API_Domian_Layer.Models
{
    public class BoardAccessUserEntity : BaseEntity
    {
        public long BoardId { get; set; }
        public long AccessUserId { get; set; }
    }
}
