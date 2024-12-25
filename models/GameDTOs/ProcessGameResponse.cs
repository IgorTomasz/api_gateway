namespace api_gateway.models.GameDTOs
{
	public class ProcessGameResponse
	{
		public bool Success { get; set; }
		public string Error { get; set; }
		public object Message { get; set; }
	}
}
