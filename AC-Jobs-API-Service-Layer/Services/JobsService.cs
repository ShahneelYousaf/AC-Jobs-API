using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AC_Job_API_Service_Layer.Services
{
    public class JobService : ICustomService<Job>
    {
        private readonly IRepository<Job> _statusRepository;
        private readonly IMapper _mapper;
        public long LoggedInUserCompanyId;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JobService(
            IRepository<Job> statusRepository,
            IRepository<RelatedContacts> relatedContacts,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _statusRepository = statusRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
        }

        public int CountAll()
        {
            try
            {
                return _statusRepository.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Delete(Job entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _statusRepository.Delete(entity);
                    _statusRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Job Get(long Id)
        {
            try
            {
                var obj = _statusRepository.Get(Id);
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

        public IEnumerable<Job> GetAll()
        {
            try
            {
                var CompanyId = LoggedInUserCompanyId;
                var listRepo = _statusRepository.GetAllAsync();

                var obj = listRepo.Where(x=>x.CompanyId== CompanyId).ToList();
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

        public Job Insert(Job entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.CreatedBy = LoggedInUserId;
                    entity.CreatedDate = DateTime.Now;
                    entity.CompanyId = LoggedInUserCompanyId;
                    entity.UserId = LoggedInUserId;
                    _statusRepository.Insert(entity);
                    _statusRepository.SaveChanges();
                    return entity;

                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Remove(Job entity)
        {
            try
            {
                if (entity != null)
                {

                    _statusRepository.Remove(entity);
                    _statusRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(Job entity)
        {
            try
            {
                if (entity != null)
                {
                   
                    var data = _statusRepository.Get(entity.Id);
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    entity.CompanyId = data.CompanyId;
                    _statusRepository.Update(entity);
                    _statusRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Job> SearchJobs(string name)
        {
            try
            {
                var CompanyId = LoggedInUserCompanyId;
                var listRepo = _statusRepository.GetAllAsync();
                var obj = listRepo.Where(x => x.CompanyId == CompanyId && x.Name.Contains(name) && x.IsDeleted==false).ToList();
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<Job> GetAllAsync()
        {
            return _statusRepository.GetAllAsync();
        }
    }

}