using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.DTOs
{
	public class UserCreateSessionRequest
	{
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public string DeviceInfo { get; set; }
		public string IpAddress { get; set; }
		[Required]
		public string RefToken { get; set; }
	}
}
