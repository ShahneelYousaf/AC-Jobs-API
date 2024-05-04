namespace AC_Jobs_API.DTos
{
    public class DTODeleteJob
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public long? StateId { get; set; }
        public string? FaxNo { get; set; }
        public string? MobileNo { get; set; }
        public string? HomeNo { get; set; }
        public string? OfficeNo { get; set; }
        public string? PhoneNo { get; set; }
        public string? JobType { get; set; }
        public string? Note { get; set; }
        public long? Zip { get; set; }
        public long JobStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public long LeadSourceId { get; set; }
        public long SalesRepsentativeId { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }
        public long? PrimaryContactId { get; set; }
        public long? OfficeLocationId { get; set; }
        public long? WorkFlowId { get; set; }
        public long SubContractorId { get; set; }
    }
}
