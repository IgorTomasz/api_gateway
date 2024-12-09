using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.DTOs
{
	public class UserRefreshTokenRequest
	{
		[Required]
		public Guid SessionId { get; set; }
		[Required]
		public string RefToken { get; set; } = string.Empty;
	}
}
