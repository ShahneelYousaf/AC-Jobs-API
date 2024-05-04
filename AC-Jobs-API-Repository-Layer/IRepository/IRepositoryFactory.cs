using AC_Jobs_API_Domian_Layer.Models;

namespace AC_Jobs_API_Repository_Layer.IRepository
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
    }    

}
