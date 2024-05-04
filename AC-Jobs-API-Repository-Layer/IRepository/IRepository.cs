using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Repository_Layer.IRepository
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        IQueryable<T> GetAllAsync();
        T Get(long Id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Remove(T entity);
        void SaveChanges();
        int Count();
    } 

}
