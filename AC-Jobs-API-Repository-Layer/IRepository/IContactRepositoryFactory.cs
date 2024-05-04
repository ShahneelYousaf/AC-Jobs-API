using AC_Jobs_API_Domian_Layer.Models.Contact;

namespace AC_Jobs_API_Repository_Layer.IRepository
{
    public interface IContactRepositoryFactory
    {
        IContactRepository<TEntity> GetRepository<TEntity>() where TEntity : ContactBaseEntity;
    }

}
