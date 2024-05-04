using Ac.Jobs.API.DTos;
using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly ICustomService<Event> _customService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public EventsController(ICustomService<Event> customService, ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _customService = customService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        [HttpGet(nameof(GetEventById))]
        public IActionResult GetEventById(int Id)
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

        [HttpGet(nameof(GetAllEvent))]
        public IActionResult GetAllEvent()
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


        [HttpGet(nameof(GetEventsByJobId))]
        public IActionResult GetEventsByJobId(int jobId)
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

        [HttpPost(nameof(CreateEvent))]
        public IActionResult CreateEvent(DTOCreateEvent events)
        {
            try
            {
                if (events != null)
                {
                    var eventsdata = _mapper.Map<Event>(events);
                    _customService.Insert(eventsdata);
                    return Ok(new NewRecord("Created Successfully", null));
                }
                else
                {
                    return BadRequest(new NewRecord("Something went wrong", null));
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new NewRecord("Internal Server Error", null));
            }
        }

        [HttpPost(nameof(UpdateEvent))]
        public IActionResult UpdateEvent(DTOEvents events)
        {
            if (events != null)
            {
                var eventsdata = _mapper.Map<Event>(events);
                _customService.Update(eventsdata);
                return Ok(new NewRecord("Updated Successfully", null));
            }
            else
            {
                return BadRequest(new NewRecord("Invalid request", null));
            }
        }

        [HttpDelete(nameof(DeleteEvent))]
        public IActionResult DeleteEvent(DTOEvents events)
        {
            if (events != null)
            {
                var eventsdata = _mapper.Map<Event>(events);
                _customService.Delete(eventsdata);
                return Ok(new NewRecord("Deleted Successfully", null));
            }
            else
            {
                return BadRequest(new NewRecord("Something went wrong", null));
            }
        }
    }

    internal record NewRecord(string Message, dynamic Data);
}
