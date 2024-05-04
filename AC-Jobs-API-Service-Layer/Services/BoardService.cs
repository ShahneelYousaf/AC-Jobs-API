﻿using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AC_Jobs_API_Service_Layer.Services
{
    public class BoardService : ICustomService<BoardEntity>
    {
        private readonly IRepository<BoardEntity> _attachmentRepository;
        public long LoggedInUserCompanyId;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BoardService(
            IRepository<BoardEntity> attachmentRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _attachmentRepository = attachmentRepository;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
        }

        public void Delete(BoardEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _attachmentRepository.Delete(entity);
                    _attachmentRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public BoardEntity Get(long Id)
        {
            try
            {
                var obj = _attachmentRepository.Get(Id);
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

        public IEnumerable<BoardEntity> GetAll()
        {
            try
            {
                var listRepo = _attachmentRepository.GetAllAsync();
                var obj = listRepo.Where(x => x.UserId == LoggedInUserId && x.CompanyId == LoggedInUserCompanyId).ToList();
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
                return _attachmentRepository.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public BoardEntity Insert(BoardEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.CreatedBy = LoggedInUserId;
                    entity.CompanyId = LoggedInUserCompanyId;
                    entity.UserId = LoggedInUserId;
                    entity.CreatedDate = DateTime.Now;
                    _attachmentRepository.Insert(entity);
                    _attachmentRepository.SaveChanges();
                    return entity;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Remove(BoardEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    _attachmentRepository.Remove(entity);
                    _attachmentRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(BoardEntity entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _attachmentRepository.Update(entity);
                    _attachmentRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<BoardEntity> GetAllAsync()
        {
            return _attachmentRepository.GetAllAsync().Where(x => x.IsDeleted != true);
        }
    }

}
