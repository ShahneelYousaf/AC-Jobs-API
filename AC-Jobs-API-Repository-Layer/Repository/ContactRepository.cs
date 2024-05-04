using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Domian_Layer.Models.Contact;
using AC_Jobs_API_Repository_Layer.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AC_Jobs_API_Repository_Layer.Repository
{
    public class ContactRepository<T> : IContactRepository<T> where T : ContactBaseEntity
    {
        #region property
        private readonly ContactDbContext _contactDbContext;
        private DbSet<T> entities;
        #endregion
        #region Constructor
        public ContactRepository(ContactDbContext contactDbContext)
        {
            _contactDbContext = contactDbContext;
            entities = _contactDbContext.Set<T>();
        }
        #endregion
        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.IsDeleted = true;
            entities.Update(entity);
            _contactDbContext.SaveChanges();
        }
        public T Get(long Id)
        {
            return entities.AsNoTracking().SingleOrDefault(c => c.Id == Id);
        }
        public IEnumerable<T> GetAll()
        {
            return entities.AsNoTracking().ToList();
        }
        public void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            _contactDbContext.SaveChanges();
        }
        public int Count()
        {
            return entities.Count();
        }
        public void Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.IsDeleted = true;
            entities.Update(entity);
            _contactDbContext.SaveChanges();
        }
        public void SaveChanges()
        {
            _contactDbContext.SaveChanges();
        }
        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            _contactDbContext.SaveChanges();
        }

        public IQueryable<T> GetAllAsync()
        {
            return entities.AsNoTracking();
        }
    }

}