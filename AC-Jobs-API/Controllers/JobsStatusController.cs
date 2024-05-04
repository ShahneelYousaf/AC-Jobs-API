using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobsStatusController : ControllerBase
    {
        private readonly ICustomService<JobsStatus> _customService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public JobsStatusController(
            ICustomService<JobsStatus> customService,
            ApplicationDbContext applicationDbContext,
            IMapper mapper
            )
        {
            _customService = customService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }
        [HttpGet(nameof(GetJobsStatusById))]
        public IActionResult GetJobsStatusById(int Id)
        {
            var obj = _customService.Get(Id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(obj);
            }
        }
        [HttpGet(nameof(GetAllJobsStatus))]
        public IActionResult GetAllJobsStatus()
        {
            var obj = _customService.GetAll();
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(obj);
            }
        }
        [HttpPost(nameof(CreateJobsStatus))]
        public IActionResult CreateJobsStatus(DTOCreateJobStatus JobsStatuss)
        {
            if (JobsStatuss != null)
            {
                var eventsdata = _mapper.Map<JobsStatus>(JobsStatuss);
                _customService.Insert(eventsdata);
                return Ok("Created Successfully");
            }
            else
            {
                return BadRequest("Somethingwent wrong");
            }
        }
        [HttpPost(nameof(UpdateJobsStatus))]
        public IActionResult UpdateJobsStatus(JobsStatus JobsStatuss)
        {
            if (JobsStatuss != null)
            {
                _customService.Update(JobsStatuss);
                return Ok("Updated SuccessFully");
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete(nameof(DeleteJobsStatus))]
        public IActionResult DeleteJobsStatus(JobsStatus JobsStatuss)
        {
            if (JobsStatuss != null)
            {
                _customService.Delete(JobsStatuss);
                return Ok("Deleted Successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}