using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkFlowController : ControllerBase
    {
        private readonly ICustomService<WorkFlow> _customService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public WorkFlowController(
            ICustomService<WorkFlow> customService, 
            ApplicationDbContext applicationDbContext,
            IMapper mapper)
        {
            _customService = customService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }
        [HttpGet(nameof(GetWorkFlowById))]
        public IActionResult GetWorkFlowById(int Id)
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
        
        [HttpGet(nameof(GetAllWorkFlows))]
        public IActionResult GetAllWorkFlows()
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
       
        [HttpPost(nameof(CreateWorkFlow))]
        public IActionResult CreateWorkFlow(DTOCreateWorkFlow workflow)
        {
            if (workflow != null)
            {
                var workflowData = _mapper.Map<WorkFlow>(workflow);
                _customService.Insert(workflowData);
                return Ok("Created Successfully");
            }
            else
            {
                return BadRequest("Somethingwent wrong");
            }
        }
        [HttpPost(nameof(UpdateWorkFlow))]
        public IActionResult UpdateWorkFlow(DTOWorkFlow workflow)
        {
            if (workflow != null)
            {
                var workflowData = _mapper.Map<WorkFlow>(workflow);
                _customService.Update(workflowData);
                return Ok("Updated SuccessFully");
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete(nameof(DeleteWorkFlow))]
        public IActionResult DeleteWorkFlow(DTOWorkFlow workflow)
        {
            if (workflow != null)
            {
                var workflowdata = _mapper.Map<WorkFlow>(workflow);
                _customService.Delete(workflowdata);
                return Ok("Deleted Successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}