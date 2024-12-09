namespace api_gateway.models.DTOs
{
	public class HttpResponseModel
	{
		public bool Success { get; set; }
		public string? Error { get; set; }
		public string? Message { get; set; }
	}
}
