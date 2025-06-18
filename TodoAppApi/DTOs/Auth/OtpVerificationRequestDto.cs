namespace TodoAppApi.DTOs.Auth
{
    public class OtpVerificationRequestDto
    {
        public Guid UserId { get; set; }
        public string Otp { get; set; } = default!;
    }
}