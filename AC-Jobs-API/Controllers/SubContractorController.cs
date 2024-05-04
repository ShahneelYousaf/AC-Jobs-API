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
    public class SubContractorController : ControllerBase
    {
        private readonly ICustomService<SubContractor> _customService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public SubContractorController(ICustomService<SubContractor> customService, ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _customService = customService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }
        [HttpGet(nameof(GetSubContractorById))]
        public IActionResult GetSubContractorById(int Id)
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
        [HttpGet(nameof(GetAllSubContractor))]
        public IActionResult GetAllSubContractor()
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
        [HttpPost(nameof(CreateSubContractor))]
        public IActionResult CreateSubContractor(DTOCreateSubContractor SubContractors)
        {

            try
            {
                if (SubContractors != null)
                {
                    var subContractor = _mapper.Map<SubContractor>(SubContractors);
                    _customService.Insert(subContractor);
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
        [HttpPost(nameof(UpdateSubContractor))]
        public IActionResult UpdateSubContractor(DTOSubContractor SubContractors)
        {
            if (SubContractors != null)
            {
                var subContractors = _mapper.Map<SubContractor>(SubContractors);
                _customService.Update(subContractors);
                return Ok("Updated SuccessFully");
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete(nameof(DeleteSubContractor))]
        public IActionResult DeleteSubContractor(DTOSubContractor SubContractors)
        {
            if (SubContractors != null)
            {
                var subContractor = _mapper.Map<SubContractor>(SubContractors);
                _customService.Delete(subContractor);
                return Ok("Deleted Successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}