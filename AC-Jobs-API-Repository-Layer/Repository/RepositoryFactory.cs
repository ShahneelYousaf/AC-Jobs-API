using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;

namespace AC_Jobs_API_Repository_Layer.Repository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<AutomationEntity> _automationRepository;

        public RepositoryFactory(IRepository<Job> jobRepository, IRepository<WorkOrder> workOrderRepository, IRepository<AutomationEntity> automationRepository)
        {
            _jobRepository = jobRepository;
            _workOrderRepository = workOrderRepository;
            _automationRepository = automationRepository;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            if (typeof(TEntity) == typeof(Job))
            {
                return (IRepository<TEntity>)_jobRepository;
            }
            else if (typeof(TEntity) == typeof(WorkOrder))
            {
                return (IRepository<TEntity>)_workOrderRepository;
            }
            else if (typeof(TEntity) == typeof(AutomationEntity))
            {
                return (IRepository<TEntity>)_automationRepository;
            }
            // Add more cases for other entity types as needed

            throw new ArgumentException("Unsupported entity type");
        }
    }

}
