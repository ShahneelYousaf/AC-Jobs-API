using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Domian_Layer.Models.Contact;
using AC_Jobs_API_Repository_Layer.IRepository;
using Microsoft.EntityFrameworkCore;
using System;

namespace AC_Jobs_API_Repository_Layer.Repository
{
    public class ContactRepositoryFactory : IContactRepositoryFactory
    {
        private readonly IContactRepository<ContactEntity> _contactRepository;
        private readonly IContactRepository<ContactsRelatedContactEntity> _relatedContactRepository;
        private readonly IContactRepository<ContactTagEntity> _tagRepository;
        private readonly IContactRepository<ContactNoteEntity> _noteRepository;
        private readonly IContactRepository<ContactPhoneNumberEntity> _phoneNumberRepository;
        private readonly IContactRepository<ContactCustomFieldEntity> _customFieldRepository;
        private readonly IContactRepository<ContactTeamMemberEntity> _teamMemberRepository;
        private readonly IContactRepository<ContactsWorkFlowEntity> _workFlowRepository;
        private readonly IContactRepository<ContactStatusEntity> _statusRepository;
        private readonly IContactRepository<TaskEntity> _taskRepository;
        private readonly IContactRepository<TaskContactEntity> _taskContactRepository;
        private readonly IContactRepository<TaskJobEntity> _taskJobRepository;
        private readonly IContactRepository<TaskSubContractorEntity> _taskSubContractorRepository;
        private readonly IContactRepository<TaskTeamMemberEntity> _taskTeamMemberRepository;

        public ContactRepositoryFactory(
            IContactRepository<ContactEntity> contactRepository,
            IContactRepository<ContactsRelatedContactEntity> relatedContactRepository,
            IContactRepository<ContactTagEntity> tagRepository,
            IContactRepository<ContactNoteEntity> noteRepository,
            IContactRepository<ContactPhoneNumberEntity> phoneNumberRepository,
            IContactRepository<ContactCustomFieldEntity> customFieldRepository,
            IContactRepository<ContactTeamMemberEntity> teamMemberRepository,
            IContactRepository<ContactsWorkFlowEntity> workFlowRepository,
            IContactRepository<ContactStatusEntity> statusRepository,
            IContactRepository<TaskEntity> taskRepository,
            IContactRepository<TaskContactEntity> taskContactRepository,
            IContactRepository<TaskJobEntity> taskJobRepository,
            IContactRepository<TaskSubContractorEntity> taskSubContractorRepository,
            IContactRepository<TaskTeamMemberEntity> taskTeamMemberRepository
            )
        {
            _contactRepository = contactRepository;
            _relatedContactRepository = relatedContactRepository;
            _tagRepository = tagRepository;
            _noteRepository = noteRepository;
            _phoneNumberRepository = phoneNumberRepository;
            _customFieldRepository = customFieldRepository;
            _teamMemberRepository = teamMemberRepository;
            _workFlowRepository = workFlowRepository;
            _statusRepository = statusRepository;
            _taskRepository = taskRepository;
            _taskContactRepository = taskContactRepository;
            _taskJobRepository = taskJobRepository;
            _taskSubContractorRepository = taskSubContractorRepository;
            _taskTeamMemberRepository = taskTeamMemberRepository;
        }

        public IContactRepository<TEntity> GetRepository<TEntity>() where TEntity : ContactBaseEntity
        {
            if (typeof(TEntity) == typeof(ContactEntity))
            {
                return (IContactRepository<TEntity>)_contactRepository;
            }
            else if (typeof(TEntity) == typeof(ContactsRelatedContactEntity))
            {
                return (IContactRepository<TEntity>)_relatedContactRepository;
            }
            else if (typeof(TEntity) == typeof(ContactTagEntity))
            {
                return (IContactRepository<TEntity>)_tagRepository;
            }
            else if (typeof(TEntity) == typeof(ContactNoteEntity))
            {
                return (IContactRepository<TEntity>)_noteRepository;
            }
            else if (typeof(TEntity) == typeof(ContactPhoneNumberEntity))
            {
                return (IContactRepository<TEntity>)_phoneNumberRepository;
            }
            else if (typeof(TEntity) == typeof(ContactCustomFieldEntity))
            {
                return (IContactRepository<TEntity>)_customFieldRepository;
            }
            else if (typeof(TEntity) == typeof(ContactTeamMemberEntity))
            {
                return (IContactRepository<TEntity>)_teamMemberRepository;
            }
            else if (typeof(TEntity) == typeof(ContactsWorkFlowEntity))
            {
                return (IContactRepository<TEntity>)_workFlowRepository;
            }
            else if (typeof(TEntity) == typeof(ContactStatusEntity))
            {
                return (IContactRepository<TEntity>)_statusRepository;
            }
            else if (typeof(TEntity) == typeof(TaskEntity))
            {
                return (IContactRepository<TEntity>)_taskRepository;
            }
            else if (typeof(TEntity) == typeof(TaskContactEntity))
            {
                return (IContactRepository<TEntity>)_taskContactRepository;
            }
            else if (typeof(TEntity) == typeof(TaskJobEntity))
            {
                return (IContactRepository<TEntity>)_taskJobRepository;
            }
            else if (typeof(TEntity) == typeof(TaskSubContractorEntity))
            {
                return (IContactRepository<TEntity>)_taskSubContractorRepository;
            }
            else if (typeof(TEntity) == typeof(TaskTeamMemberEntity))
            {
                return (IContactRepository<TEntity>)_taskTeamMemberRepository;
            }
            else
            {
                throw new ArgumentException("Unsupported entity type");
            }
        }
    }
}
