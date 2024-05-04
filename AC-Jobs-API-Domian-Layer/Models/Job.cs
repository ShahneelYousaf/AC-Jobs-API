using AC_Jobs_API_Domian_Layer.ENums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Domian_Layer.Models
{
    public class Job : BaseEntity
    {
        [Required(ErrorMessage = "Name name is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
        public string Name { get; set; }
        [MaxLength(200)]
        public string? Address1 { get; set; }
        [MaxLength(200)]
        public string? Address2 { get; set; }
        [MaxLength(50)]
        public string? City { get; set; }
        public string? FaxNo { get; set; }
        public string? MobileNo { get; set; }
        public string? HomeNo { get; set; }
        public string? OfficeNo { get; set; }
        public string? PhoneNo { get; set; }
        public string? JobType { get; set; }
        public string? Note { get; set; }
        public long? Zip { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public long? StateId { get; set; }
        //Additional Details
        public long SalesRepsentativeId { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
        //----------------Relational Fields-------------------------
        public long? PrimaryContactId { get; set; }
        // ---------------To be configured in DB--------------------------
        public long OfficeLocationId { get; set; }
        public long WorkFlowId { get; set; }
        public long SubContractorId { get; set; }
        //public long ProductioManagerId { get; set; }
        public long TimelineId { get; set; }
        public long JobStatusId { get; set; }
        public long LeadSourceId { get; set; }

        

    }
}
