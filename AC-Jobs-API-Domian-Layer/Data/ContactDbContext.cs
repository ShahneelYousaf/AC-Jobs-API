using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Domian_Layer.Models.Contact;
using Microsoft.EntityFrameworkCore;

namespace AC_Jobs_API_Domian_Layer.Data
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
        }

        public DbSet<ContactEntity> ContactEntity { get; set; }
        public DbSet<ContactsRelatedContactEntity> RelatedContactEntity { get; set; }
        public DbSet<ContactTagEntity> TagEntity { get; set; }
        public DbSet<ContactNoteEntity> NoteEntity { get; set; }
        public DbSet<ContactPhoneNumberEntity> PhoneNumberEntity { get; set; }
        public DbSet<ContactCustomFieldEntity> CustomFieldEntity { get; set; }
        public DbSet<ContactTeamMemberEntity> TeamMemberEntity { get; set; }
        public DbSet<ContactsWorkFlowEntity> WorkFlowEntity { get; set; }
        public DbSet<ContactStatusEntity> StatusEntity { get; set; }
        public DbSet<TaskEntity> TaskEntity { get; set; }
        public DbSet<TaskContactEntity> TaskRelatedContactsEntity { get; set; }
        public DbSet<TaskJobEntity> TaskRelatedJobsEntity { get; set; }
        public DbSet<TaskSubContractorEntity> TaskRelatedSubContractorsEntity { get; set; }
        public DbSet<TaskTeamMemberEntity> TaskRelatedTeamMembersEntity { get; set; }


    }
}
