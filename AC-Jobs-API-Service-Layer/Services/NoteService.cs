using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Service_Layer.Services
{
    public class NoteService : ICustomService<Note>
    {
        private readonly IRepository<Note> _noteRepository;
        private readonly IRepository<PhotosEntity> _attachmentRepository;
        public long LoggedInUserCompanyId;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NoteService(
            IRepository<Note> noteRepository,
            IRepository<PhotosEntity> attachmentRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _noteRepository = noteRepository;
            _attachmentRepository = attachmentRepository;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
        }

        public async void Delete(Note entity)
        {
            try
            {
                if (entity != null)
                {
                    // Delete associated attachments
                    var attachmentsRep = _attachmentRepository.GetAllAsync();
                    var attachments = attachmentsRep.Where(a => a.NoteId == entity.Id).ToList();
                    foreach (var attachment in attachments)
                    {
                       _attachmentRepository.Delete(attachment);
                    }

                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _noteRepository.Delete(entity);
                    _noteRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Note Get(long Id)
        {
            try
            {
                var obj = _noteRepository.Get(Id);
                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Note> GetAll()
        {
            try
            {
                var obj = _noteRepository.GetAll().Where(x=> x.CompanyId == LoggedInUserCompanyId && x.CreatedBy == LoggedInUserId).ToList();
                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int CountAll()
        {
            try
            {
                return _noteRepository.GetAll().Where(x => x.CompanyId ==  LoggedInUserCompanyId).Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Note Insert(Note entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.CreatedBy = LoggedInUserId;
                    entity.CreatedDate = DateTime.Now;
                    entity.CompanyId = LoggedInUserCompanyId;
                    entity.UserId = LoggedInUserId;
                    _noteRepository.Insert(entity);
                    _noteRepository.SaveChanges();
                    return entity;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Remove(Note entity)
        {
            try
            {
                if (entity != null)
                {
                    _noteRepository.Remove(entity);
                    _noteRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(Note entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _noteRepository.Update(entity);
                    _noteRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddAttachmentToNoteAsync(long noteId, PhotosEntity attachment)
        {
            try
            {
                var note = _noteRepository.Get(noteId);
                if (note != null)
                {
                    attachment.NoteId = noteId;
                   _attachmentRepository.Insert(attachment);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<PhotosEntity>> GetAttachmentsForNote(long noteId)
        {
            try
            {
                var result = _attachmentRepository.GetAllAsync();
                return result.Where(a => a.NoteId == noteId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<Note> GetAllForJob(long jobId)
        {
            try
            {
                return _noteRepository.GetAll().Where(n => n.JobId == jobId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Note> GetAllForWorkOrder(long workOrderId)
        {
            try
            {
                return _noteRepository.GetAllAsync().Where(n => n.WorkOrderId == workOrderId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void DeleteAllForJob(long jobId)
        {
            try
            {
                var notesToDelete = _noteRepository.GetAll().Where(n => n.JobId == jobId);

                foreach (var note in notesToDelete)
                {
                    var result = _attachmentRepository.GetAllAsync();
                    var attachments = result.Where(a => a.NoteId == note.Id);
                    foreach (var attachment in attachments)
                    {
                       _attachmentRepository.Remove(attachment);
                    }

                    _noteRepository.Delete(note);
                }

                _noteRepository.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void DeleteAllForWorkOrder(long workOrderId)
        {
            try
            {
                var notesToDelete = _noteRepository.GetAll().Where(n => n.WorkOrderId == workOrderId);

                foreach (var note in notesToDelete)
                {
                    // Delete associated attachments
                    var result = _attachmentRepository.GetAllAsync();
                    var attachments = result.Where(a => a.NoteId == note.Id);
                    foreach (var attachment in attachments)
                    {
                       _attachmentRepository.Delete(attachment);
                    }
                    _noteRepository.Delete(note);
                }

                _noteRepository.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<Note> GetAllAsync()
        {
            return _noteRepository.GetAllAsync();
        }
    }
}
