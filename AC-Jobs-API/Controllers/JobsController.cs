using AC_Contact_Services.BaseService;
using AC_Jobs_API.DTos;
using AC_Jobs_API.Filter;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AC_Jobs_API_Service_Layer.Models.Healper;
using AC_Jobs_API_Service_Layer.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly ICustomService<Job> _customService;
        private readonly ICustomService<LogbookEntry> _logbookService;
        private readonly ICustomService<RelatedContacts> _relatedContacts;
        private readonly ICustomService<TeamMembers> _teammembers;
        private readonly ICustomService<LeadSource> _leadSource;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IContactService _contactService;
        private readonly IEventAutomationService _eventAutomationService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public long LoggedInUserCompanyId;
        private readonly ApplicationDbContext _db;

        public JobsController(
            ICustomService<Job> customService,
            ICustomService<RelatedContacts> relatedContacts,
            ICustomService<LeadSource> leadSource,
            ApplicationDbContext applicationDbContext,
            IHttpContextAccessor httpContextAccessor,
            IContactService contactService,
            ApplicationDbContext db,
            IMapper mapper,
            ICustomService<TeamMembers> teammembers,
            ICustomService<LogbookEntry> logbookService,
            IEventAutomationService eventAutomationService)
        {
            _customService = customService;
            _relatedContacts = relatedContacts;
            _leadSource = leadSource;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _db = db;
            _teammembers = teammembers;
            _contactService = contactService;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            _logbookService = logbookService;
            _eventAutomationService = eventAutomationService;
        }
        [HttpGet(nameof(GetJobById))]
        public IActionResult GetJobById(int Id)
        {
            var obj = _customService.Get(Id);
            
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                var jobsData = _mapper.Map<DTOJobs>(obj);
                jobsData.TeamMememberId = _teammembers.GetAll().Where(x => x.JobId == jobsData.Id).Select(x => x.TeamMemberId).ToList();
                jobsData.RelatedContactId = _relatedContacts.GetAll().Where(x => x.JobId == jobsData.Id).Select(x => x.RelatedContactId).ToList();
     
                return Ok(jobsData);
            }
        }
        /*[HttpGet(nameof(GetAllJobByCompanyIdWithPagination))]
        public IActionResult GetAllJobByCompanyIdWithPagination(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = _customService.GetAll()
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToList();
            var totalRecords = _customService.GetAll();
            return Ok(new PagedResponse<Job>(pagedData, validFilter.PageNumber, validFilter.PageSize));
        }*/
        
        [HttpGet(nameof(GetAllJobByCompanyId))]
        public IActionResult GetAllJobByCompanyId()
        {
            var response = _customService.GetAll();
            if (response == null)
            {
                return NotFound();
            }
            else
            {
                var jobsData = _mapper.Map<List<DTOJobs>>(response);
                foreach (var item in jobsData)
                {
                    item.TeamMememberId = _teammembers.GetAll().Where(x => x.JobId == item.Id).Select(x => x.TeamMemberId).ToList();
                    item.RelatedContactId = _relatedContacts.GetAll().Where(x => x.JobId == item.Id).Select(x => x.RelatedContactId).ToList();
                }
                return Ok(jobsData);
            }
        }

        [HttpGet(nameof(GetAllJobWithPagination))]
        public IActionResult GetAllJobWithPagination([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var totalRecords = _customService.CountAll();

            var pagedData = _customService.GetAll()
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize).ToList();
            var jobsData = _mapper.Map<List<DTOJobs>>(pagedData);
            foreach (var item in jobsData)
            {
                item.TeamMememberId = _teammembers.GetAll().Where(x => x.JobId == item.Id).Select(x => x.TeamMemberId).ToList();
                item.RelatedContactId = _relatedContacts.GetAll().Where(x => x.JobId == item.Id).Select(x => x.RelatedContactId).ToList();
            }

            var response = new
            {
                pageIndex = validFilter.PageNumber,
                pageSize = validFilter.PageSize,
                count = totalRecords,
                Data = jobsData
            };

            return Ok(response);

        }

        [HttpPost(nameof(CreateJob))]
        public IActionResult CreateJob(DTOCreateJobs Jobdto)
        {
            if (Jobdto != null)
            {
                var jobsData = _mapper.Map<Job>(Jobdto);
                jobsData.LastStatusChangeDate = DateTime.Now;
                var data = _customService.Insert(jobsData);

                foreach (var item in Jobdto.RelatedContactId)
                {
                    _relatedContacts.Insert(new RelatedContacts()
                    {
                        JobId = data.Id,
                        RelatedContactId = item,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now,
                    });
                }
                foreach (var item in Jobdto.TeamMememberId)
                {
                    _teammembers.Insert(new TeamMembers()
                    {
                        JobId = data.Id,
                        TeamMemberId = item,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now,
                    });
                }
                //Trigger Automation
                _eventAutomationService.ProcessEventBasedAutomation<Job>("created", "job", data.Id);
                return Ok(new { message = "Created Successfully" });
            }
            else
            {
                return BadRequest(new { message = "Somethingwent wrong" });
            }
        }

        [HttpPost(nameof(UpdateJob))]
        public IActionResult UpdateJob(DTOJobs Jobdto)
        {
            if (Jobdto != null)
            {
                var existingJob = _customService.Get(Jobdto.Id);
                if (existingJob != null)
                {
                    var jobsData = _mapper.Map<Job>(Jobdto);
                    if (existingJob.JobStatusId != Jobdto.JobStatusId)
                    {
                        jobsData.LastStatusChangeDate = DateTime.Now;
                    }

                    _customService.Update(jobsData);

                    var rcontats = _relatedContacts.GetAll().Where(x => x.JobId == Jobdto.Id);
                    foreach (var item in rcontats)
                    {
                        _relatedContacts.Delete(item);
                    }
                    foreach (var item in Jobdto.RelatedContactId)
                    {
                        _relatedContacts.Update(new RelatedContacts()
                        {
                            JobId = Jobdto.Id,
                            RelatedContactId = item,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now,
                        });
                    }

                    var tm = _teammembers.GetAll().Where(x => x.JobId == Jobdto.Id && x.IsDeleted != true);
                    foreach (var item in tm)
                    {
                        _teammembers.Delete(item);
                    }
                    foreach (var item in Jobdto.TeamMememberId)
                    {
                        _teammembers.Insert(new TeamMembers()
                        {
                            JobId = Jobdto.Id,
                            TeamMemberId = item,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now,
                        });
                    }

                    try
                    {
                        if (existingJob.JobStatusId != Jobdto.JobStatusId)
                        {
                            
                            var existingStatusName = _contactService.GetStatusNameAsync((long)existingJob.JobStatusId).Result;
                            var newStatusName = _contactService.GetStatusNameAsync((long)Jobdto.JobStatusId).Result;

                            var logbookData = new LogbookEntry()
                            {
                                JobId = existingJob.Id,
                                Activity = $"Status Changed from \"{existingStatusName.ToUpper()}\" to \"{newStatusName.ToUpper()}\"",
                                Type = "Status Changed",
                                Comments = ""
                            };

                            _logbookService.Insert(logbookData);
                        }

                        if (existingJob.PrimaryContactId != Jobdto.PrimaryContactId)
                        {
                            var existingPcName = _contactService.GetTeamMemberNameAsync((long)existingJob.PrimaryContactId);
                            var newPcName = _contactService.GetTeamMemberNameAsync((long)Jobdto.PrimaryContactId);
                            var logbookData = new LogbookEntry()
                            {
                                JobId = existingJob.Id,
                                Activity = $"Primary Contact Changed from \"{existingPcName}\" To \"{newPcName}\"",
                                Type = "Primary Contact Changed",
                                Comments = ""
                            };
                            _logbookService.Insert(logbookData);
                        }

                        var existingTeamMembers = new HashSet<long>(tm.Select(x => x.TeamMemberId));
                        var newTeamMembers = new HashSet<long>(Jobdto.TeamMememberId);

                        var addedTeamMembers = newTeamMembers.Except(existingTeamMembers).ToList();
                        var removedTeamMembers = existingTeamMembers.Except(newTeamMembers).ToList();

                        var addedTeamMemberNames = _contactService.GetTeamMemberNamesAsync(addedTeamMembers);
                        var removedTeamMemberNames = _contactService.GetTeamMemberNamesAsync(removedTeamMembers);

                        if (addedTeamMemberNames.Any())
                        {

                            var logbookData = new LogbookEntry()
                            {
                                JobId = Jobdto.Id,
                                Activity = $"Team Members Updated From: {string.Join(", ", removedTeamMemberNames.Select(name => $"\"{name.ToUpper()}\""))} To: {string.Join(", ", addedTeamMemberNames.Select(name => $"\"{name.ToUpper()}\""))}",
                                Type = "Team Members Updated",
                                Comments = ""
                            };

                            _logbookService.Insert(logbookData);
                        }
                        else if (removedTeamMemberNames.Any())
                        {
                            var logbookData = new LogbookEntry()
                            {
                                JobId = Jobdto.Id,
                                Activity = $"Team Members Removed: {string.Join(", ", removedTeamMemberNames.Select(name => $"\"{name.ToUpper()}\""))}",
                                Type = "Team Members Removed",
                                Comments = ""
                            };
                            _logbookService.Insert(logbookData);
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    _eventAutomationService.ProcessEventBasedAutomation<Job>("modified", "job", Jobdto.Id);
                    return Ok(new { message = "Updated Successfully." });
                }
                else
                {
                    return NotFound(new { message = "Job not found!" });
                }
            }
            else
            {
                return BadRequest(new { message = "Invalid input!" });
            }
        }

        [HttpDelete(nameof(DeleteJob))]
        public IActionResult DeleteJob(DTODeleteJob Jobdto)
        {
            if (Jobdto != null)
            {
                var data = _customService.Get(Jobdto.Id);
                if (Jobdto != null)
                {
                    var jobsData = _mapper.Map<Job>(data);
                    _customService.Delete(jobsData);

                    //var rcontats = _relatedContacts.GetAll().Where(x => x.JobId == Jobdto.Id);
                    //foreach (var item in rcontats)
                    //{
                    //    _relatedContacts.Remove(item);
                    //}
                    //foreach (var item in Jobdto.RelatedContactId)
                    //{
                    //    _relatedContacts.Remove(new RelatedContacts()
                    //    {
                    //        JobId = Jobdto.Id,
                    //        RelatedContactId = item,
                    //        CreatedBy = 1,
                    //        CreatedDate = DateTime.Now,
                    //    });
                    //}

                    //var tm = _teammembers.GetAll().Where(x => x.JobId == Jobdto.Id);
                    //foreach (var item in tm)
                    //{
                    //    _teammembers.Remove(item);
                    //}
                    //foreach (var item in Jobdto.TeamMememberId)
                    //{
                    //    _teammembers.Remove(new TeamMembers()
                    //    {
                    //        JobId = Jobdto.Id,
                    //        TeamMemberId = item,
                    //        CreatedBy = 1,
                    //        CreatedDate = DateTime.Now,
                    //    });
                    //}
                }

                _eventAutomationService.ProcessEventBasedAutomation<Job>("deleted", "job", Jobdto.Id);

                return Ok("Deleted SuccessFully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet(nameof(GetJobAnalytics))]
        public IActionResult GetJobAnalytics(string? timeFrame, int? sourceId)
        {
            int currentMonthJobCount = 0;
            int lastMonthJobCount = 0;
            var jobs = _customService.GetAll();
            DateTime currentDate = DateTime.Now;
            JobAnalyticsResponseDto response = new JobAnalyticsResponseDto();
            List<SourceWiseJobsResponseDto> sourceResponse = new List<SourceWiseJobsResponseDto>();
            List<YearWiseJobsResponseDto> yearWiseResponse = new List<YearWiseJobsResponseDto>();

            var leadSources = _leadSource.GetAll();
            DateTime createdDate;

            if (jobs == null)
                return NotFound();

            currentMonthJobCount = jobs.Count(
                c => c.CreatedDate?.Month == DateTime.Now.Month);

            lastMonthJobCount = jobs.Count(
                c => c.CreatedDate?.Month == DateTime.Now.Month - 1);

            if (timeFrame != null)
            {
                switch (timeFrame.ToLower())
                {
                    case "daily":
                        createdDate = DateTime.Now.Date;
                        break;
                    case "monthly":
                        createdDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        break;
                    case "yearly":
                        createdDate = new DateTime(DateTime.Now.Year, 1, 1);
                        break;
                    default:
                        throw new ArgumentException("Invalid timeframe. Expected 'daily', 'monthly', or 'yearly'.");
                }
            }
            else
            {
                createdDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            response.NewJobs = jobs.Count(c => c.CreatedDate >= createdDate);

            if (sourceId != 0 && sourceId != null)
            {
                response.SourceWiseCount = jobs.Count(c => c.LeadSourceId == sourceId);
            }
            else
            {
                response.SourceWiseCount = jobs.Count();
            }

            var result = jobs
                .GroupBy(c => c.CreatedDate?.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() });

            foreach (var item in result)
            {
                YearWiseJobsResponseDto yearWiseResp = new YearWiseJobsResponseDto
                {
                    Year = Convert.ToInt32(item?.Year),
                    Count = item.Count
                };

                var monthlyCounts = Enumerable.Range(1, 12) // Generate numbers from 1 to 12 (representing months)
                    .Select(month => new MonthWiseJobsResponseDto
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                        Count = jobs
                            .Count(c => c.CreatedDate?.Year == item.Year && c.CreatedDate?.Month == month)
                    })
                    .ToList();

                yearWiseResp.MonthWiseJobsCount = monthlyCounts;

                yearWiseResponse.Add(yearWiseResp);
            }

            var sourceResult = jobs
                    .GroupBy(c => c.LeadSourceId)
                    .Select(g => new { SourceId = g.Key, Count = g.Count() });
            foreach (var item in sourceResult)
            {
                SourceWiseJobsResponseDto sourceResp = new SourceWiseJobsResponseDto();
                sourceResp.SourceId = item.SourceId;
                sourceResp.Count = item.Count;
                sourceResp.SourceName = leadSources
                    .Where(source => source.Id == item.SourceId)
                    .Select(source => source.Name)
                    .FirstOrDefault();

                sourceResponse.Add(sourceResp);
            }
            response.TotalJobs = jobs.Count();
            response.YearWiseJobsCount = yearWiseResponse;
            response.SourceWiseResponse = sourceResponse;
            if (lastMonthJobCount == 0 && currentMonthJobCount == 0)
            {
                // Handle the case where both last month and current month sales are 0
                response.PercentageIncrease = 0; // Display as 0% increase
            }
            else if (lastMonthJobCount == 0 && currentMonthJobCount != 0)
            {
                // Handle the case where the last month sale was 0
                response.PercentageIncrease = 100; // Display as 100% increase
            }
            else if (currentMonthJobCount == 0)
            {
                // Handle the case where the current month sale is 0
                response.PercentageIncrease = -100; // Display as -100% decrease
            }
            else
            {
                // Calculate percentage increase using the formula
                response.PercentageIncrease = ((currentMonthJobCount - lastMonthJobCount) / lastMonthJobCount) * 100;
            }
            response.JobsCompleted = jobs.Count(
                c => c.JobStatusId == 2);

            if (response == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpGet(nameof(SearchAll))]
        public async Task<IActionResult> SearchAll(string? name)
        {
            var response = new
            {
                Jobs = new List<SearchResponseDto>(),
                Contacts = new List<SearchResponseDto>(),
                WorkOrders = new List<SearchResponseDto>()
            };

            var jobs = _db.Jobs
                .Where(j => j.Name.ToLower().Contains(name.ToLower())
                            && j.CompanyId == LoggedInUserCompanyId
                            && j.IsDeleted != true)
                .Select(item => new SearchResponseDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    IsJob = "true"
                })
                .ToList();

            response.Jobs.AddRange(jobs);

            var workOrders = _db.WorkOrders
                .Where(j => j.Name.ToLower().Contains(name.ToLower())
                            && j.CompanyId == LoggedInUserCompanyId
                            && j.IsDeleted != true)
                .Select(item => new SearchResponseDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = "",
                    IsWorkOrder = "true"
                })
                .ToList();

            response.WorkOrders.AddRange(workOrders);

            var contacts = await _contactService.SearchContactsAsync(name);
            if (contacts.statusCode == 202)
            {
                var contactDtos = contacts.payload
                    .Select(item => new SearchResponseDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        IsContact = "true"
                    })
                    .ToList();

                response.Contacts.AddRange(contactDtos);
            }

            return Ok(response);
        }

        [HttpPost("updateJobStatusAndWorkflow")]
        public async Task<IActionResult> updateJobStatusAndWorkflow([FromQuery] long id, long workflowId, long statusId)
        {
                var existingJob = _customService.Get(id);
                if (existingJob != null)
                {
                var existingStatusName = _contactService.GetStatusNameAsync((long)existingJob.JobStatusId).Result;
                var newStatusName = _contactService.GetStatusNameAsync(statusId).Result;

                var logbookData = new LogbookEntry()
                {
                    JobId = existingJob.Id,
                    Activity = $"Status Changed from \"{existingStatusName.ToUpper()}\" to \"{newStatusName.ToUpper()}\"",
                    Type = "Status Changed",
                    Comments = ""
                };

                existingJob.WorkFlowId = workflowId;
                existingJob.JobStatusId = statusId; 
                existingJob.LastStatusChangeDate = DateTime.Now;
                _customService.Update(existingJob);
                _logbookService.Insert(logbookData);
                _eventAutomationService.ProcessEventBasedAutomation<Job>("modified", "job", existingJob.Id);

                return Ok(new { message = "Updated Successfully." });
                }
                else
                {
                    return NotFound(new { message = "Job not found!" });
                }
        }


    }
}