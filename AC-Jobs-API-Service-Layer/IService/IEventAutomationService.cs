using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Domian_Layer.Models.Contact;

namespace AC_Jobs_API_Service_Layer.IService
{
    public interface IEventAutomationService
    {
        DateTime? CalculateNextExecutionTime(AutomationEntity entity, DateTime refTime);
        void ProcessEventBasedAutomation<TEntity>(string eventType, string whenEntityIs, long Id) where TEntity : BaseEntity;
        void ProcessContactEventBasedAutomation<TEntity>(string eventType, string whenEntityIs, long Id) where TEntity : ContactBaseEntity;
    }
}
