using TodoAppApi.Interfaces;

namespace TodoAppApi.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailOtpAsync(string email, string otp)
        {
            Console.WriteLine($"Sending OTP {otp} to {email}");
            return Task.CompletedTask;
        }
    }

}
