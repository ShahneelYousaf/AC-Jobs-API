using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    // refactor this controller for all delete and update cases

    public class BoardsController : ControllerBase
    {
        private readonly ICustomService<BoardEntity> _boardService;
        private readonly ICustomService<FolderEntity> _folderService;
        private readonly ICustomService<Job> _customJobService;
        private readonly ICustomService<WorkOrder> _customWorkorderService;
        //private readonly ICustomService<Job> _customJobService;
        private readonly ICustomService<BoardAccessUserEntity> _boardAccessUserService;
        private readonly ICustomService<BoardStatusEntity> _boardStatusService;
        private readonly ICustomService<BoardWorkFlowStatusEntity> _boardWorkFlowService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;
        private readonly IContactService _contactService;
        private readonly ICustomService<RelatedContacts> _relatedContacts;
        private readonly ICustomService<TeamMembers> _teammembers;

        public BoardsController(
            ICustomService<BoardEntity> boardService,
            ICustomService<BoardAccessUserEntity> boardAccessUserService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ApplicationDbContext db,
            ICustomService<BoardStatusEntity> boardStatusService,
            ICustomService<BoardWorkFlowStatusEntity> boardWorkFlowService,
            ICustomService<Job> customJobService,
            ICustomService<WorkOrder> customWorkorderService,
            IContactService contactService,
            ICustomService<FolderEntity> folderService,
            ICustomService<RelatedContacts> relatedContacts,
            ICustomService<TeamMembers> teammembers)
        {
            _boardService = boardService;
            _boardAccessUserService = boardAccessUserService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _db = db;
            _boardStatusService = boardStatusService;
            _boardWorkFlowService = boardWorkFlowService;
            _customJobService = customJobService;
            _customWorkorderService = customWorkorderService;
            _contactService = contactService;
            _folderService = folderService;
            _relatedContacts = relatedContacts;
            _teammembers = teammembers;
        }

        [HttpGet(nameof(GetBoardByIdAsync))]
        public async Task<IActionResult> GetBoardByIdAsync(int Id)
        {
            try
            {
                var boardData = _mapper.Map<DTOBoard>(_boardService.Get(Id));

                if (boardData == null)
                {
                    return NotFound();
                }
                else
                {
                    boardData.AccessUsers = _mapper.Map<List<DTOBoardAccessUser>>(
                                                        _boardAccessUserService
                                                            .GetAllAsync()
                                                            .Where(x => x.BoardId == boardData.Id)
                                                    ).ToList();

                    var statusess = _boardStatusService.GetAllAsync();
                    boardData.Statuses = _mapper.Map<List<DTOBoardStatus>>(statusess.Where(x => x.BoardId == boardData.Id).ToList());

                    foreach (var item in boardData.Statuses)
                    {
                        item.WorkFlowStatuses = _mapper.Map<List<DTOBoardWorkFlowStatus>>(_boardWorkFlowService.GetAllAsync().Where(x => x.StatusId == item.Id).ToList());
                        // Initialize item.Items as a list before the loop
                        item.Items = new List<dynamic>();

                        foreach (var workFlow in item.WorkFlowStatuses)
                        {
                            List<dynamic> items;

                            switch (boardData.ProjectType)
                            {
                                case "job":
                                    var states = await _contactService.GetStateAsync();

                                    var jobs = _customJobService.GetAllAsync().Where(x => x.JobStatusId == workFlow.WorkFlowStatusId).ToList();
                                    var jobsData = _mapper.Map<List<DTOJobs>>(jobs);
                                    foreach (var job in jobsData)
                                    {
                                        if (states?.payload != null)
                                        {
                                            var state = states.payload.FirstOrDefault(x => x.Id == job.StateId);
                                            if (state != null)
                                            {
                                                job.StateName = state.Name;
                                            }
                                        }

                                        if (job.SalesRepsentativeId != null)
                                        {
                                            List<long> salesRepIds = new() { (long)job.SalesRepsentativeId };
                                            var saleReps = await _contactService.GetSalesRepresentativeByIdsAsync(salesRepIds);
                                            if (saleReps?.payload != null)
                                            {
                                                var saleRep = saleReps.payload.FirstOrDefault(x => x.Id == job.SalesRepsentativeId);
                                                if (saleRep != null)
                                                {
                                                    job.SalesReps = saleRep;
                                                }
                                            }
                                        }

                                        job.TeamMememberId = _teammembers.GetAll().Where(x => x.JobId == job.Id).Select(x => x.TeamMemberId).ToList();
                                        job.RelatedContactId = _relatedContacts.GetAll().Where(x => x.JobId == job.Id).Select(x => x.RelatedContactId).ToList();
                                        if (job.TeamMememberId.Count > 0)
                                        {
                                            job.TeamMememberId.ForEach(id =>
                                            {
                                                job.TeamMembers.Add(new AC_Jobs_API_Service_Layer.Models.DetailDto()
                                                {
                                                    Id = id,
                                                    Name = _contactService.GetTeamMemberNameAsync(id)
                                                });
                                            });
                                        }
                                    }
                                    items = _mapper.Map<List<dynamic>>(jobsData);
                                    break;
                                case "workorder":
                                    var states1 = await _contactService.GetStateAsync();

                                    var workorders = _customWorkorderService.GetAllAsync().Where(x => x.WorkOrderStatus == workFlow.WorkFlowStatusId).ToList();
                                    var extendedWorkOrders = workorders.Select(workOrder =>
                                    {
                                        var dto = _mapper.Map<WorkOrderBoardDTO>(workOrder);
                                        
                                        var workOrdersJob = _customJobService.Get((long)workOrder.JobId);
                                        if (workOrdersJob != null)
                                        {
                                            if (states1?.payload != null)
                                            {
                                                var state = states1.payload.FirstOrDefault(x => x.Id == workOrdersJob.StateId);
                                                if (state != null)
                                                {
                                                    dto.StateName = state.Name;
                                                }
                                            }

                                            if (dto.SalesRepsentativeId != null)
                                            {
                                                List<long> salesRepIds = new() { (long)workOrdersJob.SalesRepsentativeId };
                                                var saleReps = _contactService.GetSalesRepresentativeByIdsAsync(salesRepIds).Result;
                                                if (saleReps?.payload != null)
                                                {
                                                    var saleRep = saleReps.payload.FirstOrDefault(x => x.Id == workOrdersJob.SalesRepsentativeId);
                                                    if (saleRep != null)
                                                    {
                                                        dto.SalesReps = saleRep;
                                                    }
                                                }
                                            }

                                            dto.TeamMememberId = _teammembers.GetAll().Where(x => x.JobId == workOrdersJob.Id).Select(x => x.TeamMemberId).ToList();
                                            dto.RelatedContactId = _relatedContacts.GetAll().Where(x => x.JobId == workOrdersJob.Id).Select(x => x.RelatedContactId).ToList();
                                            if (dto.TeamMememberId.Count > 0)
                                            {
                                                dto.TeamMememberId.ForEach(id =>
                                                {
                                                    dto.TeamMembers.Add(new AC_Jobs_API_Service_Layer.Models.DetailDto()
                                                    {
                                                        Id = id,
                                                        Name = _contactService.GetTeamMemberNameAsync(id)
                                                    });
                                                });
                                            }
                                        }

                                        return dto;
                                    }).ToList();

                                    items = _mapper.Map<List<dynamic>>(extendedWorkOrders);
                                    break;
                                case "contact":
                                    items = _mapper.Map<List<dynamic>>(_contactService.GetContactsByWorkflowIdAndStatusIdAsync(workFlow.WorkFlowId.ToString(), workFlow.WorkFlowStatusId.ToString()).Result);
                                    break;
                                default:
                                    items = new List<dynamic>();
                                    break;
                            }

                            // Add items to the existing list in each iteration
                            item.Items.AddRange(items);
                        }


                    }
                    var ddddd = JsonConvert.SerializeObject(boardData);
                    return Ok(boardData);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet(nameof(GetAllBoards))]
        public IActionResult GetAllBoards()
        {
            var boardData = _boardService.GetAllAsync().Where(x => x.FolderId == null).ToList();

            if (boardData == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var item in boardData)
                {
                    item.AccessUsers = _boardAccessUserService.GetAllAsync()
                    .Where(x => x.BoardId == item.Id).ToList();
                    var statusess = _boardStatusService.GetAllAsync();
                    item.Statuses = statusess.Where(x => x.BoardId == item.Id).ToList();
                    foreach (var status in item.Statuses)
                    {
                        status.WorkFlowStatuses = _boardWorkFlowService.GetAllAsync().Where(x => x.StatusId == status.Id).ToList();
                    }
                }
                return Ok(boardData);
            }
        }

        //Method to get statuses and workflow statuses bases on board id
        [HttpGet(nameof(GetBoardStatuses))]
        public IActionResult GetBoardStatuses(int boardId)
        {
            var statuses = _boardStatusService.GetAllAsync().Where(x => x.BoardId == boardId && x.IsDeleted != true).ToList();
            if (statuses == null)
            {
                return NotFound();
            }
            else
            {
                var statusessData = _mapper.Map<List<DTOBoardStatus>>(statuses);
                foreach (var item in statusessData)
                {
                    var boardWorkFlowStatuses = _boardWorkFlowService.GetAllAsync().Where(x => x.StatusId == item.Id && x.IsDeleted != true).ToList();
                    item.WorkFlowStatuses = _mapper.Map<List<DTOBoardWorkFlowStatus>>(boardWorkFlowStatuses);
                }
                // return anonymous object with statuses and workflow statuses
                // Add List of contacts or jobs or workorders in every boardData.Statuses having workflow id == item.WorkFlowStatuses.workFlowId and status == item.WorkFlowStatuses.workFlowStatusId

                return Ok(statusessData);
            }
        }

        [HttpPost(nameof(CreateBoard))]
        public IActionResult CreateBoard(DTOCreateBoard boardDto)
        {
            if (boardDto != null)
            {

                var boardData = new BoardEntity
                {
                    BackgroundImageUrl = boardDto.BackgroundImageUrl,
                    CardTitle = boardDto.CardTitle,
                    ProjectColor = boardDto.ProjectColor,
                    ProjectName = boardDto.ProjectName,
                    ProjectType = boardDto.ProjectType,
                    FolderId = boardDto.FolderId,
                };
                var createdBoard = _boardService.Insert(boardData);

                foreach (var userId in boardDto.AccessUsers)
                {
                    _boardAccessUserService.Insert(new BoardAccessUserEntity
                    {
                        BoardId = createdBoard.Id,
                        AccessUserId = userId,
                    });
                }

                foreach (var status in boardDto.Statuses)
                {
                    var createdStatus = _boardStatusService.Insert(new BoardStatusEntity
                    {
                        BoardId = createdBoard.Id,
                        SortBy = status.SortBy,
                        SortingOrder = status.SortingOrder,
                        Name = status.Name,
                        Total = status.Total,
                    });

                    foreach (var workflow in status.WorkFlowStatuses)
                    {
                        _boardWorkFlowService.Insert(new BoardWorkFlowStatusEntity
                        {
                            WorkFlowStatusId = workflow.WorkFlowStatusId,
                            StatusId = createdStatus.Id,
                            WorkFlowId = workflow.WorkFlowId,
                            statusName = workflow.statusName,
                            workFlowName = workflow.workFlowName
                        });
                    }
                }

                return Ok(new { message = "Created Successfully" });
            }
            else
            {
                return BadRequest(new { message = "Invalid input!" });
            }
        }

        [HttpPost(nameof(UpdateBoard))]
        public IActionResult UpdateBoard(DTOCreateBoard boardDto)
        {
            if (boardDto != null)
            {
                var existingBoard = _boardService.Get((long)boardDto.Id);

                if (existingBoard != null)
                {
                    //map values is existing
                    existingBoard.BackgroundImageUrl = boardDto.BackgroundImageUrl;
                    existingBoard.CardTitle = boardDto.CardTitle;
                    existingBoard.ProjectColor = boardDto.ProjectColor;
                    existingBoard.ProjectName = boardDto.ProjectName;
                    existingBoard.ProjectType = boardDto.ProjectType;
                    existingBoard.FolderId = boardDto.FolderId;

                    _boardService.Update(existingBoard);

                    var existingAccessUsers = _boardAccessUserService
                        .GetAll()
                        .Where(x => x.BoardId == boardDto.Id)
                        .ToHashSet();

                    var newAccessUsers = new HashSet<long>(boardDto.AccessUsers);

                    var addedAccessUsers = newAccessUsers.Except(existingAccessUsers.Select(x => x.AccessUserId)).ToList();
                    var removedAccessUsers = existingAccessUsers.SkipWhile(x => newAccessUsers.Select(x => x).Any(u => u == x.AccessUserId)).ToList();

                    foreach (var userId in addedAccessUsers)
                    {
                        _boardAccessUserService.Insert(new BoardAccessUserEntity
                        {
                            BoardId = (long)boardDto.Id,
                            UserId = userId,
                            CreatedBy = 1, // Replace with actual user ID
                            CreatedDate = DateTime.Now,
                        });
                    }

                    foreach (var userId in removedAccessUsers)
                    {
                        var accessUser = _boardAccessUserService
                            .GetAll()
                            .FirstOrDefault(x => x.BoardId == boardDto.Id && x.AccessUserId == userId.AccessUserId);

                        if (accessUser != null)
                        {
                            _boardAccessUserService.Delete(accessUser);
                        }
                    }

                    var existingStatuses = _boardStatusService
                        .GetAll()
                        .Where(x => x.BoardId == boardDto.Id)
                        .ToHashSet();

                    foreach (var status in boardDto.Statuses)
                    {
                        var existingStatus = existingStatuses.FirstOrDefault(x => x.Id == status.Id);
                        // If status exists, update it
                        if (existingStatus != null)
                        {
                            existingStatus.SortBy = status.SortBy;
                            existingStatus.SortingOrder = status.SortingOrder;
                            existingStatus.Name = status.Name;
                            existingStatus.Total = status.Total;

                            _boardStatusService.Update(existingStatus);

                            var existingWorkFlowStatuses = _boardWorkFlowService
                                .GetAllAsync()
                                .Where(x => x.StatusId == status.Id)
                                .ToHashSet();

                            foreach (var workFlowStatusId in existingWorkFlowStatuses)
                            {
                                var workFlowStatus = _boardWorkFlowService
                                    .GetAllAsync()
                                    .FirstOrDefault(x => x.StatusId == status.Id && x.WorkFlowStatusId == workFlowStatusId.WorkFlowStatusId);
                                if (workFlowStatus != null)
                                {
                                    _boardWorkFlowService.Remove(workFlowStatus);
                                }
                            }

                            foreach (var workflow in status.WorkFlowStatuses)
                            {
                                _boardWorkFlowService.Insert(new BoardWorkFlowStatusEntity
                                {
                                    WorkFlowStatusId = workflow.WorkFlowStatusId,
                                    StatusId = status.Id,
                                    WorkFlowId = workflow.WorkFlowId,
                                    statusName = workflow.statusName,
                                    workFlowName = workflow.workFlowName
                                });
                            }
                        }
                        else
                        {
                            var createdStatus = _boardStatusService.Insert(new BoardStatusEntity
                            {
                                BoardId = (long)boardDto.Id,
                                SortBy = status.SortBy,
                                SortingOrder = status.SortingOrder,
                                Name = status.Name,
                                Total = status.Total,
                            });

                            foreach (var workflow in status.WorkFlowStatuses)
                            {
                                _boardWorkFlowService.Insert(new BoardWorkFlowStatusEntity
                                {
                                    WorkFlowStatusId = workflow.WorkFlowStatusId,
                                    StatusId = createdStatus.Id,
                                    WorkFlowId = workflow.WorkFlowId,
                                    statusName = workflow.statusName,
                                    workFlowName = workflow.workFlowName
                                });
                            }
                        }
                    }

                    return Ok(new { message = "Updated Successfully." });
                }
                else
                {
                    return NotFound(new { message = "Board not found!" });
                }
            }
            else
            {
                return BadRequest(new { message = "Invalid input!" });
            }
        }

        [HttpDelete(nameof(DeleteBoard))]
        public IActionResult DeleteBoard(int boardId)
        {
            var board = _boardService.Get(boardId);

            if (board != null)
            {
                var boardData = _mapper.Map<BoardEntity>(board);
                _boardService.Delete(boardData);

                var accessUsers = _boardAccessUserService
                    .GetAll()
                    .Where(x => x.BoardId == boardId)
                    .ToList();

                foreach (var accessUser in accessUsers)
                {
                    _boardAccessUserService.Delete(accessUser);

                }

                var statuses = _boardStatusService
                   .GetAllAsync()
                   .Where(x => x.BoardId == boardId)
                   .ToList();

                foreach (var boardStatus in statuses)
                {
                    _boardStatusService.Delete(boardStatus);
                    var boardWorkFlowStatuses = _boardWorkFlowService
                        .GetAll()
                        .Where(x => x.StatusId == boardStatus.Id)
                        .ToList();
                    foreach (var boardWorkFlowStatus in boardWorkFlowStatuses)
                    {
                        _boardWorkFlowService.Delete(boardWorkFlowStatus);
                    }
                }
                return Ok(new { message = "Deleted Successfully" });
            }
            else
            {
                return NotFound(new { message = "Board not found!" });
            }
        }

        [HttpPost(nameof(CreateFolder))]
        public IActionResult CreateFolder(DTOCreateFolder folderDto)
        {
            if (folderDto != null)
            {
                var folderData = new FolderEntity
                {
                    Name = folderDto.Name,
                    Color = folderDto.Color,
                    ParentFolderId = folderDto.ParentFolderId,
                };
                var createdFolder = _folderService.Insert(folderData);

                return Ok(new { message = "Created Successfully" });
            }
            else
            {
                return BadRequest(new { message = "Invalid input!" });
            }
        }

        [HttpPost(nameof(UpdateFolder))]
        public IActionResult UpdateFolder(DTOCreateFolder folderDto)
        {
            if (folderDto != null)
            {
                var existingFolder = _folderService.Get((long)folderDto.Id);

                if (existingFolder != null)
                {
                    var folderData = _mapper.Map<FolderEntity>(folderDto);
                    _folderService.Update(folderData);

                    return Ok(new { message = "Updated Successfully." });
                }
                else
                {
                    return NotFound(new { message = "Folder not found!" });
                }
            }
            else
            {
                return BadRequest(new { message = "Invalid input!" });
            }
        }

        [HttpDelete(nameof(DeleteFolder))]
        public IActionResult DeleteFolder(int folderId)
        {
            var folder = _folderService.Get(folderId);
            // Check if folder has any child folders and boards
            var childFolders = _folderService.GetAllAsync().Where(x => x.ParentFolderId == folderId && x.IsDeleted != true).ToList();
            var boards = _boardService.GetAllAsync().Where(x => x.FolderId == folderId && x.IsDeleted != true).ToList();
            if (childFolders.Count > 0 || boards.Count > 0)
            {
                return BadRequest("Folder has child folders or boards!");
            }

            if (folder != null)
            {
                var folderData = _mapper.Map<FolderEntity>(folder);
                _folderService.Delete(folderData);

                return Ok(new { message = "Deleted Successfully" });
            }
            else
            {
                return NotFound(new { message = "Folder not found!" });
            }

        }

        [HttpPost(nameof(MoveBoardToFolder))]
        public IActionResult MoveBoardToFolder(int boardId, int folderId)
        {
            var board = _boardService.Get(boardId);
            if (board != null)
            {
                var boardData = _mapper.Map<BoardEntity>(board);
                boardData.FolderId = folderId;
                _boardService.Update(boardData);
                return Ok(new { message = "Moved Successfully" });
            }
            else
            {
                return NotFound(new { message = "Board not found!" });
            }
        }

        [HttpPost(nameof(MoveFolderToFolder))]
        public IActionResult MoveFolderToFolder(int folderId, int parentFolderId)
        {
            var folder = _folderService.Get(folderId);
            if (folder != null)
            {
                var folderData = _mapper.Map<FolderEntity>(folder);
                folderData.ParentFolderId = parentFolderId;
                _folderService.Update(folderData);
                return Ok(new { message = "Moved Successfully" });
            }
            else
            {
                return NotFound(new { message = "Folder not found!" });
            }
        }

        [HttpGet(nameof(GetAllFolders))]
        public IActionResult GetAllFolders()
        {
            var folderData = _folderService.GetAllAsync().Where(x => x.IsDeleted != true && x.ParentFolderId == null || x.ParentFolderId == 0);
            if (folderData == null)
            {
                return NotFound(new { message = "not found" });
            }
            else
            {
                return Ok(folderData);
            }
        }

        [HttpGet(nameof(GetFolderById))]
        public IActionResult GetFolderById(int Id)
        {
            var folderData = _folderService.Get(Id);

            if (folderData == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(folderData);
            }
        }

        [HttpGet(nameof(GetAllBoardsAndChildFoldersByParentFolderId))]
        public IActionResult GetAllBoardsAndChildFoldersByParentFolderId(long Id)
        {
            var currentFolder = _folderService.Get(Id);
            var folderData = _folderService.GetAllAsync().Where(x => x.ParentFolderId == Id).ToList() ?? new List<FolderEntity>();

            if (currentFolder == null)
            {
                return NotFound();
            }
            else
            {
                var boards = _boardService.GetAllAsync().Where(x => x.FolderId == Id);
                var boardsData = _mapper.Map<List<DTOBoard>>(boards);
                foreach (var item in boardsData)
                {
                    item.AccessUsers = _mapper.Map<List<DTOBoardAccessUser>>(_boardAccessUserService.GetAllAsync()
                                               .Where(x => x.BoardId == item.Id).ToList());

                    var statusess = _boardStatusService.GetAllAsync();
                    item.Statuses = _mapper.Map<List<DTOBoardStatus>>(statusess.Where(x => x.BoardId == item.Id).ToList());
                    foreach (var status in item.Statuses)
                    {
                        status.WorkFlowStatuses = _mapper.Map<List<DTOBoardWorkFlowStatus>>(_boardWorkFlowService.GetAllAsync().Where(x => x.StatusId == status.Id).ToList());
                    }
                }
                return Ok(new
                {
                    currentFolder.Id,
                    currentFolder.ParentFolderId,
                    currentFolder.Name,
                    currentFolder.Color,
                    boardsData,
                    folderData
                });
            }
        }

        [HttpGet(nameof(GetAllBoardsAndMainFolders))]
        public IActionResult GetAllBoardsAndMainFolders()
        {
            var folderData = _folderService.GetAllAsync().Where(x => x.IsDeleted != true && x.ParentFolderId == null || x.ParentFolderId == 0).ToList();

            if (folderData == null)
            {
                return NotFound();
            }
            else
            {
                var boards = _boardService.GetAllAsync().Where(x => x.IsDeleted != true && x.FolderId == null || x.FolderId == 0);
                var boardsData = _mapper.Map<List<DTOBoard>>(boards);
                foreach (var item in boardsData)
                {
                    item.AccessUsers = _mapper.Map<List<DTOBoardAccessUser>>(_boardAccessUserService.GetAllAsync()
                                                                      .Where(x => x.BoardId == item.Id).ToList());

                    var statusess = _boardStatusService.GetAllAsync();
                    item.Statuses = _mapper.Map<List<DTOBoardStatus>>(statusess.Where(x => x.BoardId == item.Id).ToList());
                    foreach (var status in item.Statuses)
                    {
                        status.WorkFlowStatuses = _mapper.Map<List<DTOBoardWorkFlowStatus>>(_boardWorkFlowService.GetAllAsync().Where(x => x.StatusId == status.Id).ToList());
                    }
                }
                return Ok(new { boardsData, folderData });
            }
        }

        //Create method to update statuses by using DTOBoardStatus

        [HttpPost(nameof(UpdateBoardStatuses))]
        public IActionResult UpdateBoardStatuses(DTOBoardStatus boardStatusDto)
        {
            if (boardStatusDto != null)
            {
                var existingBoardStatus = _boardStatusService.Get((long)boardStatusDto.Id);

                if (existingBoardStatus != null)
                {
                    //map values is existing
                    existingBoardStatus.SortBy = boardStatusDto.SortBy;
                    existingBoardStatus.SortingOrder = boardStatusDto.SortingOrder;
                    existingBoardStatus.Name = boardStatusDto.Name;
                    existingBoardStatus.Total = boardStatusDto.Total;

                    _boardStatusService.Update(existingBoardStatus);

                    var existingWorkFlowStatuses = _boardWorkFlowService
                        .GetAllAsync()
                        .Where(x => x.StatusId == boardStatusDto.Id)
                        .ToHashSet();

                    foreach (var workFlowStatusId in existingWorkFlowStatuses)
                    {
                        var workFlowStatus = _boardWorkFlowService
                            .GetAllAsync()
                            .FirstOrDefault(x => x.StatusId == boardStatusDto.Id && x.WorkFlowStatusId == workFlowStatusId.WorkFlowStatusId);
                        if (workFlowStatus != null)
                        {
                            _boardWorkFlowService.Remove(workFlowStatus);
                        }
                    }

                    foreach (var workflow in boardStatusDto.WorkFlowStatuses)
                    {
                        _boardWorkFlowService.Insert(new BoardWorkFlowStatusEntity
                        {
                            WorkFlowStatusId = workflow.WorkFlowStatusId,
                            StatusId = boardStatusDto.Id,
                            WorkFlowId = workflow.WorkFlowId,
                            statusName = workflow.statusName,
                            workFlowName = workflow.workFlowName
                        });
                    }

                    return Ok(new { message = "Updated Successfully." });
                }
                else
                {
                    return NotFound(new { message = "Board Status not found!" });
                }
            }
            else
            {
                return BadRequest(new { message = "Invalid input!" });
            }
        }

        //Method to delete status and its workflowstatuses by id
        [HttpDelete(nameof(DeleteBoardStatus))]
        public IActionResult DeleteBoardStatus(int boardStatusId)
        {
            var boardStatus = _boardStatusService.Get(boardStatusId);

            if (boardStatus != null)
            {
                _boardStatusService.Remove(boardStatus);

                var boardWorkFlowStatuses = _boardWorkFlowService
                    .GetAll()
                    .Where(x => x.StatusId == boardStatusId)
                    .ToList();

                foreach (var boardWorkFlowStatus in boardWorkFlowStatuses)
                {
                    _boardWorkFlowService.Remove(boardWorkFlowStatus);
                }
                return Ok(new { message = "Deleted Successfully" });
            }
            else
            {
                return NotFound(new { message = "Board Status not found!" });
            }
        }



    }
}
