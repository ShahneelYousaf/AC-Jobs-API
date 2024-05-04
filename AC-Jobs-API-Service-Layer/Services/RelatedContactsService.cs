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

namespace AC_Jobs_API_Service_Layer.Services
{
    public class RelatedContactsService : ICustomService<RelatedContacts>
    {
        private readonly IRepository<RelatedContacts> _statusRepository;
        private readonly IMapper _mapper;
        public long LoggedInUserCompanyId;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RelatedContactsService(
             IRepository<RelatedContacts> statusRepository,
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
        public void Delete(RelatedContacts entity)
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

        public RelatedContacts Get(long Id)
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

        public IEnumerable<RelatedContacts> GetAll()
        {
            try
            {
                var obj = _statusRepository.GetAll();
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

        public RelatedContacts Insert(RelatedContacts entity)
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

        public void Remove(RelatedContacts entity)
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

        public void Update(RelatedContacts entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _statusRepository.Update(entity);
                    _statusRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<RelatedContacts> GetAllAsync()
        {
           return _statusRepository.GetAllAsync();
        }
    }

}