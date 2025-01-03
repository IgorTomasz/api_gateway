using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.GameDTOs
{
	public class ProcessGameRequest
	{
		[Required]
		public Guid UserSessionId { get; set; }
		public Guid GameSessionId { get; set; }
		[Required]
		public ActionType Action { get; set; }
		[Required]
		public decimal BetAmount { get; set; }
		[Required]
		public Dictionary<string, object> Data { get; set; }
	}
}
