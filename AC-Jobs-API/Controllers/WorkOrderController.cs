using AC_Contact_Services.BaseService;
using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkOrderController : ControllerBase
    {
        private readonly ICustomService<WorkOrder> _workOrderService;
        private readonly ICustomService<TeamMembers> _teamMemberService;
        private readonly ICustomService<SubContractor> _subContractorService;
        private readonly ICustomService<PhotosEntity> _photosService;
        private readonly ICustomService<Note> _noteService;
        private readonly ICustomService<LineItem> _lineItemService;
        private readonly IEventAutomationService _eventAutomationService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public WorkOrderController(
            ICustomService<WorkOrder> workOrderService,
            ApplicationDbContext applicationDbContext,
            IMapper mapper,
            ICustomService<TeamMembers> teamMemberService,
            ICustomService<LineItem> lineItemService,
            ICustomService<SubContractor> subContractorService,
            ICustomService<Note> noteService,
            ICustomService<PhotosEntity> photosService,
            IEventAutomationService eventAutomationService)
        {
            _workOrderService = workOrderService;
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _teamMemberService = teamMemberService;
            _lineItemService = lineItemService;
            _subContractorService = subContractorService;
            _noteService = noteService;
            _photosService = photosService;
            _eventAutomationService = eventAutomationService;
        }
        [HttpGet(nameof(GetWorkOrderById))]
        public IActionResult GetWorkOrderById(int Id)
        {
            var obj = _workOrderService.Get(Id);
            if (obj == null)
            {
                return NotFound(new
                {
                    message = "WorkOrder not found."
                });
            }
            else
            {
                var result = new DTOWordOrder()
                {
                    Id = obj.Id,
                    Name = obj.Name,
                    WorkflowId = obj.WorkflowId,
                    WorkOrderPriority = obj.WorkOrderPriority,
                    WorkOrderStatus = obj.WorkOrderStatus,
                    DueDate = (DateTime)obj.DueDate,
                    LastStatusChangeDate = obj.LastStatusChangeDate,
                    StartDate = obj.StartDate,
                    JobId = obj.JobId
                };

                result.LineItems = _lineItemService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(item => new LineItems
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                }).ToList();
                result.Notes = _noteService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(item => new DTONote()
                {
                    Id = item.Id,
                    Content = item.Content,
                    Type = item.Type,
                    WorkOrderId = item.WorkOrderId
                }).ToList();
                foreach (var item in result?.Notes)
                {
                    var attachments = _photosService.GetAll().Where(x => x.NoteId == item.Id);
                    if (attachments != null)
                    {
                        item.Attachments = attachments.Select(sub => new DTOAttachment()
                        {
                            Id = sub.Id,
                            FilePath = sub.FilePath,

                        }).ToList();
                    }
                }
                result.SubContractorId = _subContractorService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(sub => sub.SubContractorId).ToList();
                result.TeamMemberId = _teamMemberService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(sub => sub.TeamMemberId).ToList();
                return Ok(new
                {
                    message = "Operation successful.",
                    data = result
                });
            }
        }


        [HttpGet(nameof(GetWorkOrderByJobId))]
        public IActionResult GetWorkOrderByJobId(int JobId)
        {
            var workOrders = _workOrderService.GetAll().Where(x => x.JobId == JobId);
            var responseData = new List<DTOWordOrder>() { };
            if (workOrders == null)
            {
                return NotFound(new
                {
                    message = "WorkOrder not found."
                });
            }
            else
            {
                foreach (var obj in workOrders)
                {
                    var result = new DTOWordOrder()
                    {
                        Id = obj.Id,
                        Name = obj.Name,
                        WorkflowId = obj.WorkflowId,
                        WorkOrderPriority = obj.WorkOrderPriority,
                        WorkOrderStatus = obj.WorkOrderStatus,
                        DueDate = (DateTime)obj.DueDate,
                        LastStatusChangeDate = obj.LastStatusChangeDate,
                        StartDate = obj.StartDate,
                        JobId = obj.JobId
                    };
                    result.LineItems = _lineItemService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(item => new LineItems
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Quantity = item.Quantity,
                    }).ToList();
                    result.Notes = _noteService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(item => new DTONote()
                    {
                        Id = item.Id,
                        Content = item.Content,
                        Type = item.Type,
                        WorkOrderId = item.WorkOrderId
                    }).ToList();
                    foreach (var item in result?.Notes)
                    {
                        var attachments = _photosService.GetAll().Where(x => x.NoteId == item.Id);
                        if (attachments != null)
                        {
                            item.Attachments = attachments.Select(sub => new DTOAttachment()
                            {
                                Id = sub.Id,
                                FilePath = sub.FilePath,

                            }).ToList();
                        }
                    }
                    result.SubContractorId = _subContractorService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(sub => sub.SubContractorId).ToList();
                    result.TeamMemberId = _teamMemberService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(sub => sub.TeamMemberId).ToList();
                    responseData.Add(result);
                }
                return Ok(new
                {
                    message = "Operation successful.",
                    data = responseData
                });
            }
        }


        [HttpGet(nameof(GetAllWorkOrder))]
        public IActionResult GetAllWorkOrder()
        {
            var workOrders = _workOrderService.GetAll();

            if (workOrders == null)
            {
                return NotFound(new
                {
                    message = "WorkOrder not found."
                });
            }
            else
            {
                var result = workOrders.Select(obj => new DTOWordOrder
                {
                    Id = obj.Id,
                    Name = obj.Name,
                    WorkflowId = obj.WorkflowId,
                    WorkOrderPriority = obj.WorkOrderPriority,
                    WorkOrderStatus = obj.WorkOrderStatus,
                    DueDate = (DateTime)obj.DueDate,
                    LastStatusChangeDate = obj.LastStatusChangeDate,
                    StartDate = obj.StartDate,
                    JobId = obj.JobId,
                    LineItems = _lineItemService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(item => new LineItems
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Quantity = item.Quantity,
                    }).ToList(),
                    Notes = _noteService.GetAll().Where(x => x.WorkOrderId == obj.Id).Select(item => new DTONote()
                    {
                        Id = item.Id,
                        Content = item.Content,
                        JobId = item.JobId,
                        Type = item.Type,
                        WorkOrderId = item.WorkOrderId
                    }).ToList()
                }).ToList();

                foreach (var order in result)
                {
                    foreach (var item in order.Notes)
                    {
                        item.Attachments = _photosService.GetAll().Where(x => x.NoteId == item.Id).Select(sub => new DTOAttachment
                        {
                            Id = sub.Id,
                            FilePath = sub.FilePath,
                        }).ToList();
                    }

                    order.SubContractorId = _subContractorService.GetAll().Where(x => x.WorkOrderId == order.Id).Select(sub => sub.SubContractorId).ToList();
                    order.TeamMemberId = _teamMemberService.GetAll().Where(x => x.WorkOrderId == order.Id).Select(sub => sub.TeamMemberId).ToList();
                }

                return Ok(new
                {
                    message = "Operation successful.",
                    data = result
                });
            }
        }

        [HttpPost(nameof(CreateWorkOrder))]
        public IActionResult CreateWorkOrder(DTOCreateWorkOrder WorkOrder)
        {
            if (WorkOrder != null)
            {
                var workOrderData = _mapper.Map<WorkOrder>(WorkOrder);
                workOrderData.LastStatusChangeDate = DateTime.Now;
                _workOrderService.Insert(workOrderData);
                foreach (var item in WorkOrder.LineItems)
                {
                    _lineItemService.Insert(new LineItem()
                    {
                        Description = item.Description,
                        Name = item.Name,
                        Quantity = item.Quantity,
                        WorkOrderId = workOrderData.Id
                    });
                }

                foreach (var item in WorkOrder.TeamMemberId)
                {
                    _teamMemberService.Insert(new TeamMembers()
                    {
                        TeamMemberId = item,
                        WorkOrderId = workOrderData.Id
                    });
                }

                foreach (var item in WorkOrder.SubContractorId)
                {
                    _subContractorService.Insert(new SubContractor()
                    {
                        WorkOrderId = workOrderData.Id,
                        SubContractorId = item
                    });
                }

                foreach (var item in WorkOrder.Notes)
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

                _eventAutomationService.ProcessEventBasedAutomation<WorkOrder>("created", "workorder", workOrderData.Id);
                return Ok(new { message = "Created Successfully" });
            }
            else
            {
                return BadRequest(new { message = "Somethingwent wrong" });
            }
        }

        [HttpPost(nameof(UpdateWorkOrder))]
        public IActionResult UpdateWorkOrder(DTOWordOrder WorkOrder)
        {
            try
            {
                var existingWorkOreder = _workOrderService.Get(WorkOrder.Id);
                if (existingWorkOreder != null)
                {
                    var workOrderData = _mapper.Map<WorkOrder>(WorkOrder);
                    if(existingWorkOreder.WorkOrderStatus != workOrderData.WorkOrderStatus)
                    {
                        workOrderData.LastStatusChangeDate = DateTime.Now;
                    }
                    _workOrderService.Update(workOrderData);

                    HandleWorkOrderLineItems(WorkOrder.LineItems, workOrderData);

                    HandleWorkOrderTeamMembers(WorkOrder.TeamMemberId, workOrderData);

                    HandleWorkOrderSubContractors(WorkOrder.SubContractorId, workOrderData);

                    HandleWorkOrderNotesAndAttachments(WorkOrder.Notes, workOrderData);

                    _eventAutomationService.ProcessEventBasedAutomation<WorkOrder>("modified", "workorder", WorkOrder.Id);
                    return Ok(new { message = "Updated Successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Invalid input" });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An error occurred. Please try again later." });
            }
        }


        [HttpDelete(nameof(DeleteWorkOrder))]
        public IActionResult DeleteWorkOrder(int workOrderId)
        {
            var existingWorkOrder = _workOrderService.Get(workOrderId);

            if (existingWorkOrder == null)
            {
                return NotFound(new { message = "WorkOrder not found for deletion" });
            }

            // Delete associated LineItems
            var lineItems = _lineItemService.GetAll().Where(x => x.WorkOrderId == workOrderId);
            foreach (var lineItem in lineItems)
            {
                _lineItemService.Remove(lineItem);
            }

            // Delete associated TeamMembers
            var teamMembers = _teamMemberService.GetAll().Where(x => x.WorkOrderId == workOrderId);
            foreach (var teamMember in teamMembers)
            {
                _teamMemberService.Remove(teamMember);
            }

            // Delete associated SubContractors
            var subContractors = _subContractorService.GetAll().Where(x => x.WorkOrderId == workOrderId);
            foreach (var subContractor in subContractors)
            {
                _subContractorService.Remove(subContractor);
            }

            // Delete associated Notes and Attachments
            var notes = _noteService.GetAll().Where(x => x.WorkOrderId == workOrderId);
            foreach (var note in notes)
            {
                var attachments = _photosService.GetAll().Where(x => x.NoteId == note.Id);
                foreach (var attachment in attachments)
                {
                    _photosService.Remove(attachment);
                }
                _noteService.Remove(note);
            }

            // Delete the WorkOrder itself
            _workOrderService.Remove(existingWorkOrder);
            _eventAutomationService.ProcessEventBasedAutomation<WorkOrder>("deleted", "workorder", workOrderId);

            return Ok(new { message = "Deleted Successfully" });
        }

        [HttpPost("updateWorkOrderStatusAndWorkflow")]
        public async Task<IActionResult> updateWorkOrderStatusAndWorkflow([FromQuery] long id, long workflowId, long statusId)
        {
            var existing = _workOrderService.Get(id);
            if (existing != null)
            {
                existing.WorkflowId = workflowId;
                existing.WorkOrderStatus = statusId;
                existing.LastStatusChangeDate = DateTime.Now;
                _workOrderService.Update(existing);
                _eventAutomationService.ProcessEventBasedAutomation<WorkOrder>("modified", "workorder", id);

                return Ok(new { message = "Updated Successfully." });
            }
            else
            {
                return NotFound(new { message = "Job not found!" });
            }
        }



        //-------------------------------New Version



        private void HandleWorkOrderLineItems(List<LineItems> lineItems, WorkOrder workOrder)
        {
            var existingLineItems = _lineItemService.GetAll().Where(w => w.WorkOrderId == workOrder.Id).ToList();
            var lineItemsToDelete = existingLineItems.Where(existing => !lineItems.Any(newItem => newItem.Id == existing.Id)).ToList();
            foreach (var item in lineItems)
            {
                if (item.Id == 0 || item.Id == null)
                {
                    var lineItemData = new LineItem
                    {
                        Id = item.Id ?? 0,
                        Description = item.Description,
                        Name = item.Name,
                        Quantity = item.Quantity
                    };
                    lineItemData.WorkOrderId = workOrder.Id;
                    _lineItemService.Insert(lineItemData);
                }
                else
                {
                    var lineItemData = existingLineItems.FirstOrDefault(x => x.Id == item.Id);
                    lineItemData.Description = item.Description;
                    lineItemData.Name = item.Name;
                    lineItemData.Quantity = item.Quantity;
                    lineItemData.WorkOrderId = workOrder.Id;
                    _lineItemService.Update(lineItemData);
                }
            }

            foreach (var lineItemToDelete in lineItemsToDelete)
            {
                _lineItemService.Remove(lineItemToDelete);
            }

        }

        private void HandleWorkOrderTeamMembers(List<long> teamMembers, WorkOrder workOrder)
        {
            var existingTeamMembers = _teamMemberService.GetAll().Where(w => w.WorkOrderId == workOrder.Id).ToList();

            foreach (var teamMemberToDelete in existingTeamMembers)
            {
                _teamMemberService.Remove(teamMemberToDelete);
            }

            foreach (var teamMemberId in teamMembers)
            {
                var teamMemberData = new TeamMembers { TeamMemberId = teamMemberId, WorkOrderId = workOrder.Id };
                _teamMemberService.Insert(teamMemberData);

            }

        }

        private void HandleWorkOrderSubContractors(List<long> subContractors, WorkOrder workOrder)
        {
            var existingSubContractors = _subContractorService.GetAll().Where(w => w.WorkOrderId == workOrder.Id).ToList();
            foreach (var subContractorToDelete in existingSubContractors)
            {
                _subContractorService.Remove(subContractorToDelete);
            }

            foreach (var subContractorId in subContractors)
            {
                var subContractorData = new SubContractor { SubContractorId = subContractorId, WorkOrderId = workOrder.Id };
                _subContractorService.Insert(subContractorData);
            }
        }


        private void HandleWorkOrderNotesAndAttachments(List<DTONote> notes, WorkOrder workOrder)
        {
            var existingNotes = _noteService.GetAll().Where(w => w.WorkOrderId == workOrder.Id).ToList();
            var notesToDelete = existingNotes.Where(existing => !notes.Any(newItem => newItem.Id == existing.Id)).ToList();

            foreach (var item in notes)
            {
                if (item.Id == 0 || item.Id == null)
                {
                    var noteData = new Note
                    {
                        Id = item.Id,
                        Type = item.Type,
                        Content = item.Content,
                    };
                    noteData.WorkOrderId = (int?)workOrder.Id;
                    _noteService.Insert(noteData);
                    foreach (var attachment in item.Attachments)
                    {
                        var attachmentData = _mapper.Map<PhotosEntity>(attachment);
                        attachmentData.WorkOrderId = workOrder.Id;
                        attachmentData.NoteId = noteData.Id;
                        _photosService.Insert(attachmentData);
                    }
                }
                else
                {
                    var noteData = existingNotes.Where(x => x.Id == item.Id).FirstOrDefault();
                    noteData.Type = item.Type;
                    noteData.Content = item.Content;
                    noteData.WorkOrderId = (int?)workOrder.Id;
                    _noteService.Update(noteData);
                    HandleNoteAttachments(item.Attachments, noteData);
                }
            }
            foreach (var noteToDelete in notesToDelete)
            {
                var attachmentsToDelete = _photosService.GetAll().Where(a => a.NoteId == noteToDelete.Id).ToList();
                foreach (var attachmentToDelete in attachmentsToDelete)
                {
                    _photosService.Remove(attachmentToDelete);
                }
                _noteService.Remove(noteToDelete);
            }
        }

        private void HandleNoteAttachments(List<DTOAttachment> attachments, Note note)
        {
            var existingAttachments = _photosService.GetAll().Where(a => a.NoteId == note.Id).ToList();
            foreach (var item in attachments)
            {
                if (item.Id == 0 || item.Id == null)
                {
                    var attachmentData = new PhotosEntity
                    {
                        Id = item.Id,
                        FilePath = item.FilePath,
                    };
                    attachmentData.WorkOrderId = note.WorkOrderId;
                    attachmentData.NoteId = note.Id;
                    _photosService.Insert(attachmentData);
                }
                else
                {
                    var attachmentData = existingAttachments.FirstOrDefault(x => x.Id == item.Id);
                    attachmentData.NoteId = note.Id;
                    attachmentData.FilePath = item.FilePath;
                    attachmentData.WorkOrderId = attachmentData.WorkOrderId ?? note.WorkOrderId;
                    _photosService.Update(attachmentData);
                }
            }

            var attachmentsToDelete = existingAttachments.Where(existing => !attachments.Any(newItem => newItem.Id == existing.Id)).ToList();
            foreach (var attachmentToDelete in attachmentsToDelete)
            {
                _photosService.Remove(attachmentToDelete);
            }
        }



    }
}

