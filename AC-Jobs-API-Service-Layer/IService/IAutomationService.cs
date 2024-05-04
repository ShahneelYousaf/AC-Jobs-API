

using AC_Jobs_API_Domian_Layer.Models;

namespace AC_Jobs_API_Service_Layer.IService
{
    public interface IAutomationService
    {
        ActionEntity CreateAction(ActionEntity action);
        AutomationEntity CreateAutomation(AutomationEntity automation);
        ConditionEntity CreateCondition(ConditionEntity condition);
        void DeleteAction(int id);
        void DeleteAutomation(int id);
        void DeleteCondition(int id);
        void ExecuteAutomationById(long id);
        AutomationEntity GetAutomationById(int id);
        List<AutomationEntity> GetAllAutomation();
        void UpdateAction(ActionEntity action);
        void UpdateAutomation(AutomationEntity automation);
        void UpdateCondition(ConditionEntity condition);
    }
}
