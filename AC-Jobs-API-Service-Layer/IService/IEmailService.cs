using AC_Jobs_API_Domian_Layer.Models;

namespace AC_Jobs_API_Service_Layer.IService
{
    public interface IEmailService
    {
        Task<AutomationResponseDto> SendEmailAsync(string toAddress, string subject, string emailBody);
    }
}
