using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AC_Jobs_API_Service_Layer.Services
{
    public class EmailTemplateService : ICustomService<EmailTemplateEntity>
    {
        private readonly IRepository<EmailTemplateEntity> _emailTemplateRepository;
        public long LoggedInUserCompanyId;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailTemplateService(
            IRepository<EmailTemplateEntity> emailTemplateRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _emailTemplateRepository = emailTemplateRepository;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value);
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value);
        }

        public void Delete(EmailTemplateEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _emailTemplateRepository.Delete(entity);
                    _emailTemplateRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EmailTemplateEntity Get(long Id)
        {
            try
            {
                var obj = _emailTemplateRepository.Get(Id);
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

        public IEnumerable<EmailTemplateEntity> GetAll()
        {
            try
            {
                var listRepo = _emailTemplateRepository.GetAllAsync();
                var obj = listRepo.Where(x => x.IsDeleted != true).ToList();
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
                return _emailTemplateRepository.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EmailTemplateEntity Insert(EmailTemplateEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.CreatedBy = LoggedInUserId;
                    entity.CompanyId = LoggedInUserCompanyId;
                    entity.UserId = LoggedInUserId;
                    entity.CreatedDate = DateTime.Now;
                    _emailTemplateRepository.Insert(entity);
                    _emailTemplateRepository.SaveChanges();
                    return entity;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Remove(EmailTemplateEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    _emailTemplateRepository.Remove(entity);
                    _emailTemplateRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(EmailTemplateEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _emailTemplateRepository.Update(entity);
                    _emailTemplateRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<EmailTemplateEntity> GetAllAsync()
        {
            return _emailTemplateRepository.GetAllAsync().Where(x => x.IsDeleted != true);
        }

    }
}