using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhotosController : ControllerBase
    {
        private readonly ICustomService<PhotosEntity> _photosService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        public long LoggedInUserCompanyId;
        public string LoggedInUserName;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PhotosController(ApplicationDbContext applicationDbContext, IMapper mapper, ICustomService<PhotosEntity> photosService, IHttpContextAccessor httpContextAccessor, IFileService fileService)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _photosService = photosService;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserName = user.FindFirst("name").Value.ToString();
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
        }

        [HttpGet(nameof(GetPhotoById))]
        public IActionResult GetPhotoById(int Id)
        {
            var obj = _photosService.Get(Id);
            if (obj == null)
            {
                return BadRequest(new { Message = "Photo Not Found" });
            }
            else
            {
                _photosService.GetAllAsync().Where(x => x.Id == Id).FirstOrDefault();
                return Ok(new { Message = "Operation Successful", data = obj });
            }
        }

        [HttpGet(nameof(GetPhotosByJobId))]
        public IActionResult GetPhotosByJobId(int Id)
        {
            var obj = _photosService.GetAllAsync().Where(x => x.JobId == Id).ToList();
            if (obj == null)
            {
                return BadRequest(new { Message = "Photo Not Found" });
            }
            else
            {
                return Ok(new { Message = "Operation Successful", data = obj });
            }
        }

        [HttpGet(nameof(GetAllPhotos))]
        public IActionResult GetAllPhotos()
        {
            var obj = _photosService.GetAll();
            if (obj == null)
            {
                return BadRequest(new { Message = "Photo Not Found" });
            }
            else
            {
                return Ok(new { Message = "Operation Successful", data = obj });
            }
        }
        
        [HttpPost(nameof(DownloadFiles))]
        public async Task<IActionResult> DownloadFiles(List<string> FilePaths)
        {
            if (FilePaths == null || FilePaths.Count == 0)
            {
                // Handle the case where no files are provided.
                return BadRequest(new { message = "No files to download." });
            }
            else
            {
                return await _fileService.DownloadFiles(FilePaths);
            }
        }

        [HttpPost(nameof(AddJobPhoto))]
        public IActionResult AddJobPhoto([FromForm] JobsAttachmentCreateDto createDto)
        {
            try
            {
                if (createDto != null)
                {
                    var fileResult = _fileService.AddFileAsync(createDto.File).Result;
                    var attachmentData = _mapper.Map<PhotosEntity>(fileResult);
                    attachmentData.JobId = createDto.JobId;
                    attachmentData.Description = createDto.Description;
                    attachmentData.UploadedBy = LoggedInUserName;
                    attachmentData.CreatedDate = DateTime.Now;
                    _photosService.Insert(attachmentData);

                    return Ok(new { Message = "Note and Photos Created Successfully" });
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

        [HttpPut(nameof(UpdateJobPhoto))]
        public IActionResult UpdateJobPhoto([FromForm] JobsAttachmentCreateDto updateDto)
        {
            try
            {
                if (updateDto.Id == null || updateDto.Id == 0)
                {
                    return BadRequest(new { Message = "Photo Not Found" });
                }

                var existingPhoto = _photosService.Get((long)updateDto.Id);
                if (existingPhoto == null)
                {
                    return NotFound(new { Message = "Photo not found" });
                }

                // Save the old file path before updating
                var oldFilePath = existingPhoto.FilePath;

                if (updateDto.File != null)
                {
                    var newFileResult = _fileService.AddFileAsync(updateDto.File).Result;

                    // Map file properties to existing attachment
                    existingPhoto.FileName = newFileResult.FileName;
                    existingPhoto.ActualName = newFileResult.ActualName;
                    existingPhoto.ContentType = newFileResult.ContentType;
                    existingPhoto.FilePath = newFileResult.FilePath;
                    existingPhoto.FileExtension = newFileResult.FileExtension;
                    existingPhoto.FileSize = newFileResult.FileSize;
                    existingPhoto.FileUrl = newFileResult.FileUrl;
                }

                existingPhoto.Description = updateDto.Description;
                existingPhoto.UploadedBy = LoggedInUserName;
                _photosService.Update(existingPhoto);

                // Delete the old file after the new file has been successfully added
                var res = _fileService.DeleteFileByIdAsync(oldFilePath).Result;

                return Ok(new { Message = "Photo updated successfully" });
            }
            catch (Exception)
            {
                // Handle exceptions and potentially log them
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error" });
            }
        }

        [HttpDelete(nameof(DeleteJobPhoto))]
        public IActionResult DeleteJobPhoto(int attachmentId)
        {
            try
            {
                var existingPhoto = _photosService.Get(attachmentId);
                if (existingPhoto == null)
                {
                    return NotFound(new { Message = "Photo not found" });
                }

                var deleteFileResult = _fileService.DeleteFileByIdAsync(existingPhoto.FilePath).Result;
                if (deleteFileResult)
                {
                    _photosService.Delete(existingPhoto);
                    return Ok(new { Message = "Photo deleted successfully" });
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
