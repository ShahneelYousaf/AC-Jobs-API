using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AC_Jobs_API_Service_Layer.Services
{
    public class LineItemService : ICustomService<LineItem>
    {
        private readonly IRepository<LineItem> _lineItemRepository;
        public long LoggedInUserCompanyId;
        public long LoggedInUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LineItemService(
            IRepository<LineItem> lineItemRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _lineItemRepository = lineItemRepository;
            _httpContextAccessor = httpContextAccessor;
            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
        }

        public void Delete(LineItem entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _lineItemRepository.Delete(entity);
                    _lineItemRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LineItem Get(long Id)
        {
            try
            {
                var obj = _lineItemRepository.Get(Id);
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

        public IEnumerable<LineItem> GetAll()
        {
            try
            {
                var obj = _lineItemRepository.GetAll();
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
                return _lineItemRepository.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LineItem Insert(LineItem entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.CreatedBy = LoggedInUserId;
                    entity.CreatedDate = DateTime.Now;
                    entity.CompanyId = LoggedInUserCompanyId;
                    entity.UserId = LoggedInUserId;
                    _lineItemRepository.Insert(entity);
                    _lineItemRepository.SaveChanges();
                    return entity;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Remove(LineItem entity)
        {
            try
            {
                if (entity != null)
                {
                    _lineItemRepository.Remove(entity);
                    _lineItemRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(LineItem entity)
        {
            try
            {
                if (entity != null)
                {
                    entity.ModifiedBy = LoggedInUserId;
                    entity.ModifiedDate = DateTime.Now;
                    _lineItemRepository.Update(entity);
                    _lineItemRepository.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<LineItem> GetAllForJob(long jobId)
        {
            try
            {
                return _lineItemRepository.GetAll().Where(li => li.JobId == jobId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<LineItem> GetAllForWorkOrder(long workOrderId)
        {
            try
            {
                return _lineItemRepository.GetAll().Where(li => li.WorkOrderId == workOrderId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteAllForJob(long jobId)
        {
            try
            {
                var lineItemsToDelete = _lineItemRepository.GetAll().Where(li => li.JobId == jobId);

                foreach (var lineItem in lineItemsToDelete)
                {
                    _lineItemRepository.Delete(lineItem);
                }

                _lineItemRepository.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteAllForWorkOrder(long workOrderId)
        {
            try
            {
                var lineItemsToDelete = _lineItemRepository.GetAll().Where(li => li.WorkOrderId == workOrderId);

                foreach (var lineItem in lineItemsToDelete)
                {
                    _lineItemRepository.Delete(lineItem);
                }

                _lineItemRepository.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<LineItem> GetAllAsync()
        {
            return _lineItemRepository.GetAllAsync();
        }
    }
}
