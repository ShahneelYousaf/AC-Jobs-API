using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AC_Contact_Services.BaseService
{
    public class AutomationService : IAutomationService
    {
        private readonly IRepository<AutomationEntity> _automationRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<ConditionEntity> _conditionRepository;
        private readonly IRepository<ActionEntity> _actionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public long LoggedInUserCompanyId;
        public long LoggedInUserId;

        public AutomationService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IRepository<AutomationEntity> automationRepository, IRepository<ConditionEntity> conditionRepository, IRepository<ActionEntity> actionRepository, IRepository<Job> jobRepository, IRepository<WorkOrder> workOrderRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _automationRepository = automationRepository;
            _conditionRepository = conditionRepository;
            _actionRepository = actionRepository;
            _jobRepository = jobRepository;
            _workOrderRepository = workOrderRepository;
        }

        public AutomationEntity CreateAutomation(AutomationEntity automation)
        {
            var obj = JsonConvert.SerializeObject(automation);
            automation.CreatedDate = DateTime.Now;
            automation.CompanyId = LoggedInUserCompanyId;
            automation.CreatedBy = LoggedInUserId;
            automation.IsDeleted = false;
            foreach (var condition in automation.Conditions)
            {
                condition.Id = 0;
                condition.AutomationEntity = automation;
                condition.CreatedDate = DateTime.Now;
                condition.CompanyId = LoggedInUserCompanyId;
                condition.CreatedBy = LoggedInUserId;
                condition.IsDeleted = false;
            }

            foreach (var action in automation.Actions)
            {
                action.Id = 0;
                action.AutomationEntity = automation;
                action.CreatedDate = DateTime.Now;
                action.CompanyId = LoggedInUserCompanyId;
                action.CreatedBy = LoggedInUserId;
                action.IsDeleted = false;
            }

            //automation.NextExecutionTime = CalculateNextExecutionTime(automation, DateTime.Now);
            _automationRepository.Insert(automation);
            return automation;
        }

        public void UpdateAutomation(AutomationEntity automation)
        {
            automation.ModifiedBy = LoggedInUserId;
            automation.ModifiedDate = DateTime.Now;

            // Get the existing automation entity from the repository
            var existingAutomation = _automationRepository.Get(automation.Id);
            var existingConditions = _conditionRepository.GetAllAsync().Where(x => x.AutomationEntityId == automation.Id).ToList();
            var existingActions = _actionRepository.GetAllAsync().Where(x => x.AutomationEntityId == automation.Id).ToList();



            // If the existing entity is found, update its properties
            if (existingAutomation != null)
            {
                existingAutomation.Name = automation.Name;
                existingAutomation.IsActive = automation.IsActive;
                existingAutomation.TriggerType = automation.TriggerType;
                existingAutomation.TriggerRecord = automation.TriggerRecord;
                existingAutomation.WhenEntityIs = automation.WhenEntityIs;
                existingAutomation.Duration = automation.Duration;
                existingAutomation.TimeUnit = automation.TimeUnit;
                existingAutomation.BeforeAfter = automation.BeforeAfter;
                existingAutomation.IsSpecificDay = automation.IsSpecificDay;
                existingAutomation.SelectedDay = automation.SelectedDay;
                existingAutomation.IsSpecificTime = automation.IsSpecificTime;
                existingAutomation.SelectedTime = automation.SelectedTime;
                existingAutomation.AutomationTriggerDateField = automation.AutomationTriggerDateField;
                existingAutomation.RequireAllConditionsToBeTrue = automation.RequireAllConditionsToBeTrue;
                existingAutomation.Conditions = automation.Conditions;
                existingAutomation.Actions = automation.Actions;

                // Identify conditions and actions to delete
                var conditionsToDelete = existingConditions.Where(ec => automation.Conditions.All(c => c.Id != ec.Id)).ToList();
                var actionsToDelete = existingActions.Where(ea => automation.Actions.All(a => a.Id != ea.Id)).ToList();

                // Delete conditions and actions that are not present in the provided automation
                foreach (var condition in conditionsToDelete)
                {
                    _conditionRepository.Delete(condition);
                }

                foreach (var action in actionsToDelete)
                {
                    _actionRepository.Delete(action);
                }

                foreach (var condition in automation.Conditions)
                {
                    if (condition.Id > 0)
                    {
                        condition.ModifiedDate = DateTime.Now;
                        condition.ModifiedBy = LoggedInUserId;
                    }
                    else
                    {
                        condition.Id = 0;
                        condition.AutomationEntity = automation;
                        condition.CreatedDate = DateTime.Now;
                        condition.CompanyId = LoggedInUserCompanyId;
                        condition.CreatedBy = LoggedInUserId;
                        condition.IsDeleted = false;
                    }

                }

                foreach (var action in automation.Actions)
                {
                    if (action.Id > 0)
                    {
                        action.ModifiedDate = DateTime.Now;
                        action.ModifiedBy = LoggedInUserId;
                    }
                    else
                    {
                        action.Id = 0;
                        action.AutomationEntity = automation;
                        action.CreatedDate = DateTime.Now;
                        action.CompanyId = LoggedInUserCompanyId;
                        action.CreatedBy = LoggedInUserId;
                        action.IsDeleted = false;
                    }
                }

                // Save the changes to the database
                //automation.NextExecutionTime = CalculateNextExecutionTime(automation, DateTime.Now);

                _automationRepository.Update(existingAutomation);
            }
        }

        private void UpdateAssociatedConditions(AutomationEntity existingAutomation, List<ConditionEntity> updatedConditions)
        {
            // Delete existing conditions not present in the updated list
            foreach (var existingCondition in existingAutomation.Conditions.ToList())
            {
                if (!updatedConditions.Any(c => c.Id == existingCondition.Id))
                {
                    _conditionRepository.Delete(existingCondition);
                    existingAutomation.Conditions.Remove(existingCondition);
                }
            }

            // Add new or update existing conditions
            foreach (var updatedCondition in updatedConditions)
            {
                if (updatedCondition.Id == 0)
                {
                    // Add new condition
                    updatedCondition.AutomationEntityId = (int)existingAutomation.Id;

                    _conditionRepository.Insert(updatedCondition);
                    existingAutomation.Conditions.Add(updatedCondition);
                }
                else
                {
                    // Update existing condition
                    var existingCondition = existingAutomation.Conditions.FirstOrDefault(c => c.Id == updatedCondition.Id);
                    if (existingCondition != null)
                    {
                        existingCondition.Field = updatedCondition.Field;
                        existingCondition.Value = updatedCondition.Value;
                        existingCondition.Comparison = updatedCondition.Comparison;
                        existingCondition.OnlyIfModified = updatedCondition.OnlyIfModified;
                        existingCondition.ModifiedBy = LoggedInUserId;
                        existingCondition.ModifiedDate = DateTime.Now;
                        existingCondition.Field = updatedCondition.Field;

                        UpdateCondition(existingCondition);
                    }
                }
            }
        }

        private void UpdateAssociatedActions(AutomationEntity existingAutomation, List<ActionEntity> updatedActions)
        {
            // Delete existing actions not present in the updated list
            foreach (var existingAction in existingAutomation.Actions.ToList())
            {
                if (!updatedActions.Any(a => a.Id == existingAction.Id))
                {
                    _actionRepository.Delete(existingAction);
                    existingAutomation.Actions.Remove(existingAction);
                }
            }

            // Add new or update existing actions
            foreach (var updatedAction in updatedActions)
            {
                if (updatedAction.Id == 0)
                {
                    updatedAction.AutomationEntityId = (int)existingAutomation.Id;
                    _actionRepository.Insert(updatedAction);
                    existingAutomation.Actions.Add(updatedAction);
                }
                else
                {
                    var existingAction = existingAutomation.Actions.FirstOrDefault(a => a.Id == updatedAction.Id);
                    if (existingAction != null)
                    {
                        updatedAction.AutomationEntityId = (int)existingAutomation.Id;
                        UpdateAction(updatedAction);
                    }
                }
            }
        }

        public void DeleteAutomation(int id)
        {
            var automation = _automationRepository.Get(id);
            if (automation != null)
            {
                var existingConditions = _conditionRepository.GetAllAsync().Where(x => x.AutomationEntityId == automation.Id).ToList();
                var existingActions = _actionRepository.GetAllAsync().Where(x => x.AutomationEntityId == automation.Id).ToList();

                if (existingConditions != null && existingConditions.Any())
                {
                    foreach (var condition in existingConditions)
                    {
                        _conditionRepository.Delete(condition);
                    }
                }
                if (existingActions != null && existingActions.Any())
                {
                    foreach (var action in existingActions)
                    {
                        _actionRepository.Delete(action);
                    }
                }
                _automationRepository.Delete(automation);
            }
        }


        public ActionEntity CreateAction(ActionEntity action)
        {
            action.CreatedDate = DateTime.Now;
            action.CompanyId = LoggedInUserCompanyId;
            action.CreatedBy = LoggedInUserId;
            action.IsDeleted = false;
            _actionRepository.Insert(action);
            return action;
        }

        public void UpdateAction(ActionEntity action)
        {
            action.ModifiedBy = LoggedInUserId;
            action.ModifiedDate = DateTime.Now;
            _actionRepository.Update(action);
        }

        public void DeleteAction(int id)
        {
            var action = _actionRepository.Get(id);
            if (action != null)
            {
                action.ModifiedBy = LoggedInUserId;
                action.ModifiedDate = DateTime.Now;
                action.IsDeleted = true;
                _actionRepository.Delete(action);
            }
        }

        public ConditionEntity CreateCondition(ConditionEntity condition)
        {
            condition.CreatedDate = DateTime.Now;
            condition.CompanyId = LoggedInUserCompanyId;
            condition.CreatedBy = LoggedInUserId;
            condition.IsDeleted = false;
            _conditionRepository.Insert(condition);
            return condition;
        }

        public void UpdateCondition(ConditionEntity condition)
        {
            condition.ModifiedBy = LoggedInUserId;
            condition.ModifiedDate = DateTime.Now;
            _conditionRepository.Update(condition);
        }

        public void DeleteCondition(int id)
        {
            var condition = _conditionRepository.Get(id);
            if (condition != null)
            {
                condition.ModifiedBy = LoggedInUserId;
                condition.ModifiedDate = DateTime.Now;
                condition.IsDeleted = true;
                _conditionRepository.Delete(condition);
            }
        }

        public AutomationEntity GetAutomationById(int id)
        {
            var automation = _automationRepository.GetAllAsync();
            var data = automation.Where(x => x.Id == id && x.IsDeleted != true).Include(x => x.Conditions).Include(x => x.Actions)
                                            .FirstOrDefault();
            return data;
        }

        public void ExecuteAutomationById(long id)
        {
            var automation = _automationRepository.GetAllAsync().Include(x => x.Conditions).Include(x => x.Actions)
                                            .FirstOrDefault(x => x.Id == id);

            if (automation != null)
            {
                // Implement logic to evaluate conditions and execute actions
                if (EvaluateConditions(automation.TriggerRecord, automation.Conditions, automation.RequireAllConditionsToBeTrue))
                {
                    ExecuteActions(automation.Actions);
                }
            }
        }

        private bool EvaluateConditions(string triggerRecord, List<ConditionEntity> conditions, bool requireAllConditionsToBeTrue)
        {
            //If false then any of the conditoion must be true
            if (!requireAllConditionsToBeTrue)
            {
                return true;
            }

            foreach (var condition in conditions)
            {
                var result = EvaluateCondition(triggerRecord, condition.Field, condition.Value, condition.Comparison);

                // If any condition evaluates to false, return false immediately
                if (!result)
                {
                    return false;
                }
            }

            // All conditions evaluated to true
            return true;
        }
        private bool EvaluateCondition(string triggerRecord, string leftOperand, string rightOperand, string condition)
        {
            switch (triggerRecord.ToLower())
            {
                case "job":
                    return EvaluateConditionForJob(leftOperand, rightOperand, condition);
                case "workorder":
                    return EvaluateConditionForWorkOrder(leftOperand, rightOperand, condition);
                case "contact":
                    return EvaluateConditionForContact(leftOperand, rightOperand, condition);
                default:
                    // Handle unknown triggerRecord
                    return false;
            }
        }

        private bool EvaluateConditionForJob(string leftOperand, string rightOperand, string condition)
        {
            var job = _jobRepository.GetAllAsync(); // Returns IQueryable
                                                    // Evaluate the condition based on the left and right operands
            switch (condition)
            {
                case "==":
                    return job.GetType().GetProperty(leftOperand)?.GetValue(job)?.ToString() == rightOperand;
                case "!=":
                    return job.GetType().GetProperty(leftOperand)?.GetValue(job)?.ToString() != rightOperand;
                default:
                    // Handle unknown condition
                    return false;
            }
        }

        private bool EvaluateConditionForWorkOrder(string leftOperand, string rightOperand, string condition)
        {
            // Implement similar logic for work orders
            return false;
        }

        private bool EvaluateConditionForContact(string leftOperand, string rightOperand, string condition)
        {
            // Implement similar logic for contacts
            return false;
        }
        private void ExecuteActions(List<ActionEntity> actions)
        {
            // Implement logic to execute actions
            // You may need to call external APIs, perform database operations, send emails, etc.
        }

        public List<AutomationEntity> GetAllAutomation()
        {
            var automation = _automationRepository.GetAllAsync();
            var data = automation.Where(x => x.IsDeleted != true && x.CreatedBy == LoggedInUserId).Include(x => x.Conditions).Include(x => x.Actions)
                                            .ToList();
            return data;
        }


        private DateTime? CalculateNextExecutionTime(AutomationEntity entity, DateTime refTime)
        {
            if (!entity.IsActive || entity.TriggerType.ToLower() != "timebase")
            {
                return null;
            }

            DateTime? nextExecutionTime = null;
            switch (entity.TimeUnit)
            {
                case "m":
                    nextExecutionTime = AdjustTime(refTime, entity.Duration.GetValueOrDefault(), entity.BeforeAfter, (d, dur) => d.AddMinutes(dur));
                    break;
                case "h":
                    nextExecutionTime = AdjustTime(refTime, entity.Duration.GetValueOrDefault(), entity.BeforeAfter, (d, dur) => d.AddHours(dur));
                    break;
                case "d":
                    nextExecutionTime = AdjustTime(refTime, entity.Duration.GetValueOrDefault(), entity.BeforeAfter, (d, dur) => d.AddDays(dur));
                    break;
                case "w":
                    nextExecutionTime = AdjustTime(refTime, entity.Duration.GetValueOrDefault() * 7, entity.BeforeAfter, (d, dur) => d.AddDays(dur));
                    break;
                case "mo":
                    nextExecutionTime = AdjustTime(refTime, entity.Duration.GetValueOrDefault(), entity.BeforeAfter, (d, dur) => d.AddMonths((int)dur));
                    break;
                case "y":
                    nextExecutionTime = AdjustTime(refTime, entity.Duration.GetValueOrDefault(), entity.BeforeAfter, (d, dur) => d.AddYears((int)dur));
                    break;
                default:
                    break;
            }

            return nextExecutionTime;
        }

        private DateTime AdjustTime(DateTime time, int duration, string? beforeAfter, Func<DateTime, double, DateTime> adjustFunc)
        {
            if (beforeAfter?.ToLower() == "before")
            {
                return adjustFunc(time, -duration);
            }
            else if (beforeAfter?.ToLower() == "after")
            {
                return adjustFunc(time, duration);
            }
            else
            {
                return DateTime.MinValue;
            }
        }

    }

}
