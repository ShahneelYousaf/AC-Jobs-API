using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Domian_Layer.Models.Contact;
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

namespace AC_Jobs_API_Service_Layer.Services.Contacts
{
    public class ContactStatusEntityService : IContactCustomService<ContactStatusEntity>
    {
        private readonly IContactRepository<ContactStatusEntity> _contactRepository;
        public long LoggedInUserCompanyId;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ContactStatusEntityService(
            IContactRepository<ContactStatusEntity> contactRepository,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _contactRepository = contactRepository;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
        }

        public int CountAll()
        {
            try
            {
                return _contactRepository.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Delete(ContactStatusEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _contactRepository.Delete(entity);
                    _contactRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ContactStatusEntity Get(long Id)
        {
            try
            {
                var obj = _contactRepository.Get(Id);
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
        public IEnumerable<ContactStatusEntity> GetAll()
        {
            try
            {
                var CompanyId = LoggedInUserCompanyId;
                var listRepo = _contactRepository.GetAllAsync();

                var obj = listRepo.Where(x => x.CompanyId == CompanyId).ToList();
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
        public ContactStatusEntity Insert(ContactStatusEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.CreatedBy = LoggedInUserId;
                    entity.CreatedDate = DateTime.Now;
                    entity.CompanyId = LoggedInUserCompanyId;
                    _contactRepository.Insert(entity);
                    _contactRepository.SaveChanges();
                    return entity;

                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Remove(ContactStatusEntity entity)
        {
            try
            {
                if (entity != null)
                {

                    _contactRepository.Remove(entity);
                    _contactRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Update(ContactStatusEntity entity)
        {
            try
            {
                if (entity != null)
                {

                    var data = _contactRepository.Get(entity.Id);
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    entity.CompanyId = data.CompanyId;
                    _contactRepository.Update(entity);
                    _contactRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IQueryable<ContactStatusEntity> GetAllAsync()
        {
            return _contactRepository.GetAllAsync();
        }
    }

}
