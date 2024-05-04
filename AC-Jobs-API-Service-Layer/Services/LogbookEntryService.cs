using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AC_Jobs_API_Service_Layer.Services
{
    public class LogbookEntryService : ICustomService<LogbookEntry>
    {
        private readonly IRepository<LogbookEntry> _logbookRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public long LoggedInUserId;
        public long LoggedInUserCompanyId;
        public string LoggedInUserName;

        public LogbookEntryService(
            IRepository<LogbookEntry> logbookRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _logbookRepository = logbookRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserName = user.FindFirst("name").Value.ToString();


        }

        public int CountAll()
        {
            try
            {
                return _logbookRepository.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(LogbookEntry entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _logbookRepository.Delete(entity);
                    _logbookRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LogbookEntry Get(long Id)
        {
            try
            {
                var obj = _logbookRepository.Get(Id);
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<LogbookEntry> GetAll()
        {
            try
            {
                var obj = _logbookRepository.GetAll().Where(x=> x.CompanyId == LoggedInUserCompanyId).ToList();
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LogbookEntry Insert(LogbookEntry entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.CreatedBy = LoggedInUserId;
                    entity.CompanyId = LoggedInUserCompanyId;
                    entity.PerformedBy = LoggedInUserName;
                    entity.CreatedDate = DateTime.Now;
                    _logbookRepository.Insert(entity);
                    _logbookRepository.SaveChanges();
                    return entity;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Remove(LogbookEntry entity)
        {
            try
            {
                if (entity != null)
                {
                    _logbookRepository.Remove(entity);
                    _logbookRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(LogbookEntry entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    entity.CompanyId = LoggedInUserCompanyId;
                    entity.PerformedBy = LoggedInUserName;
                    _logbookRepository.Update(entity);
                    _logbookRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<LogbookEntry> GetAllAsync()
        {
            return _logbookRepository.GetAllAsync();
        }
    }
}
