using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using Azure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Service_Layer.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<AutomationResponseDto> SendEmailAsync(string toAddress, string subject, string emailBody)
        {
            if (string.IsNullOrWhiteSpace(toAddress))
            {
                return new AutomationResponseDto
                {
                    success = false,
                    message = "Failed to send email."
                };
            }

            try
            {
                var emailDto = new EmailDto
                {
                    fromAddress = _configuration.GetSection("email").Value,
                    toAddress = toAddress,
                    ccAddress = string.Empty,
                    bccAddress = string.Empty,
                    subject = subject,
                    isHtmlBody = true,
                    body = emailBody
                };

                using (var httpClient = new HttpClient())
                {
                    var emailUrl = _configuration.GetSection("emailUrl").Value;
                    var jsonContent = JsonConvert.SerializeObject(emailDto);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(emailUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return new AutomationResponseDto
                        {
                            success = true,
                            message = "Email has been sent successfully."
                        };
                    }
                    else
                    {
                        return new AutomationResponseDto
                        {
                            success = false,
                            message = "Failed to send email."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new AutomationResponseDto
                {
                    success = false,
                    message = "Failed to send email. An error occurred: " + ex.Message
                };
            }
        }
    }
}
