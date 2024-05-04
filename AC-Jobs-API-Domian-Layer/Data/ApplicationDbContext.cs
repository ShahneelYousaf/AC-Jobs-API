using AC_Jobs_API_Domian_Layer.Models;
using Microsoft.EntityFrameworkCore;

namespace AC_Jobs_API_Domian_Layer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConditionEntity>()
                .HasOne(c => c.AutomationEntity)
                .WithMany(a => a.Conditions)
                .HasForeignKey(c => c.AutomationEntityId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading behavior

            modelBuilder.Entity<ActionEntity>()
                .HasOne(a => a.AutomationEntity)
                .WithMany(ae => ae.Actions)
                .HasForeignKey(a => a.AutomationEntityId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading behavior


            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Attachments> Attachments { get; set; }
        public DbSet<JobAttachmentsEntity> JobAttachments { get; set; }
        public DbSet<PhotosEntity> Photos { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<WorkFlow> WorkFlows { get; set; }
        public DbSet<JobsStatus> JobsStatuses { get; set; }
        public DbSet<LeadSource> LeadSources { get; set; }
        public DbSet<RelatedContacts> RelatedContacts { get; set; }
        public DbSet<SubContractor> SubContractors { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TeamMembers> TeamMembers { get; set; }
        public DbSet<LogbookEntry> JobsLogbook { get; set; }
        public DbSet<BoardWorkFlowStatusEntity> boardWorkFlowStatuses { get; set; }
        public DbSet<BoardStatusEntity> boardStatuses { get; set; }
        public DbSet<BoardEntity> boardEntities { get; set; }
        public DbSet<BoardAccessUserEntity> boardAccessUsers { get; set; }
        public DbSet<FolderEntity> Folders { get; set; }
        public DbSet<ActionEntity> AutomationActions { get; set; }
        public DbSet<ConditionEntity> AutomationConditions { get; set; }
        public DbSet<AutomationEntity> Automations { get; set; }
        public DbSet<EmailTemplateEntity> EmailTemplates { get; set; }
        public DbSet<SMSTemplateEntity> SMSTemplates { get; set; }

    }
}
