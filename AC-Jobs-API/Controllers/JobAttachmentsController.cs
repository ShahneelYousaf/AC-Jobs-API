using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobAttachmentsController : ControllerBase
    {
        private readonly ICustomService<JobAttachmentsEntity> _attachmentService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        public long LoggedInUserCompanyId;
        public string LoggedInUserName;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JobAttachmentsController(ApplicationDbContext applicationDbContext, IMapper mapper, ICustomService<JobAttachmentsEntity> attachmentService, IHttpContextAccessor httpContextAccessor, IFileService fileService)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserName = user.FindFirst("name").Value.ToString();
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
        }

        [HttpGet(nameof(GetAttachmentById))]
        public IActionResult GetAttachmentById(int Id)
        {
            var obj = _attachmentService.Get(Id);
            if (obj == null)
            {
                return BadRequest(new { Message = "Attachment Not Found" });
            }
            else
            {
                _attachmentService.GetAllAsync().Where(x => x.Id == Id).FirstOrDefault();
                return Ok(new { Message = "Operation Successful", data = obj });
            }
        }

        [HttpGet(nameof(GetAttachmentByJobId))]
        public IActionResult GetAttachmentByJobId(int Id)
        {
            var obj = _attachmentService.GetAllAsync().Where(x => x.JobId == Id).ToList();
            if (obj == null)
            {
                return BadRequest(new { Message = "Attachment Not Found" });
            }
            else
            {
                return Ok(new { Message = "Operation Successful", data = obj });
            }
        }

        [HttpGet(nameof(GetAllAttachments))]
        public IActionResult GetAllAttachments()
        {
            var obj = _attachmentService.GetAll();
            if (obj == null)
            {
                return BadRequest(new { Message = "Attachment Not Found" });
            }
            else
            {
                return Ok(new { Message = "Operation Successful", data = obj });
            }
        }

        [HttpPost(nameof(CreateAttachment))]
        public IActionResult CreateAttachment([FromForm] JobsAttachmentCreateDto createDto)
        {
            try
            {
                if (createDto != null)
                {
                    var fileResult = _fileService.AddFileAsync(createDto.File).Result;
                    var attachmentData = _mapper.Map<JobAttachmentsEntity>(fileResult);
                    attachmentData.Description = createDto.Description;
                    attachmentData.JobId = createDto.JobId;
                    attachmentData.UploadedBy = LoggedInUserName;
                    _attachmentService.Insert(attachmentData);

                    return Ok(new { Message = "Note and Attachments Created Successfully"});
                }
                else
                {
                    return BadRequest(new { Message = "Something went wrong" });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut(nameof(UpdateAttachment))]
        public IActionResult UpdateAttachment([FromForm] JobsAttachmentCreateDto updateDto)
        {
            try
            {
                if (updateDto.Id == null || updateDto.Id == 0)
                {
                    return BadRequest(new { Message = "Attachment Not Found" });
                }

                var existingAttachment = _attachmentService.Get((long)updateDto.Id);
                if (existingAttachment == null)
                {
                    return NotFound(new { Message = "Attachment not found" });
                }

                // Save the old file path before updating
                var oldFilePath = existingAttachment.FilePath;

                if (updateDto.File != null)
                {
                    var newFileResult = _fileService.AddFileAsync(updateDto.File).Result;

                    // Map file properties to existing attachment
                    existingAttachment.FileName = newFileResult.FileName;
                    existingAttachment.ActualName = newFileResult.ActualName;
                    existingAttachment.ContentType = newFileResult.ContentType;
                    existingAttachment.FilePath = newFileResult.FilePath;
                    existingAttachment.FileExtension = newFileResult.FileExtension;
                    existingAttachment.FileSize = newFileResult.FileSize;
                    existingAttachment.FileUrl = newFileResult.FileUrl;
                }
                existingAttachment.UploadedBy = LoggedInUserName;
                existingAttachment.Description = updateDto.Description;
                _attachmentService.Update(existingAttachment);

                // Delete the old file after the new file has been successfully added
                var res = _fileService.DeleteFileByIdAsync(oldFilePath).Result;

                return Ok(new { Message = "Attachment updated successfully" });
            }
            catch (Exception)
            {
                // Handle exceptions and potentially log them
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error" });
            }
        }

        [HttpDelete(nameof(DeleteAttachment))]
        public IActionResult DeleteAttachment(int attachmentId)
        {
            try
            {
                var existingAttachment = _attachmentService.Get(attachmentId);
                if (existingAttachment == null)
                {
                    return NotFound(new { Message = "Attachment not found" });
                }

                var deleteFileResult = _fileService.DeleteFileByIdAsync(existingAttachment.FilePath).Result;
                if (deleteFileResult)
                {
                    _attachmentService.Delete(existingAttachment);
                    return Ok(new { Message = "Attachment deleted successfully" });
                }
                else
                {
                    // Handle the case where file deletion fails
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to delete attachment file" });
                }
            }
            catch (Exception)
            {
                // Handle exceptions and potentially log them
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error" });
            }
        }

    }
}
