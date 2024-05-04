namespace AC_Jobs_API.DTos
{
    public class DTOWorkFlow
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? JobId { get; set; }
    }
}
