namespace AC_Jobs_API_Domian_Layer.Models
{
    public class LineItem : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; } = 0;
        public long? JobId { get; set; }
        public long? WorkOrderId { get; set; }
    }
}
