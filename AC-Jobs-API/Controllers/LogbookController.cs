using Ac.Jobs.API.DTos;
using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobsLogbookController : ControllerBase
    {
        private readonly ICustomService<LogbookEntry> _customService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public JobsLogbookController(ICustomService<LogbookEntry> customService, ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _customService = customService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        [HttpGet(nameof(GetLogById))]
        public IActionResult GetLogById(int Id)
        {
            var obj = _customService.Get(Id);
            if (obj == null)
            {
                return NotFound(new NewRecord("Not Found", null));
            }
            else
            {
                return Ok(new NewRecord("Operation Successful", obj));
            }
        }

        [HttpGet(nameof(GetAllLogs))]
        public IActionResult GetAllLogs()
        {
            var obj = _customService.GetAll();
            if (obj == null)
            {
                return NotFound(new NewRecord("Operation Successful", obj));
            }
            else
            {
                return Ok(new NewRecord("Operation Successful", obj));
            }
        }


        [HttpGet(nameof(GetLogsByJobId))]
        public IActionResult GetLogsByJobId(int jobId)
        {
            var events = _customService.GetAllAsync().Where(x => x.JobId == jobId);
            if (events == null)
            {
                return NotFound(new NewRecord("No events found for the given JobId", null));
            }
            else
            {
                return Ok(new NewRecord("Operation Successful", events));
            }
        }

        [HttpDelete(nameof(DeleteLog))]
        public IActionResult DeleteLog(LogbookEntry log)
        {
            if (log != null)
            {
                _customService.Delete(log);
                return Ok(new NewRecord("Deleted Successfully", null));
            }
            else
            {
                return BadRequest(new NewRecord("Something went wrong", null));
            }
        }
    }
}
