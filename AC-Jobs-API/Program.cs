using AC_Contact_Services.BaseService;
using AC_Job_API_Service_Layer.Services;
using AC_Jobs_API.Helper;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Domian_Layer.Models.Contact;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Repository_Layer.Repository;
using AC_Jobs_API_Service_Layer.IService;
using AC_Jobs_API_Service_Layer.Services;
using AC_Jobs_API_Service_Layer.Services.Contacts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program));

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(ConnectionString));

var ContactConnectionString = builder.Configuration.GetConnectionString("ContactConnection");
builder.Services.AddDbContext<ContactDbContext>(options => options.UseSqlServer(ContactConnectionString));

builder.Services.AddControllers();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<CustomJwtBearerHandler>();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<JwtBearerOptions, CustomJwtBearerHandler>(JwtBearerDefaults.AuthenticationScheme, options => { });
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICustomService<Job>, JobService>();
builder.Services.AddScoped<ICustomService<JobAttachmentsEntity>, JobAttachmentService>();
builder.Services.AddScoped<ICustomService<PhotosEntity>, PhotoService>();
builder.Services.AddScoped<ICustomService<BoardAccessUserEntity>, BoardAccessUserService>();
builder.Services.AddScoped<ICustomService<BoardEntity>, BoardService>();
builder.Services.AddScoped<ICustomService<BoardStatusEntity>, BoardStatusService>();
builder.Services.AddScoped<ICustomService<BoardWorkFlowStatusEntity>, BoardWorkFlowService>();
builder.Services.AddScoped<ICustomService<Event>, EventService>();
builder.Services.AddScoped<ICustomService<JobsStatus>, JobsStatusService>();
builder.Services.AddScoped<ICustomService<LeadSource>, LeadSourceService>();
builder.Services.AddScoped<ICustomService<SubContractor>, SubContractorService>();
builder.Services.AddScoped<ICustomService<Tag>, TagsService>();
builder.Services.AddScoped<ICustomService<TeamMembers>, TeamMembersService>();
builder.Services.AddScoped<ICustomService<RelatedContacts>, RelatedContactsService>();
builder.Services.AddScoped<ICustomService<LogbookEntry>, LogbookEntryService>();
builder.Services.AddScoped<ICustomService<WorkOrder>, WordOrderService>();
builder.Services.AddScoped<ICustomService<WorkFlow>, WorkFlowService>();
builder.Services.AddScoped<ICustomService<Note>, NoteService>();
builder.Services.AddScoped<ICustomService<LineItem>, LineItemService>();
builder.Services.AddScoped<ICustomService<FolderEntity>, FolderService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEventAutomationService, EventAutomationService>();
builder.Services.AddScoped<IAutomationService, AutomationService>(); 
builder.Services.AddScoped<ICustomService<EmailTemplateEntity>, EmailTemplateService>();
builder.Services.AddScoped<ICustomService<SMSTemplateEntity>, SMSTemplateService>();

builder.Services.AddScoped(typeof(IContactRepository<>), typeof(ContactRepository<>));
builder.Services.AddScoped<IContactCustomService<ContactEntity>, ContactEntityService>();
builder.Services.AddScoped<IContactCustomService<ContactsRelatedContactEntity>, ContactsRelatedContactEntityService>();
builder.Services.AddScoped<IContactCustomService<ContactTagEntity>, ContactTagEntityService>();
builder.Services.AddScoped<IContactCustomService<ContactNoteEntity>, ContactNoteEntityService>();
builder.Services.AddScoped<IContactCustomService<ContactPhoneNumberEntity>, ContactPhoneNumberEntityService>();
builder.Services.AddScoped<IContactCustomService<ContactCustomFieldEntity>, ContactCustomFieldEntityService>();
builder.Services.AddScoped<IContactCustomService<ContactTeamMemberEntity>, ContactTeamMemberEntityService>();
builder.Services.AddScoped<IContactCustomService<ContactsWorkFlowEntity>, ContactsWorkFlowEntityService>();
builder.Services.AddScoped<IContactCustomService<ContactStatusEntity>, ContactStatusEntityService>();

builder.Services.AddScoped<IContactCustomService<TaskEntity>, TaskEntityService>();
builder.Services.AddScoped<IContactCustomService<TaskContactEntity>, TaskContactEntityService>();
builder.Services.AddScoped<IContactCustomService<TaskJobEntity>, TaskJobEntityService>();
builder.Services.AddScoped<IContactCustomService<TaskSubContractorEntity>, TaskSubContractorEntityService>();
builder.Services.AddScoped<IContactCustomService<TaskTeamMemberEntity>, TaskTeamMemberEntityService>();


builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped<IContactRepositoryFactory, ContactRepositoryFactory>();
builder.Services.AddScoped<IAutomationBackgroundService, AutomationBackgroundService>();
builder.Services.AddHostedService<AutomationBackgroundService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv6", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
            });
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
    builder =>
    {
        builder.WithOrigins().AllowAnyHeader().AllowAnyMethod();
    });
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
