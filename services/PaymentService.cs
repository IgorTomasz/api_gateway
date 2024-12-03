using System.Text.Json;

namespace api_gateway.services
{
	public interface IPaymentService
	{

	}
	public class PaymentService : IPaymentService
	{
		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _options;
		private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

		public PaymentService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_headers.Add("X-Int-Secret", configuration["ExtraHeaders:X-Int-Secret"]);
			_options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};
		}


	}
}
