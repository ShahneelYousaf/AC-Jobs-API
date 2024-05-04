﻿namespace AC_Jobs_API.DTos
{
    public class DTOGetWordOrder
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? WorkflowId { get; set; }
        public long? WorkOrderStatus { get; set; }
        public List<long> TeamMememberId { get; set; }
        public List<long> SubContractorId { get; set; }
        public string WorkOrderPriority { get; set; }
        public int? JobId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<LineItems> LineItems { get; set; }
        public List<DTONote> Notes { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
        //----------------Relational Fields-------------------------
        public int? ContactId { get; set; }
    }

}
