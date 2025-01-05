using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.AccountDTOs
{
	public class UpdateSessionWithRefTokenRequestMicroservice
	{
		[Required]
		public Guid SessionId { get; set; }
		[Required]
		public string RefToken { get; set; }
	}
}
