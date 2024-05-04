using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AC_Jobs_API_Service_Layer.Services;
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
    public class NotesController : ControllerBase
    {
        private readonly ICustomService<Note> _noteService;
        private readonly ICustomService<PhotosEntity> _photosService;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        public long LoggedInUserCompanyId;
        public string LoggedInUserName;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NotesController(ICustomService<Note> noteService, ApplicationDbContext applicationDbContext, IMapper mapper, ICustomService<PhotosEntity> photosService, IHttpContextAccessor httpContextAccessor, IFileService fileService)
        {
            _noteService = noteService;
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

        [HttpGet(nameof(GetNoteById))]
        public IActionResult GetNoteById(int Id)
        {
            var obj = _noteService.Get(Id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                    var data = _mapper.Map<DTOGetNote>(obj);
                    var attachments = _photosService.GetAllAsync().Where(x => x.NoteId == data.Id).ToList();
                    var attachmentsDto = _mapper.Map<IEnumerable<DtoNotePhoto>>(attachments);
                    data.Attachments = attachmentsDto.ToList();
                return Ok(data);
            }
        }

        [HttpGet(nameof(GetAllNotes))]
        public IActionResult GetAllNotes()
        {
            try
            {
                var notes = _noteService.GetAll();

                if (notes == null || !notes.Any())
                {
                    return NotFound("No notes found.");
                }

                var notesDto = _mapper.Map<IEnumerable<DTOGetNote>>(notes);
                foreach (var noteDto in notesDto)
                {
                    var attachments = _photosService.GetAllAsync().Where(x=> x.NoteId == noteDto.Id).ToList();
                    var attachmentsDto = _mapper.Map<IEnumerable<DtoNotePhoto>>(attachments);
                    noteDto.Attachments = attachmentsDto.ToList();
                }

                return Ok(notesDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost(nameof(CreateNote))]
        public IActionResult CreateNote([FromForm]DTOCreateNote note)
        {
            try
            {
                if (note != null)
                {
                    var noteData = _mapper.Map<Note>(note);

                    noteData.CreatedBy = LoggedInUserId;
                    noteData.CompanyId = LoggedInUserCompanyId;
                    noteData.UserId = LoggedInUserId;
                    noteData.CreatedDate = DateTime.Now;
                    noteData.ModifiedDate = DateTime.Now;
                    _noteService.Insert(noteData);
                    var files = _fileService.AddFilesAsync(note.Attachments).Result;
                    foreach (var item in files)
                    {
                        var photo = _mapper.Map<PhotosEntity>(item);
                        photo.CreatedBy = LoggedInUserId;
                        photo.CompanyId = LoggedInUserCompanyId;
                        photo.UserId = LoggedInUserId;
                        photo.NoteId = noteData.Id;
                        photo.Description = photo.Description;
                        photo.UploadedBy = LoggedInUserName;
                        _photosService.Insert(photo);
                    }
                    return Ok("Note and Attachments Created Successfully");
                }
                else
                {
                    return BadRequest("Something went wrong");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut(nameof(UpdateNote))]
        public IActionResult UpdateNote([FromForm] DTOUpdateNote updatedNote)
        {
            try
            {
                if (updatedNote != null)
                {
                    var existingNote = _noteService.Get(updatedNote.Id);
                    if (existingNote == null)
                    {
                        return NotFound($"Note with ID {updatedNote.Id} not found.");
                    }

                    existingNote.Type = updatedNote.Type;
                    existingNote.Content = updatedNote.Content;
                    existingNote.ModifiedDate = DateTime.Now;
                    _noteService.Update(existingNote);
                    if (updatedNote.Attachments != null && updatedNote.Attachments.Count > 0)
                    {
                        var existingPhotos = _photosService.GetAllAsync().Where(x => x.NoteId == updatedNote.Id).ToList();
                        foreach (var existingPhoto in existingPhotos)
                        {
                            if (!updatedNote.Attachments.Any(a => a.FileName == existingPhoto.FileName))
                            {
                                _photosService.Delete(existingPhoto);
                                _fileService.DeleteFileByIdAsync(existingPhoto.FilePath);
                            }
                        }

                        var newFiles = _fileService.AddFilesAsync(updatedNote.Attachments).Result;
                        foreach (var item in newFiles)
                        {
                            var photo = _mapper.Map<PhotosEntity>(item);
                            photo.CreatedBy = LoggedInUserId;
                            photo.CompanyId = LoggedInUserCompanyId;
                            photo.UserId = LoggedInUserId;
                            photo.NoteId = updatedNote.Id;
                            photo.Description = photo.Description;
                            photo.UploadedBy = LoggedInUserName;
                            _photosService.Insert(photo);
                        }
                    }

                    return Ok("Note and Attachments Updated Successfully");
                }
                else
                {
                    return BadRequest("Invalid data provided for updating the note.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (log or rethrow)
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }




        private void HandleAttachments(List<PhotosEntity> existingAttachments, List<PhotosEntity> newAttachments)
        {
            var attachmentsToDelete = existingAttachments.Where(existing => !newAttachments.Any(newAttachment => newAttachment.Id == existing.Id)).ToList();
            var attachmentsToAdd = newAttachments.Where(newAttachment => newAttachment.Id == 0 || !existingAttachments.Any(existing => existing.Id == newAttachment.Id)).ToList();
            var attachmentsToUpdate = newAttachments.Where(newAttachment => existingAttachments.Any(existing => existing.Id == newAttachment.Id)).ToList();
            foreach (var attachmentToDelete in attachmentsToDelete)
            {
                _photosService.Delete(attachmentToDelete);
            }
            foreach (var attachmentToAdd in attachmentsToAdd)
            {
                _photosService.Insert(attachmentToAdd);
            }

            foreach (var attachmentToUpdate in attachmentsToUpdate)
            {
                attachmentToUpdate.ModifiedBy = LoggedInUserId;
                attachmentToUpdate.ModifiedDate = DateTime.Now;
                _photosService.Update(attachmentToUpdate);
            }
        }


        [HttpDelete(nameof(DeleteNote))]
        public IActionResult DeleteNote(long Id)
        {
            try
            {
                if (Id != null && Id != 0)
                {
                    var note = _noteService.Get(Id);
                    _noteService.Delete(note);

                    var existingAttachments = _photosService.GetAll().Where(x=> x.NoteId == Id);
                    foreach (var attachment in existingAttachments)
                    {
                        _photosService.Delete(attachment);
                        var res = _fileService.DeleteFileByIdAsync(attachment?.FilePath).Result;
                    }
                    return Ok("Note and Attachments Deleted Successfully");
                }
                else
                {
                    return BadRequest("Invalid input");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}
