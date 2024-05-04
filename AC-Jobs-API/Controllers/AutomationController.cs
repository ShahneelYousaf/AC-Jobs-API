using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Domian_Layer.Models.Contact;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AutomationController : ControllerBase
    {
        private readonly IAutomationService _automationService;
        private readonly IEventAutomationService _eventAutomationService;
        private readonly IMapper _mapper;
        private readonly IAutomationBackgroundService _automationBackgroundService;
        public AutomationController(IMapper mapper, IAutomationService automationService, IEventAutomationService eventAutomationService, IAutomationBackgroundService automationBackgroundService)
        {
            _mapper = mapper;
            _automationService = automationService;
            _eventAutomationService = eventAutomationService;
            _automationBackgroundService = automationBackgroundService;
        }

        [HttpGet("")]
        public IActionResult GetAutomationById(int id)
        {
            var obj = _automationService.GetAutomationById(id);
            if (obj == null)
            {
                return NotFound(new NewRecord("Not Found", null));
            }
            else
            {
                return Ok(new NewRecord("Operation Successful", obj));
            }
        }

        [HttpGet("getall")]
        public IActionResult GetAllAutomation()
        {
            var obj = _automationService.GetAllAutomation();
            if (obj == null)
            {
                return NotFound(new NewRecord("Not Found", null));
            }
            else
            {
                return Ok(new NewRecord("Operation Successful", obj));
            }
        }

        [HttpPost("create")]
        public IActionResult CreateAutomation([FromBody] AutomationDto automation)
        {
            var data = _mapper.Map<AutomationEntity>(automation);
            data.Conditions = _mapper.Map<List<ConditionEntity>>(automation.Conditions);
            data.Actions = _mapper.Map<List<ActionEntity>>(automation.Actions);


            var createdAutomation = _automationService.CreateAutomation(data);
            _automationBackgroundService.RestartService();
            return Ok(new NewRecord("Operation Successful", createdAutomation));
        }

        [HttpPut("update")]
        public IActionResult UpdateAutomation([FromBody] AutomationDto automation)
        {
            if (automation?.Id == 0)
            {
                return BadRequest();
            }
            var data = _mapper.Map<AutomationEntity>(automation);
            data.Conditions = _mapper.Map<List<ConditionEntity>>(automation.Conditions);
            data.Actions = _mapper.Map<List<ActionEntity>>(automation.Actions);
            _automationService.UpdateAutomation(data);
            _automationBackgroundService.RestartService();
            return Ok(new NewRecord("Operation Successful", data));
        }

        [HttpPut("test")]
        public IActionResult test()
        {
            var obj = _automationService.GetAutomationById(24);
            if (obj == null)
            {
                return NotFound(new NewRecord("Not Found", null));
            }
            else
            {
                _eventAutomationService.ProcessEventBasedAutomation<Job>("created", "job", 10042);
                return Ok(new NewRecord("Operation Successful", obj));
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteAutomation(int id)
        {

            _automationService.DeleteAutomation(id);
            _automationBackgroundService.RestartService();

            return Ok(new NewRecord("Operation Successful", null));
        }


        [HttpPost("executeContactAutomation")]
        public IActionResult ExecuteContactAutomation([FromBody] DTOContactEventGeneration req)
        {
            _eventAutomationService.ProcessContactEventBasedAutomation<ContactEntity>(req.EventType.ToLower(), "contact", req.Id);
            return Ok(new NewRecord("Operation Successful", null));
        }



    }
}
