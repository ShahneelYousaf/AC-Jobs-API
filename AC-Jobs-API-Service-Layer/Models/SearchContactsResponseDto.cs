using AC_Jobs_API_Domian_Layer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Service_Layer.Models
{
    public class SearchContactsResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IsContact { get; set; } = "true";
    }

    public class ContactDTO : BaseEntity
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
        public string? Discription { get; set; }
        public string? PicUrl { get; set; }
        public long? SourceId { get; set; }
        public long? StateId { get; set; }
        public long? SalesRepId { get; set; }
        public long? SubContractorId { get; set; }
        //RelationalProperties
        public long? OfficeLocationId { get; set; }
        public long WorkFlowId { get; set; }
        public long? StatusId { get; set; }
    }

    public class ContactBoardsResponseDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Company { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public int? ZipCode { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? LastStatusChangeDate { get; set; }
        public string? StateName { get; set; }

        public string? FaxNo { get; set; }
        public string? DisplayName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Discription { get; set; }
        public string? PicUrl { get; set; }
        public List<DetailDto> TeamMembers { get; set; } = new List<DetailDto> { };
        public SalesRepresentativeDropDownResponseDto? SalesReps { get; set; }


        //DropDownTable
        public long? SourceId { get; set; }
        public long? StateId { get; set; }
        //FromAuth
        public long? SalesRepId { get; set; }
        public long? SubContractorId { get; set; }
        //RelationalProperties
        public long? OfficeLocationId { get; set; }
        public long WorkFlowId { get; set; }
        public long? StatusId { get; set; }

        //public long SalesRepId { get; set; }
        //public SalesRepresenativeEntity SalesRepresentativ { get; set; }
    }
    public class DetailDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class SalesRepresentativeDropDownResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class GetEmailDto
    {
        public List<string> Emails { get; set; }
    }

}
