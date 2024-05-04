using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Domian_Layer.Models.Contact;
using AC_Jobs_API_Repository_Layer.IRepository;
using AC_Jobs_API_Service_Layer.IService;
using AC_Jobs_API_Service_Layer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace AC_Contact_Services.BaseService
{
    public class EventAutomationService : IEventAutomationService
    {
        private readonly IRepository<AutomationEntity> _automationRepository;
        private readonly ICustomService<WorkOrder> _workOrderService;
        private readonly ICustomService<TeamMembers> _teamMemberService;
        private readonly ICustomService<RelatedContacts> _relatedContacts;
        private readonly ICustomService<SubContractor> _subContractorService;
        private readonly ICustomService<Job> _jobService;
        private readonly ICustomService<PhotosEntity> _photosService;
        private readonly ICustomService<Note> _noteService;
        private readonly ICustomService<LineItem> _lineItemService;


        private readonly IContactCustomService<ContactsRelatedContactEntity> _contactsRelatedContactService;
        private readonly IContactCustomService<ContactTeamMemberEntity> _contactsTeamMemberService;
        private readonly IContactCustomService<ContactEntity> _contactsService;


        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IContactRepositoryFactory _contactRepositoryFactory;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public long LoggedInUserCompanyId;
        public long LoggedInUserId;

        public EventAutomationService(IRepository<AutomationEntity> automationRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IRepositoryFactory repositoryFactory,
            IEmailService emailService,
            ICustomService<WorkOrder> workOrderService,
            ICustomService<TeamMembers> teamMemberService,
            ICustomService<SubContractor> subContractorService,
            ICustomService<PhotosEntity> photosService,
            ICustomService<Note> noteService,
            ICustomService<LineItem> lineItemService,
            ICustomService<RelatedContacts> relatedContacts,
            ICustomService<Job> jobService,
            IContactCustomService<ContactsRelatedContactEntity> contactsRelatedContactService,
            IContactCustomService<ContactTeamMemberEntity> contactsTeamMemberService,
            IContactCustomService<ContactEntity> contactsService,
            IContactRepositoryFactory contactRepositoryFactory)
        {
            _automationRepository = automationRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _repositoryFactory = repositoryFactory;
            _emailService = emailService;
            _workOrderService = workOrderService;
            _teamMemberService = teamMemberService;
            _subContractorService = subContractorService;
            _photosService = photosService;
            _noteService = noteService;
            _lineItemService = lineItemService;
            _relatedContacts = relatedContacts;
            _jobService = jobService;
            _contactsRelatedContactService = contactsRelatedContactService;
            _contactsTeamMemberService = contactsTeamMemberService;
            _contactsService = contactsService;
            _contactRepositoryFactory = contactRepositoryFactory;
        }

        public void ProcessEventBasedAutomation<TEntity>(string whenEntityIs, string triggerRecord, long Id) where TEntity : BaseEntity
        {
            var repository = _repositoryFactory.GetRepository<TEntity>();
            var automationRulesQuery = _automationRepository.GetAllAsync()
                                                .Where(a =>
                                                       a.IsActive &&
                                                       a.IsDeleted != true &&
                                                       a.TriggerType.ToLower() == "eventbase" &&
                                                       a.TriggerRecord == triggerRecord &&
                                                       ((whenEntityIs == "created" || whenEntityIs == "modified" || whenEntityIs == "createdormodified") &&
                                                        (a.WhenEntityIs == whenEntityIs || a.WhenEntityIs == "createdormodified")) ||
                                                        (whenEntityIs == "deleted" && a.WhenEntityIs == whenEntityIs))
                                                .Include(a => a.Conditions)
                                                .Include(a => a.Actions);
            var automationRules = automationRulesQuery.ToList();

            foreach (var rule in automationRules)
            {
                var entity = repository.Get(Id);
                bool allConditionsMet = true;
                foreach (var condition in rule.Conditions)
                {
                    bool conditionMet = EvaluateCondition(condition, entity);
                    if (rule.RequireAllConditionsToBeTrue && !conditionMet)
                    {
                        allConditionsMet = false;
                        break;
                    }
                    else if (!rule.RequireAllConditionsToBeTrue && conditionMet)
                    {
                        allConditionsMet = true;
                        break;
                    }
                }

                if (allConditionsMet)
                {
                    ExecuteActionsAsync(rule.Actions, entity);
                }

            }
        
        }

        public void ProcessContactEventBasedAutomation<TEntity>(string whenEntityIs, string triggerRecord, long Id) where TEntity : ContactBaseEntity
        {
            var repository = _contactRepositoryFactory.GetRepository<TEntity>();
            var automationRulesQuery = _automationRepository.GetAllAsync()
                                                .Where(a =>
                                                       a.IsActive &&
                                                       a.IsDeleted != true &&
                                                       a.TriggerType.ToLower() == "eventbase" &&
                                                       a.TriggerRecord == triggerRecord &&
                                                       ((whenEntityIs == "created" || whenEntityIs == "modified" || whenEntityIs == "createdormodified") &&
                                                        (a.WhenEntityIs == whenEntityIs || a.WhenEntityIs == "createdormodified")) ||
                                                        (whenEntityIs == "deleted" && a.WhenEntityIs == whenEntityIs))
                                                .Include(a => a.Conditions)
                                                .Include(a => a.Actions);
            var automationRules = automationRulesQuery.ToList();

            foreach (var rule in automationRules)
            {
                var entity = repository.Get(Id);
                bool allConditionsMet = true;
                foreach (var condition in rule.Conditions)
                {
                    bool conditionMet = EvaluateCondition(condition, entity);
                    if (rule.RequireAllConditionsToBeTrue && !conditionMet)
                    {
                        allConditionsMet = false;
                        break;
                    }
                    else if (!rule.RequireAllConditionsToBeTrue && conditionMet)
                    {
                        allConditionsMet = true;
                        break;
                    }
                }

                if (allConditionsMet)
                {
                    ExecuteActionsAsync(rule.Actions, entity);
                }

            }

        }

        public void ProcessTimeBasedAutomation(AutomationEntity rule)
        {
            if (rule.TriggerRecord.ToLower() == "contact")
            {
                ProcessContactAutomation(rule);
            }
            else if (rule.TriggerRecord.ToLower() == "job")
            {
                ProcessJobAutomation(rule);
            }
            else if (rule.TriggerRecord.ToLower() == "workorder")
            {
                ProcessWorkOrderAutomation(rule);
            }
        }

        private void ProcessJobAutomation(AutomationEntity rule)
        {
            var repository = _repositoryFactory.GetRepository<Job>();
            var entities = repository.GetAllAsync().ToList();
            foreach (var entity in entities)
            {
                bool allConditionsMet = true;
                foreach (var condition in rule.Conditions)
                {
                    bool conditionMet = EvaluateCondition(condition, entity);
                    if (rule.RequireAllConditionsToBeTrue && !conditionMet)
                    {
                        allConditionsMet = false;
                        break;
                    }
                    else if (!rule.RequireAllConditionsToBeTrue && conditionMet)
                    {
                        allConditionsMet = true;
                        break;
                    }
                }

                if (allConditionsMet)
                {
                    ExecuteActionsAsync(rule.Actions, entity);
                }
            }
        }
        private void ProcessWorkOrderAutomation(AutomationEntity rule)
        {
            var repository = _repositoryFactory.GetRepository<WorkOrder>();
            var entities = repository.GetAllAsync().ToList();
            foreach (var entity in entities)
            {
                bool allConditionsMet = true;
                foreach (var condition in rule.Conditions)
                {
                    bool conditionMet = EvaluateCondition(condition, entity);
                    if (rule.RequireAllConditionsToBeTrue && !conditionMet)
                    {
                        allConditionsMet = false;
                        break;
                    }
                    else if (!rule.RequireAllConditionsToBeTrue && conditionMet)
                    {
                        allConditionsMet = true;
                        break;
                    }
                }

                if (allConditionsMet)
                {
                    ExecuteActionsAsync(rule.Actions, entity);
                }
            }
        }
        private void ProcessContactAutomation(AutomationEntity rule)
        {
            var _contactsRepository = _contactRepositoryFactory.GetRepository<ContactEntity>();

            var entities = _contactsRepository.GetAllAsync().ToList();

            foreach (var entity in entities)
            {
                bool allConditionsMet = true;
                foreach (var condition in rule.Conditions)
                {
                    bool conditionMet = EvaluateCondition(condition, entity);
                    if (rule.RequireAllConditionsToBeTrue && !conditionMet)
                    {
                        allConditionsMet = false;
                        break;
                    }
                    else if (!rule.RequireAllConditionsToBeTrue && conditionMet)
                    {
                        allConditionsMet = true;
                        break;
                    }
                }

                if (allConditionsMet)
                {
                    ExecuteActionsAsync(rule.Actions, entity);
                }
            }
        }

        #region Evaluate Conditions and Automation Executions
        private bool EvaluateCondition<T>(ConditionEntity condition, T eventData) where T : class
        {

            dynamic fieldValue = null;
            if (typeof(T) == typeof(Job))
            {
                var job = eventData as Job;
                switch (condition.Field)
                {
                    case "status":
                        fieldValue = job.JobStatusId;
                        break;
                    case "saleRep":
                        fieldValue = job.SalesRepsentativeId;
                        break;
                    case "workflowId":
                        fieldValue = job.WorkFlowId;
                        break;
                    case "source":
                        fieldValue = job.LeadSourceId;
                        break;
                    case "subcontractor":
                        fieldValue = job.SubContractorId;
                        break;
                    //case "relatedContacts":

                    //    break;
                    default:
                        break;
                }
            }
            else if (typeof(T) == typeof(WorkOrder))
            {
                var workOrder = eventData as WorkOrder;
                switch (condition.Field)
                {
                    case "status":
                        fieldValue = workOrder.WorkOrderStatus;
                        break;
                    case "saleRep":
                        var job = _jobService.GetAllAsync().Where(x => x.Id == workOrder.JobId).Select(x => x.SalesRepsentativeId).FirstOrDefault();
                        if (job > 0)
                            fieldValue = job;
                        break;
                    case "workflowId":
                        fieldValue = workOrder.WorkflowId;
                        break;
                    case "source":
                        var LeadSourceId = _jobService.GetAllAsync().Where(x => x.Id == workOrder.JobId).Select(x => x.LeadSourceId).FirstOrDefault();
                        if (LeadSourceId > 0)
                            fieldValue = LeadSourceId;
                        break;
                    case "subcontractor":
                        var SubContractorId = _jobService.GetAllAsync().Where(x => x.Id == workOrder.JobId).Select(x => x.SubContractorId).FirstOrDefault();
                        if (SubContractorId > 0)
                            fieldValue = SubContractorId;
                        break;
                    //case "relatedContacts":

                    //    break;
                    default:
                        break;
                }

            }
            else if (typeof(T) == typeof(ContactEntity))
            {
                var contact = eventData as ContactEntity;
                switch (condition.Field)
                {
                    case "status":
                        fieldValue = contact.StatusId;
                        break;
                    case "saleRep":
                        fieldValue = contact.SalesRepId;
                        break;
                    case "workflowId":
                        fieldValue = contact.WorkFlowId;
                        break;
                    case "source":
                        fieldValue = contact.SourceId;
                        break;
                    case "subcontractor":
                        fieldValue = contact.SubContractorId;
                        break;
                    //case "relatedContacts":

                    //    break;
                    default:
                        break;
                }

            }
            else
            {
                fieldValue = string.Empty;
            }
            string fieldValueString = fieldValue?.ToString();
            string conditionValue = condition.Value;

            return condition.Comparison switch
            {
                "==" => fieldValueString == conditionValue,
                "!=" => fieldValueString != conditionValue,
                _ => false,
            };
        }

        private async Task ExecuteActionsAsync<T>(List<ActionEntity> actions, T eventData) where T : class
        {
            foreach (var action in actions)
            {
                switch (action.ActionType)
                {
                    case "sendEmail":
                        await ExecuteSendEmailAction<T>(action, eventData);
                        break;
                    case "changeField":
                        ExecuteChangeFieldAction(action);
                        break;
                    case "sendTextMessage":
                        ExecuteSendTextMessageAction<T>(action, eventData);
                        break;
                    case "callWebhook":
                        await ExecuteCallWebhookAction(action);
                        break;
                    case "createTask":
                        await ExecuteCreateTaskActionAsync<T>(action, eventData);
                        break;
                    case "createWorkOrder":
                        ExecuteCreateWorkOrderAction<T>(action, eventData);
                        break;
                    default:
                        Console.WriteLine($"Unsupported action type: {action.ActionType}");
                        break;
                }
            }
        }

        #endregion

        #region Automation Actions
        private async Task ExecuteSendEmailAction<T>(ActionEntity action, T eventData) where T : class
        {
            try
            {
                var actionObj = JsonConvert.DeserializeObject<TemplateDto<EmailTemplateEntity>>(action.ActionObj);
                if (actionObj != null)
                {
                    EmailTemplateEntity emailDetails = actionObj.Template;
                    var emailTasks = new List<Task<AutomationResponseDto>>();

                    foreach (var recipient in actionObj.Recipients)
                    {
                        if (IsValidEmail(recipient))
                        {
                            emailTasks.Add(_emailService.SendEmailAsync(recipient, emailDetails.Subject, emailDetails.Body));
                        }
                        else
                        {
                            var recipients = new List<string>() { recipient };
                            var actualRecipients = GetLinkedEmailIds(recipients, eventData);
                            foreach (var actualRecipient in actualRecipients)
                            {
                                emailTasks.Add(_emailService.SendEmailAsync(actualRecipient, emailDetails.Subject, emailDetails.Body));
                            }
                        }
                    }
                    await Task.WhenAll(emailTasks);
                }
                else
                {
                    throw new Exception("Error deserializing email template.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void ExecuteSendTextMessageAction<T>(ActionEntity action, T eventData) where T : class
        {
            try
            {
                // Implement logic to send a text message using the action details and event data
                Console.WriteLine("Text message sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending text message: {ex.Message}");
            }
        }
        private async Task ExecuteCallWebhookAction(ActionEntity action)
        {
            try
            {
                var webhookDto = JsonConvert.DeserializeObject<WebHookDTO>(action.ActionObj);
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, webhookDto.TargetedUrl);
                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Webhook called successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to call webhook. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling webhook: {ex.Message}");
            }
        }
        private async Task ExecuteChangeFieldAction(ActionEntity action)
        {
            try
            {
                var webhookDto = JsonConvert.DeserializeObject<WebHookDTO>(action.ActionObj);
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, webhookDto.TargetedUrl);
                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Webhook called successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to call webhook. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling webhook: {ex.Message}");
            }
        }
        private async Task<AutomationResponseDto> ExecuteCreateTaskActionAsync<T>(ActionEntity action, T entity) where T : class
        {
            try
            {
                var automationTask = JsonConvert.DeserializeObject<CreateAutomationTaskDto>(action.ActionObj);
                if( automationTask == null )
                {
                    return new AutomationResponseDto
                    {
                        success = false,
                        message = "Failed to create task."
                    };
                }
                var task = ConvertAutomationTask(automationTask);
                var AssignedTeamMembers = GetLinkedIds(automationTask.AssignedTeamMembers, entity);
                var RelatedContacts = GetLinkedIds(automationTask.RelatedContacts, entity);
                var RelatedSubcontractors = GetLinkedIds(automationTask.RelatedSubcontractors, entity);

                long createdById = 0;
                long companyId = 0;

                if (entity is Job jobObj)
                {
                    createdById = jobObj.CreatedBy ?? 0;
                    companyId = jobObj.CompanyId ?? 0;
                }
                else if (entity is WorkOrder workOrder)
                {
                    createdById = workOrder.CreatedBy ?? 0;
                    companyId = workOrder.CompanyId ?? 0;
                }
                else if (entity is ContactEntity contact)
                {
                    createdById = contact.CreatedBy;
                    companyId = contact.CompanyId;
                }

                var taskRepo = _contactRepositoryFactory.GetRepository<TaskEntity>();
                task.CreatedBy = createdById;
                task.CompanyId = companyId;

                taskRepo.Insert(task);

                foreach (var rltcontactid in RelatedContacts)
                {
                    var taskRltContactsRepo = _contactRepositoryFactory.GetRepository<TaskContactEntity>();
                    var relatedContact = new TaskContactEntity
                    {
                        CreatedBy = createdById,
                        CompanyId = companyId,
                        TaskId = task.Id,
                        ContactId = rltcontactid,
                        CreatedDate = DateTime.Now,
                    };
                    taskRltContactsRepo.Insert(relatedContact);
                }

                foreach (var jobid in automationTask.RelatedJobs)
                {
                    var taskJobsRepo = _contactRepositoryFactory.GetRepository<TaskJobEntity>();
                    var job = new TaskJobEntity
                    {
                        CreatedBy = createdById,
                        CompanyId = companyId,
                        TaskId = task.Id,
                        CreatedDate = DateTime.Now,

                    };

                    if (entity is Job jobObjTemp)
                    {
                        job.JobId = jobObjTemp.Id;
                    }
                    else if (entity is WorkOrder workOrder)
                    {
                        job.JobId = (long)workOrder.JobId;
                    }

                    taskJobsRepo.Insert(job);
                }

                foreach (var subContractorid in RelatedSubcontractors)
                {
                    var taskSubcontroctorsRepo = _contactRepositoryFactory.GetRepository<TaskSubContractorEntity>();
                    var sbC = new TaskSubContractorEntity
                    {
                        CreatedBy = createdById,
                        CompanyId = companyId,
                        TaskId = task.Id,
                        SubContractorId = subContractorid,
                        CreatedDate = DateTime.Now,

                    };
                    taskSubcontroctorsRepo.Insert(sbC);
                }

                foreach (var teamMemberid in AssignedTeamMembers)
                {
                    var taskTeamMembersRepo = _contactRepositoryFactory.GetRepository<TaskTeamMemberEntity>();
                    var tM = new TaskTeamMemberEntity
                    {
                        CreatedBy = createdById,
                        CompanyId = companyId,
                        TaskId = task.Id,
                        TeamMemberId = teamMemberid,
                        CreatedDate = DateTime.Now,

                    };
                    taskTeamMembersRepo.Insert(tM);
                }

                return new AutomationResponseDto
                {
                    success = true,
                    message = "Task has been created successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating task: {ex.Message}");
                return new AutomationResponseDto
                {
                    success = false,
                    message = "Failed to create task."
                };
            }
        }
        private void ExecuteCreateWorkOrderAction<T>(ActionEntity action, T entity) where T : class
        {
            try
            {
                var workOrderObj = JsonConvert.DeserializeObject<DTOCreateAutomationWorkOrder>(action.ActionObj);

                var workOrderData = new WorkOrder
                {
                    Name = workOrderObj.Name,
                    WorkflowId = workOrderObj.WorkflowId,
                    WorkOrderStatus = workOrderObj.WorkOrderStatus,
                    WorkOrderPriority = workOrderObj.WorkOrderPriority,
                    StartDate = DateTime.Now,
                    DueDate = CalculateStartAndEndDate(workOrderObj.TimeUnit, workOrderObj.Duration),
                    LastStatusChangeDate = workOrderObj.LastStatusChangeDate
                };

                if (typeof(T) == typeof(Job))
                {
                    var job = entity as Job;
                    workOrderData.JobId = (int)job.Id;
                    workOrderData.ContactId = (int)job.PrimaryContactId;
                    workOrderData.CreatedBy = job.CreatedBy;
                    workOrderData.CompanyId = job.CompanyId;
                    _workOrderService.Insert(workOrderData);

                }
                else if (typeof(T) == typeof(WorkOrder))
                {
                    var workOrder = entity as WorkOrder;
                    workOrderData.JobId = (int)workOrder.JobId;
                    workOrderData.ContactId = workOrder.ContactId;
                    workOrderData.CreatedBy = workOrder.CreatedBy;
                    workOrderData.CompanyId = workOrder.CompanyId;
                    _workOrderService.Insert(workOrderData);


                }
                else if (typeof(T) == typeof(ContactEntity))
                {
                    var contact = entity as ContactEntity;
                    workOrderData.ContactId = (int)contact.Id;
                    workOrderData.CreatedBy = contact.CreatedBy;
                    workOrderData.CompanyId = contact.CompanyId;
                    _workOrderService.Insert(workOrderData);


                }
                else
                {
                    throw new Exception("Not Created");
                }

                foreach (var item in workOrderObj.LineItems)
                {
                    _lineItemService.Insert(new LineItem()
                    {
                        Description = item.Description,
                        Name = item.Name,
                        Quantity = item.Quantity,
                        WorkOrderId = workOrderData.Id
                    });
                }
                foreach (var item in workOrderObj.Notes)
                {
                    var noteData = new Note
                    {
                        WorkOrderId = (int)workOrderData.Id,
                        Type = item.Type,
                        Content = item.Content,

                    };
                    _noteService.Insert(noteData);

                    foreach (var attachment in item.Attachments)
                    {
                        _photosService.Insert(new PhotosEntity
                        {
                            FilePath = attachment.FilePath,
                            WorkOrderId = workOrderData.Id,
                            NoteId = noteData.Id
                        });

                    }
                }

                var teamMembers = GetLinkedIds(workOrderObj.TeamMemberId, entity);
                var subContractors = GetLinkedIds(workOrderObj.SubContractorId, entity);

                foreach (var item in teamMembers)
                {
                    _teamMemberService.Insert(new TeamMembers()
                    {
                        TeamMemberId = item,
                        WorkOrderId = workOrderData.Id
                    });
                }

                foreach (var item in subContractors)
                {
                    _subContractorService.Insert(new SubContractor()
                    {
                        WorkOrderId = workOrderData.Id,
                        SubContractorId = item
                    });
                }

                ProcessEventBasedAutomation<WorkOrder>("created", "workorder", workOrderData.Id);
                Console.WriteLine("Work order created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating work order: {ex.Message}");
            }
        }
        #endregion

        #region Helpers
        private List<long> GetLinkedIds<T>(List<string>? members, T entity) where T : class
        {
            var assignedTeamMembers = new List<long>();
            if (members == null) return assignedTeamMembers;

            if (typeof(T) == typeof(Job))
            {
                var job = entity as Job;
                var teamMemberIds = _teamMemberService.GetAll().Where(x => x.JobId == job.Id).Select(x => x.TeamMemberId).ToList();
                var relatedContactIds = _relatedContacts.GetAll().Where(x => x.JobId == job.Id).Select(x => x.RelatedContactId).ToList();

                foreach (var item in members)
                {
                    if (item == "assignees")
                    {
                        assignedTeamMembers.AddRange(teamMemberIds);
                    }
                    else if (item == "contact")
                    {
                        assignedTeamMembers.Add((long)job.PrimaryContactId);
                    }
                    else if (item == "salesRep")
                    {
                        assignedTeamMembers.Add(job.SalesRepsentativeId);
                    }
                    else if (item == "relatedContacts")
                    {
                        assignedTeamMembers.AddRange(relatedContactIds);
                    }
                    else if (item == "subcontractor")
                    {
                        if (job.SubContractorId > 0)
                            assignedTeamMembers.Add(job.SubContractorId);
                    }
                    else
                    {
                        assignedTeamMembers.Add(Convert.ToInt64(item));
                    }
                }
            }
            else if (typeof(T) == typeof(WorkOrder))
            {
                var workOrder = entity as WorkOrder;
                var teamMemberIds = _teamMemberService.GetAll().Where(x => x.WorkOrderId == workOrder.Id).Select(x => x.TeamMemberId).ToList();
                var relatedContactIds = _relatedContacts.GetAll().Where(x => x.JobId == workOrder.JobId).Select(x => x.RelatedContactId).ToList();

                foreach (var item in members)
                {
                    if (item == "assignees")
                    {
                        assignedTeamMembers.AddRange(teamMemberIds);
                    }
                    else if (item == "contact")
                    {
                        assignedTeamMembers.Add((long)workOrder.ContactId);
                    }
                    else if (item == "salesRep")
                    {
                        var salesRep = _jobService.GetAllAsync().Where(x => x.Id == workOrder.JobId).Select(x => x.SalesRepsentativeId).FirstOrDefault();
                        assignedTeamMembers.Add(salesRep);
                    }
                    else if (item == "relatedContacts")
                    {
                        assignedTeamMembers.AddRange(relatedContactIds);
                    }
                    else if (item == "subcontractor")
                    {
                        var subcontractor = _jobService.GetAllAsync().Where(x => x.Id == workOrder.JobId).Select(x => x.SubContractorId).FirstOrDefault();
                        if (subcontractor > 0)
                            assignedTeamMembers.Add(subcontractor);
                    }
                    else
                    {
                        assignedTeamMembers.Add(Convert.ToInt64(item));
                    }
                }
            }
            else if (typeof(T) == typeof(ContactEntity))
            {
                var contact = entity as ContactEntity;

                var teamMemberIds = _contactsTeamMemberService.GetAll().Where(x => x.ContactId == contact.Id).Select(x => x.TeamMemberId).ToList();
                var relatedContactIds = _contactsRelatedContactService.GetAll().Where(x => x.ContactId == contact.Id).Select(x => x.RelatedContactId).ToList();

                foreach (var item in members)
                {
                    if (item == "assignees")
                    {
                        assignedTeamMembers.AddRange(teamMemberIds);
                    }
                    else if (item == "contact")
                    {
                        assignedTeamMembers.Add((long)contact.Id);
                    }
                    else if (item == "salesRep")
                    {
                        if (contact.SalesRepId > 0)
                            assignedTeamMembers.Add((long)contact.SalesRepId);
                    }
                    else if (item == "relatedContacts")
                    {
                        assignedTeamMembers.AddRange(relatedContactIds);
                    }
                    else if (item == "subcontractor")
                    {
                        if (contact.SubContractorId > 0)
                            assignedTeamMembers.Add((long)contact.SubContractorId);
                    }
                    else
                    {
                        assignedTeamMembers.Add(Convert.ToInt64(item));
                    }
                }
            }

            return assignedTeamMembers;
        }
        private List<string> GetLinkedEmailIds<T>(List<string> members, T entity) where T : class
        {
            var emailIds = new List<string>();
            if (typeof(T) == typeof(Job))
            {
                var job = entity as Job;
                var teamMemberIds = _teamMemberService.GetAll().Where(x => x.JobId == job.Id).Select(x => x.TeamMemberId).ToList();
                var relatedContactIds = _relatedContacts.GetAll().Where(x => x.JobId == job.Id).Select(x => x.RelatedContactId).ToList();

                foreach (var item in members)
                {
                    if (item == "assignees")
                    {
                        var emails = GetEmailsById(teamMemberIds).Result;
                        emailIds.AddRange(emails.Emails);
                    }
                    else if (item == "contact")
                    {
                        var contact = _contactsService.Get((long)job.PrimaryContactId);
                        if (contact != null && !string.IsNullOrEmpty(contact.Email))
                            emailIds.Add(contact.Email);
                    }
                    else if (item == "salesRep")
                    {
                        if (job.SalesRepsentativeId > 0)
                        {

                            var emails = GetEmailsById(new List<long> { job.SalesRepsentativeId }).Result;
                            emailIds.AddRange(emails.Emails);
                        }

                    }
                    else if (item == "relatedContacts")
                    {
                        var emails = GetEmailsById(relatedContactIds).Result;
                        emailIds.AddRange(emails.Emails);
                    }
                    else if (item == "subcontractor")
                    {
                        if (job.SubContractorId > 0)
                        {

                            var emails = GetEmailsById(new List<long> { job.SubContractorId }).Result;
                            emailIds.AddRange(emails.Emails);
                        }
                    }
                    else
                    {
                        if (IsValidEmail(item))
                            emailIds.Add(item);
                    }
                }
            }
            else if (typeof(T) == typeof(WorkOrder))
            {
                var workOrder = entity as WorkOrder;
                var teamMemberIds = _teamMemberService.GetAll().Where(x => x.WorkOrderId == workOrder.Id).Select(x => x.TeamMemberId).ToList();
                var relatedContactIds = _relatedContacts.GetAll().Where(x => x.JobId == workOrder.JobId).Select(x => x.RelatedContactId).ToList();

                foreach (var item in members)
                {
                    if (item == "assignees")
                    {
                        var emails = GetEmailsById(teamMemberIds).Result;
                        emailIds.AddRange(emails.Emails);
                    }
                    else if (item == "contact")
                    {
                        var contact = _contactsService.Get((long)workOrder.ContactId);
                        if (contact != null && !string.IsNullOrEmpty(contact.Email))
                            emailIds.Add(contact.Email);
                    }
                    else if (item == "salesRep")
                    {
                        var salesRep = _jobService.GetAllAsync().Where(x => x.Id == workOrder.JobId).Select(x => x.SalesRepsentativeId).FirstOrDefault();
                        if (salesRep > 0)
                        {

                            var emails = GetEmailsById(new List<long> { salesRep }).Result;
                            emailIds.AddRange(emails.Emails);
                        }
                    }
                    else if (item == "relatedContacts")
                    {
                        var emails = GetEmailsById(relatedContactIds).Result;
                        emailIds.AddRange(emails.Emails);
                    }
                    else if (item == "subcontractor")
                    {
                        var subcontractor = _jobService.GetAllAsync().Where(x => x.Id == workOrder.JobId).Select(x => x.SubContractorId).FirstOrDefault();
                        if (subcontractor > 0)
                        {

                            var emails = GetEmailsById(new List<long> { subcontractor }).Result;
                            emailIds.AddRange(emails.Emails);
                        }
                    }
                    else
                    {
                        emailIds.Add(item);
                    }
                }
            }
            else if (typeof(T) == typeof(ContactEntity))
            {
                var contact = entity as ContactEntity;

                var teamMemberIds = _contactsTeamMemberService.GetAll().Where(x => x.ContactId == contact.Id).Select(x => x.TeamMemberId).ToList();
                var relatedContactIds = _contactsRelatedContactService.GetAll().Where(x => x.ContactId == contact.Id).Select(x => x.RelatedContactId).ToList();

                foreach (var item in members)
                {
                    if (item == "assignees")
                    {
                        var emails = GetEmailsById(teamMemberIds).Result;
                        emailIds.AddRange(emails.Emails);
                    }
                    else if (item == "contact")
                    {
                        if (contact != null && !string.IsNullOrEmpty(contact.Email))
                            emailIds.Add(contact.Email);
                    }
                    else if (item == "salesRep")
                    {
                        if (contact.SalesRepId > 0)
                        {

                            var emails = GetEmailsById(new List<long> { (long)contact.SalesRepId }).Result;
                            emailIds.AddRange(emails.Emails);
                        }
                    }
                    else if (item == "relatedContacts")
                    {
                        var emails = GetEmailsById(relatedContactIds).Result;
                        emailIds.AddRange(emails.Emails);
                    }
                    else if (item == "subcontractor")
                    {
                        if (contact.SubContractorId > 0)
                        {
                            var emails = GetEmailsById(new List<long> { (long)contact.SubContractorId }).Result;
                            emailIds.AddRange(emails.Emails);
                        }
                    }
                    else
                    {
                        emailIds.Add(item);
                    }
                }
            }


            return emailIds;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public TaskEntity ConvertAutomationTask(CreateAutomationTaskDto automationTaskDto)
        {
            var endDate = CalculateStartAndEndDate(automationTaskDto.TimeUnit, automationTaskDto.Duration);
            var taskDto = new TaskEntity
            {
                TaskName = automationTaskDto.TaskName,
                TaskType = automationTaskDto.TaskType,
                Priority = automationTaskDto.Priority,
                StartDate = DateTime.Now,
                EndDate = endDate,
                EstimatedDuration = automationTaskDto.EstimatedDuration,
                Tags = automationTaskDto.Tags,
                Description = automationTaskDto.Description,
                EstimatedType = automationTaskDto.EstimatedType
            };

            return taskDto;
        }
        public static DateTime CalculateStartAndEndDate(string unit, string duration)
        {
            DateTime startDate = DateTime.Now;
            TimeSpan durationTimeSpan = TimeSpan.Zero;

            if (!string.IsNullOrEmpty(unit) && !string.IsNullOrEmpty(duration))
            {
                durationTimeSpan = unit switch
                {
                    "m" => TimeSpan.FromMinutes(double.Parse(duration)),
                    "h" => TimeSpan.FromHours(double.Parse(duration)),
                    "d" => TimeSpan.FromDays(double.Parse(duration)),
                    "w" => TimeSpan.FromDays(7 * double.Parse(duration)),
                    "mo" => TimeSpan.FromDays(30 * double.Parse(duration)),
                    "y" => TimeSpan.FromDays(365 * double.Parse(duration)),
                    _ => TimeSpan.FromDays(double.Parse(duration)),
                };
            }

            DateTime endDate = startDate.Add(durationTimeSpan);

            return endDate;
        }
        private async Task<GetEmailDto> GetEmailsById(List<long> ids)
        {
            var emailsRes = new GetEmailDto();
            try
            {
                var baseApiUrl = _configuration.GetSection("ServiceUrls:IdentityAPI").Value;
                var apiUrl = $"{baseApiUrl}getEmailsByIds";

                var content = new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var emailsResponse = JsonConvert.DeserializeObject<GenericResponse<GetEmailDto>>(responseData);
                    emailsRes = emailsResponse?.payload;
                    return emailsRes ?? new GetEmailDto();
                }
                else
                {
                    return emailsRes;
                }
            }
            catch (Exception)
            {

                return emailsRes;
            }
        }

        #endregion

        #region Time Calculations
        public DateTime? CalculateNextExecutionTime(AutomationEntity entity, DateTime refTime)
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

        #endregion
    }
}
