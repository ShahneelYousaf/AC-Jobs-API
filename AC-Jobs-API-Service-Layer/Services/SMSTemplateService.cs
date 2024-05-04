using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AC_Jobs_API_Service_Layer.Services
{
    public class SMSTemplateService : ICustomService<SMSTemplateEntity>
    {
        private readonly IRepository<SMSTemplateEntity> _smsTemplateRepository;
        public long LoggedInUserCompanyId;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SMSTemplateService(
            IRepository<SMSTemplateEntity> smsTemplateRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _smsTemplateRepository = smsTemplateRepository;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value);
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value);
        }

        public void Delete(SMSTemplateEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _smsTemplateRepository.Delete(entity);
                    _smsTemplateRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SMSTemplateEntity Get(long Id)
        {
            try
            {
                var obj = _smsTemplateRepository.Get(Id);
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

        public IEnumerable<SMSTemplateEntity> GetAll()
        {
            try
            {
                var listRepo = _smsTemplateRepository.GetAllAsync();
                var obj = listRepo.Where(x => x.IsDeleted != true ).ToList(); //x.UserId == LoggedInUserId && x.CompanyId == LoggedInUserCompanyId
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
                return _smsTemplateRepository.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SMSTemplateEntity Insert(SMSTemplateEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.CreatedBy = LoggedInUserId;
                    entity.CompanyId = LoggedInUserCompanyId;
                    entity.UserId = LoggedInUserId;
                    entity.CreatedDate = DateTime.Now;
                    _smsTemplateRepository.Insert(entity);
                    _smsTemplateRepository.SaveChanges();
                    return entity;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Remove(SMSTemplateEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    _smsTemplateRepository.Remove(entity);
                    _smsTemplateRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(SMSTemplateEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _smsTemplateRepository.Update(entity);
                    _smsTemplateRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<SMSTemplateEntity> GetAllAsync()
        {
            return _smsTemplateRepository.GetAllAsync().Where(x => x.IsDeleted != true);
        }

    }
}