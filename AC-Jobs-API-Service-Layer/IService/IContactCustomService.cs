namespace AC_Jobs_API_Service_Layer.IService
{
    public interface IContactCustomService<T> where T : class
    {
        IEnumerable<T> GetAll();
        IQueryable<T> GetAllAsync();
        T Get(long Id);
        T Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Remove(T entity);
        int CountAll();
    }
}
