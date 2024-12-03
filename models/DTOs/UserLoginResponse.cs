namespace api_gateway.models.DTOs
{
	public class UserLoginResponse
	{
		public bool Success { get; set; }
		public string Error { get; set; } = string.Empty;
		public Guid UserId { get; set; }
	}
}
