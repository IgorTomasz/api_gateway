using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.DTOs
{
	public class UserRefreshTokenRequestMicroservice
	{
		[Required]
		public Guid SessionId { get; set; }
		[Required]
		public Guid UserId { get; set; }
	}
}
