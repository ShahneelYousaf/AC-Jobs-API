using AC_Jobs_API_Domian_Layer.ENums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class WorkOrder : BaseEntity
    {
        public string WorkOrderPriority { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public long? WorkflowId { get; set; }
        public long? WorkOrderStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }





        //----------------Relational Fields-------------------------
        public int? ContactId { get; set; }
        public int? JobId { get; set; }
    }
}
