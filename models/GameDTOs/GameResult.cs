namespace api_gateway.models.GameDTOs
{
	public class GameResult
	{
		public GameStatus Status { get; set; }
		public decimal Multiplier { get; set; }
		public decimal Result { get; set; }
		public Dictionary<string, object>? Data { get; set; }
	}

	public enum GameStatus
	{
		InProgress, EndedWin, EndedLose
	}
}
