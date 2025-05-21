using System.Threading.Tasks;

namespace TimorINSSBackEnd.DataManager.Interfaces
{
    public interface IEmailSenderDataManager
    {
        Task SendEmailAsync(string email, string subject, string token, string? username = null, bool? isInternal = null);

        Task SendEmailNotify(string email, string subject, string content);
    }
}