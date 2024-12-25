using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.GameDTOs
{
	public class GameResponse
	{
		public bool Success { get; set; }
		public string Error { get; set; }
		public List<Game> Games { get; set; }
	}

	public class Game
	{
		public int GameId { get; set; }
		public string GameName { get; set; } = string.Empty;
		public GameCategory GameCategory { get; set; }
		public string GameDescription { get; set; } = string.Empty;
		public Dictionary<string, string> GameAdditionalFields { get; set; }
		public bool IsActive { get; set; }
	}
	public enum GameCategory
	{
		Card, Arcade, Random, Strategy
	}
}
