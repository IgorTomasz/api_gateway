namespace api_gateway.models.GameDTOs
{
	public class AdminGameUpdate
	{
		public List<GameActive> gameActives {  get; set; }
	}

	public class GameActive
	{
		public string Id { get; set; }
		public bool IsActive { get; set; }
	}
}
