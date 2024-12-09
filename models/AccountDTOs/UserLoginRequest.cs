using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.DTOs
{
	public class UserLoginRequest
	{
		[Required]
		public string Username { get; set; }
		[Required] 
		public string Password { get; set; }
		[Required]
		public DateTime Timestamp { get; set; }
	}
}
