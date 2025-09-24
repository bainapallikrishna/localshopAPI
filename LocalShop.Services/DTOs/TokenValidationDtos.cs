namespace LocalShop.Models.DTOs
{
	public class TokenValidateRequest
	{
		public string? Token { get; set; }
	}

	public class TokenValidateResponse
	{
		public bool IsValid { get; set; }
		public string? Username { get; set; }
		public string? Email { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
		public DateTime? ExpiresAt { get; set; }
	}
}


