using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Domian_Layer.Models.Contact
{
    [Table("Contacts")]
    public class ContactEntity : ContactBaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Company { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public int? ZipCode { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? FaxNo { get; set; }
        public string? DisplayName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
        public string? Discription { get; set; }
        public string? PicUrl { get; set; }

        //DropDownTable
        public long? SourceId { get; set; }
        public long? StateId { get; set; }
        //FromAuth
        public long? SalesRepId { get; set; }
        public long? SubContractorId { get; set; }
        //RelationalProperties
        public long? OfficeLocationId { get; set; }
        public long WorkFlowId { get; set; }
        public ContactsWorkFlowEntity WorkFlow { get; set; }
        public long? StatusId { get; set; }
        public ContactStatusEntity Status { get; set; }

        //public long SalesRepId { get; set; }
        //public SalesRepresenativeEntity SalesRepresentativ { get; set; }
    }

    public enum WorkFlowType
    {
        contact = 1,
        jobs,
        global,
        workOrder
    }
}
