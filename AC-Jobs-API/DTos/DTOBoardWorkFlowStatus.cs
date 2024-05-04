namespace AC_Jobs_API.DTos
{ 
    public class DTOBoardWorkFlowStatus
    {
        public long WorkFlowId { get; set; }
        public long WorkFlowStatusId { get; set; }
        public long StatusId { get; set; }
        public string? workFlowName { get; set; }
        public string? statusName { get; set; }
    }

}