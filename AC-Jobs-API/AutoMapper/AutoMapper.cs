using Ac.Jobs.API.DTos;
using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.Services;
using AutoMapper;

namespace AC_Jobs_API.AutoMapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Event, DTOEvents>().ReverseMap();
            CreateMap<Note, DTOUpdateNote>().ReverseMap();
            CreateMap<Note, DTOCreateNote>().ReverseMap();
            CreateMap<Note, DTOGetNote>().ReverseMap();
            CreateMap<PhotosEntity, DtoNotePhoto>().ReverseMap();
            CreateMap<JobAttachmentsEntity, FileDTO>().ReverseMap();
            CreateMap<PhotosEntity, FileDTO>().ReverseMap();
            CreateMap<Event, DTOCreateEvent>().ReverseMap();
            CreateMap<Job, DTOJobs>().ReverseMap();
            CreateMap<Job, DTODeleteJob>().ReverseMap();
            CreateMap<Job, DTOCreateJobs>().ReverseMap();
            CreateMap<JobsStatus, DTOJobStatus>().ReverseMap();
            CreateMap<JobsStatus, DTOCreateJobStatus>().ReverseMap();
            CreateMap<LeadSource, DTOLeadSource>().ReverseMap();
            CreateMap<LeadSource, DTOCreateLeadSource>().ReverseMap();
            CreateMap<RelatedContacts, DTORelatedContacts>().ReverseMap();
            CreateMap<RelatedContacts, DTOCreateRelatedContacts>().ReverseMap();
            CreateMap<SubContractor, DTOSubContractor>().ReverseMap();
            CreateMap<SubContractor, DTOCreateSubContractor>().ReverseMap();
            CreateMap<Tag, DTOTag>().ReverseMap();
            CreateMap<Tag, DTOCreateTags>().ReverseMap();
            CreateMap<TeamMembers, DTOTeamMembers>().ReverseMap();
            CreateMap<TeamMembers, DTOCreateTeamMembers>().ReverseMap();
            CreateMap<WorkOrder, DTOWordOrder>().ReverseMap();
            CreateMap<WorkOrder, DTOCreateWorkOrder>().ReverseMap();
            CreateMap<WorkFlow, DTOWorkFlow>().ReverseMap();
            CreateMap<WorkFlow, DTOCreateWorkFlow>().ReverseMap();
            CreateMap<BoardEntity, DTOBoard>().ReverseMap();
            CreateMap<BoardEntity, DTOCreateBoard>().ReverseMap();
            CreateMap<BoardAccessUserEntity, DTOBoardAccessUser>().ReverseMap();
            CreateMap<BoardStatusEntity, DTOBoardStatus>().ReverseMap();
            CreateMap<BoardWorkFlowStatusEntity, DTOBoardWorkFlowStatus>().ReverseMap();
            CreateMap<FolderEntity, DTOCreateFolder>().ReverseMap();
            CreateMap<WorkOrder, WorkOrderBoardDTO>().ReverseMap();
            CreateMap<StateResponse, StateResponseDto>().ReverseMap();
            CreateMap<AutomationEntity, AutomationDto>().ReverseMap();
            CreateMap<ConditionEntity, ConditionDto>().ReverseMap();
            CreateMap<ActionEntity, ActionDto>().ReverseMap();
        }
    }
}
