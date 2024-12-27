namespace api_gateway.models.GameDTOs
{
	public class ProcessGameStartResponse
	{
		public bool Success { get; set; }
		public string? Error { get; set; }
		public object? Message { get; set; }
	}
}
