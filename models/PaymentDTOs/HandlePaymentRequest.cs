using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.PaymentDTOs
{
	public class HandlePaymentRequest
	{
		[Required]
		public Guid SessionId { get; set; }
		[Required]
		public decimal Amount { get; set; }
		public string? PaymentMethod { get; set; }
		public Dictionary<string, string>? MetaData { get; set; }
	}
}
