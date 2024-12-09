using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.DTOs
{
	public class ChangeUserPasswordUserMicroservice
	{
		[Required]
		public Guid userId {  get; set; }
		[Required]
		public string newPassword { get; set; }
	}
}
