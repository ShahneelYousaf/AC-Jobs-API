namespace AC_Jobs_API_Service_Layer.Models
{
    public class LineItems
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; } = 0;
    }

}
