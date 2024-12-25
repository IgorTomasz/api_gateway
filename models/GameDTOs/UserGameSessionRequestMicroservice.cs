using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.GameDTOs
{
	public class UserGameSessionRequestMicroservice
	{
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public Guid UserSessionId { get; set; }
		[Required]
		public GameType GameType { get; set; }
	}
}
