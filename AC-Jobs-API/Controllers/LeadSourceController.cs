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
    public class LeadSourceController : ControllerBase
    {
        private readonly ICustomService<LeadSource> _customService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public LeadSourceController(ICustomService<LeadSource> customService, ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _customService = customService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }
        [HttpGet(nameof(GetLeadSourceById))]
        public IActionResult GetLeadSourceById(int Id)
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
        [HttpGet(nameof(GetAllLeadSource))]
        public IActionResult GetAllLeadSource()
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
        [HttpPost(nameof(CreateLeadSource))]
        public IActionResult CreateLeadSource(DTOCreateLeadSource LeadSources)
        {

            try
            {
                if (LeadSources != null)
                {
                    var leadSource = _mapper.Map<LeadSource>(LeadSources);
                    _customService.Insert(leadSource);
                    return Ok("Created Successfully");
                }
                else
                {
                    return BadRequest("Somethingwent wrong");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost(nameof(UpdateLeadSource))]
        public IActionResult UpdateLeadSource(DTOLeadSource LeadSources)
        {
            if (LeadSources != null)
            {
                var leadSource = _mapper.Map<LeadSource>(LeadSources);
                _customService.Update(leadSource);
                return Ok("Updated SuccessFully");
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete(nameof(DeleteLeadSource))]
        public IActionResult DeleteLeadSource(DTOLeadSource LeadSources)
        {
            if (LeadSources != null)
            {
                var leadSource = _mapper.Map<LeadSource>(LeadSources);
                _customService.Delete(leadSource);
                return Ok("Deleted Successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}