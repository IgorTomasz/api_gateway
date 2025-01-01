using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace api_gateway.models.GameDTOs
{
	public class ProcessGameRequestMicroservice
	{
		[Required]
		public GameType Type { get; set; }
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public Guid UserSessionId { get; set; }
		public Guid? GameSessionId { get; set; }
		[Required]
		public ActionType Action { get; set; }
		[Required]
		public decimal BetAmount { get; set; }
		[Required]
		public Dictionary<string, object> Data { get; set; }
	}

	public enum GameType
	{
		Mines, Plinko, Frog, Dice, BlackJack
	}

	public enum ActionType
	{
		Start, Move, End, System
	}
}
