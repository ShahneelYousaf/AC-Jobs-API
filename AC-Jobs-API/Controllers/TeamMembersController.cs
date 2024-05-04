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
    public class TeamMembersController : ControllerBase
    {
        private readonly ICustomService<TeamMembers> _customService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public TeamMembersController(ICustomService<TeamMembers> customService, ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _customService = customService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }
        [HttpGet(nameof(GetTeamMembersById))]
        public IActionResult GetTeamMembersById(int Id)
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
        [HttpGet(nameof(GetAllTeamMembers))]
        public IActionResult GetAllTeamMembers()
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
        [HttpPost(nameof(CreateTeamMembers))]
        public IActionResult CreateTeamMembers(DTOCreateTeamMembers TeamMemberss)
        {

            try
            {
                if (TeamMemberss != null)
                {
                    var TeamMemberData = _mapper.Map<TeamMembers>(TeamMemberss);
                    _customService.Insert(TeamMemberData);
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
        [HttpPost(nameof(UpdateTeamMembers))]
        public IActionResult UpdateTeamMembers(DTOTeamMembers TeamMemberss)
        {
            if (TeamMemberss != null)
            {
                var TeamMembersData = _mapper.Map<TeamMembers>(TeamMemberss);
                _customService.Update(TeamMembersData);
                return Ok("Updated SuccessFully");
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete(nameof(DeleteTeamMembers))]
        public IActionResult DeleteTeamMembers(DTOTeamMembers TeamMemberss)
        {
            if (TeamMemberss != null)
            {
                var TeamMembersData = _mapper.Map<TeamMembers>(TeamMemberss);
                _customService.Delete(TeamMembersData);
                return Ok("Deleted Successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}