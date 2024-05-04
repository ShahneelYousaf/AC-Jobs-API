namespace AC_Jobs_API.DTos
{
    // DTOs for related entities (if needed)
    public class DTOBoardStatus
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SortBy { get; set; }
        public string SortingOrder { get; set; }
        public string Total { get; set; }
        public List<dynamic>? Items { get; set; }

        public List<DTOBoardWorkFlowStatus> WorkFlowStatuses { get; set; }
    }

}
