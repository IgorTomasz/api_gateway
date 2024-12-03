using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.DTOs
{
	public class UserRegisterRequest
	{
		[Required]
		public string username { get; set; }
		[Required]
		public string name { get; set; }
		[Required]
		public string lastname { get; set; }
		[Required]
		public string passwordHash { get; set; }
		[Required]
		public string email { get; set; }
		public DateOnly dateOfBirth { get; set; }
	}
}
