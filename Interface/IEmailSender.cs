using System.Threading.Tasks;
using EmailSender.Model;

namespace EmailSender.Interface
{
    public interface IEmailSender
    {
        Task<string> SendEmailAsync(EmailRequest request);
    }
}
