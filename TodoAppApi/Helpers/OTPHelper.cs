namespace TodoAppApi.Helpers
{
    public static class OTPHelper
    {
        public static string GenerateOtp() => new Random().Next(100000, 999999).ToString();
    }
}
