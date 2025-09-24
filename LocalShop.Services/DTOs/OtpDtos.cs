namespace LocalShop.Models.DTOs
{
    public class OtpRequest
    {
        public string? Username { get; set; }
    }

    public class OtpVerify
    {
        public string? Username { get; set; }
        public string? Otp { get; set; }
    }
}


