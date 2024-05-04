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
    public class TagsController : ControllerBase
    {
        private readonly ICustomService<Tag> _customService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public TagsController(ICustomService<Tag> customService, ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _customService = customService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }
        [HttpGet(nameof(GetTagById))]
        public IActionResult GetTagById(int Id)
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
        [HttpGet(nameof(GetAllTag))]
        public IActionResult GetAllTag()
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
        [HttpPost(nameof(CreateTag))]
        public IActionResult CreateTag(DTOCreateTags Tags)
        {

            try
            {
                if (Tags != null)
                {
                    var tagsData = _mapper.Map<Tag>(Tags);
                    _customService.Insert(tagsData);
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
        [HttpPost(nameof(UpdateTag))]
        public IActionResult UpdateTag(DTOTag Tags)
        {
            if (Tags != null)
            {
                var tagsData = _mapper.Map<Tag>(Tags);
                _customService.Update(tagsData);
                return Ok("Updated SuccessFully");
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete(nameof(DeleteTag))]
        public IActionResult DeleteTag(DTOTag Tags)
        {
            if (Tags != null)
            {
                var tagsData = _mapper.Map<Tag>(Tags);
                _customService.Delete(tagsData);
                return Ok("Deleted Successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}