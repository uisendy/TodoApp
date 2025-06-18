namespace TodoAppApi.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailOtpAsync(string email, string otp);
    }

}
