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
    public class RelatedContactController : ControllerBase
    {
        private readonly ICustomService<RelatedContacts> _customService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public RelatedContactController(ICustomService<RelatedContacts> customService, ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _customService = customService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }
        [HttpGet(nameof(GetRelatedContactsById))]
        public IActionResult GetRelatedContactsById(int Id)
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
        [HttpGet(nameof(GetAllRelatedContacts))]
        public IActionResult GetAllRelatedContacts()
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
        [HttpPost(nameof(CreateRelatedContacts))]
        public IActionResult CreateRelatedContacts(DTOCreateRelatedContacts RelatedContactss)
        {

            try
            {
                if (RelatedContactss != null)
                {
                    var relatedContactsData = _mapper.Map<RelatedContacts>(RelatedContactss);
                    _customService.Insert(relatedContactsData);
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
        [HttpPost(nameof(UpdateRelatedContacts))]
        public IActionResult UpdateRelatedContacts(DTORelatedContacts RelatedContactss)
        {
            if (RelatedContactss != null)
            {
                var relatedContactsData = _mapper.Map<RelatedContacts>(RelatedContactss);
                _customService.Update(relatedContactsData);
                return Ok("Updated SuccessFully");
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete(nameof(DeleteRelatedContacts))]
        public IActionResult DeleteRelatedContacts(DTORelatedContacts RelatedContactss)
        {
            if (RelatedContactss != null)
            {
                var relatedContactsData = _mapper.Map<RelatedContacts>(RelatedContactss);
                _customService.Delete(relatedContactsData);
                return Ok("Deleted Successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}