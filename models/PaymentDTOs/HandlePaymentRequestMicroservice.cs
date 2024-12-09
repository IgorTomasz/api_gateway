using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.PaymentDTOs
{
	public class HandlePaymentRequestMicroservice
	{
		[Required]
		public Guid UserId { get; set; }
		[Required]
		public decimal Amount { get; set; }
		[Required]
		public Dictionary<string, object> MetaData { get; set; }
	}
}
