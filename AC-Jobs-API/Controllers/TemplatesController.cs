using AC_Jobs_API.DTos;
using AC_Jobs_API_Domian_Layer.Data;
using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AC_Jobs_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly ICustomService<EmailTemplateEntity> _emailTemplateService;
        private readonly ICustomService<SMSTemplateEntity> _smsTemplateService;
        private readonly IMapper _mapper;

        public TemplatesController(ICustomService<EmailTemplateEntity> emailTemplateService, ICustomService<SMSTemplateEntity> smsTemplateService, IMapper mapper)
        {
            _emailTemplateService = emailTemplateService;
            _smsTemplateService = smsTemplateService;
            _mapper = mapper;
        }

        [HttpGet("email")]
        public IActionResult GetEmailTemplateById(int id)
        {
            var emailTemplate = _emailTemplateService.Get(id);
            if (emailTemplate == null)
                return NotFound(new NewRecord($"Email template with ID {id} not found", null));

            return Ok(new NewRecord("Email template found", emailTemplate));
        }


        [HttpGet("sms")]
        public IActionResult GetSMSTemplateById(int id)
        {
            var smsTemplate = _smsTemplateService.Get(id);
            if (smsTemplate == null)
                return NotFound(new NewRecord($"SMS template with ID {id} not found", null));

            return Ok(new NewRecord("SMS template found", smsTemplate));
        }


        [HttpPost("email/create")]
        public IActionResult CreateEmailTemplate(EmailTemplateEntity emailTemplateDto)
        {
            try
            {
                _emailTemplateService.Insert(emailTemplateDto);
                return Ok(new NewRecord("Email template created successfully", emailTemplateDto));
            }
            catch (Exception ex)
            {
                return BadRequest(new NewRecord($"Error creating email template: {ex.Message}", null));
            }
        }


        [HttpPost("sms/create")]
        public IActionResult CreateSMSTemplate(SMSTemplateEntity smsTemplateDto)
        {
            try
            {
                _smsTemplateService.Insert(smsTemplateDto);
                return Ok(new NewRecord("SMS template created successfully", smsTemplateDto));
            }
            catch (Exception ex)
            {
                return BadRequest(new NewRecord($"Error creating SMS template: {ex.Message}", null));
            }
        }


        [HttpPut("email/update")]
        public IActionResult UpdateEmailTemplate(EmailTemplateEntity emailTemplateDto)
        {
            try
            {
                var existingEmailTemplate = _emailTemplateService.Get(emailTemplateDto.Id);
                if (existingEmailTemplate == null)
                    return NotFound(new NewRecord($"Email template with ID {emailTemplateDto.Id} not found", null));

                existingEmailTemplate.Name = emailTemplateDto.Name;
                existingEmailTemplate.Subject = emailTemplateDto.Subject;
                existingEmailTemplate.Body = emailTemplateDto.Body;

                _emailTemplateService.Update(existingEmailTemplate);
                return Ok(new NewRecord("Email template updated successfully", existingEmailTemplate));
            }
            catch (Exception ex)
            {
                return BadRequest(new NewRecord($"Error updating email template: {ex.Message}", null));
            }
        }


        [HttpPut("sms/update")]
        public IActionResult UpdateSMSTemplate(SMSTemplateEntity smsTemplateDto)
        {
            try
            {
                var existingSMSTemplate = _smsTemplateService.Get(smsTemplateDto.Id);
                if (existingSMSTemplate == null)
                    return NotFound(new NewRecord($"SMS template with ID {smsTemplateDto.Id} not found", null));

                existingSMSTemplate.Name = smsTemplateDto.Name;
                existingSMSTemplate.Message = smsTemplateDto.Message;

                _smsTemplateService.Update(existingSMSTemplate);
                return Ok(new NewRecord("SMS template updated successfully", existingSMSTemplate));
            }
            catch (Exception ex)
            {
                return BadRequest(new NewRecord($"Error updating SMS template: {ex.Message}", null));
            }
        }


        [HttpDelete("email/delete")]
        public IActionResult DeleteEmailTemplate(int id)
        {
            try
            {
                var existingEmailTemplate = _emailTemplateService.Get(id);
                if (existingEmailTemplate == null)
                    return NotFound(new NewRecord($"Email template with ID {id} not found", null));

                _emailTemplateService.Delete(existingEmailTemplate);
                return Ok(new NewRecord("Email template deleted successfully", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new NewRecord($"Error deleting email template: {ex.Message}", null));
            }
        }


        [HttpDelete("sms/delete")]
        public IActionResult DeleteSMSTemplate(int id)
        {
            try
            {
                var existingSMSTemplate = _smsTemplateService.Get(id);
                if (existingSMSTemplate == null)
                    return NotFound(new NewRecord($"SMS template with ID {id} not found", null));

                _smsTemplateService.Delete(existingSMSTemplate);
                return Ok(new NewRecord("SMS template deleted successfully", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new NewRecord($"Error deleting SMS template: {ex.Message}", null));
            }
        }

        [HttpGet("getAllEmailTemplates")]
        public IActionResult GetAllEmailTemplates()
        {
            try
            {
                var emailTemplates = _emailTemplateService.GetAll();
                if (emailTemplates == null || !emailTemplates.Any())
                    return NotFound(new NewRecord("No email templates found", null));

                return Ok(new NewRecord("Email templates found", emailTemplates));
            }
            catch (Exception ex)
            {
                return BadRequest(new NewRecord($"Error fetching email templates: {ex.Message}", null));
            }
        }

        [HttpGet("getAllSMSTemplates")]
        public IActionResult GetAllSMSTemplates()
        {
            try
            {
                var smsTemplates = _smsTemplateService.GetAll();
                if (smsTemplates == null || !smsTemplates.Any())
                    return NotFound(new NewRecord("No SMS templates found", null));

                return Ok(new NewRecord("SMS templates found", smsTemplates));
            }
            catch (Exception ex)
            {
                return BadRequest(new NewRecord($"Error fetching SMS templates: {ex.Message}", null));
            }
        }



    }
}
