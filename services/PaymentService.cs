using api_gateway.models;
using api_gateway.models.DTOs;
using api_gateway.models.PaymentDTOs;
using System.Text.Json;

namespace api_gateway.services
{
	public interface IPaymentService
	{
		public Task<HttpResponseModel> CreateAccount(string endpoint, CreateAccountRequestMicroservice request);
		public Task<HttpResponseModel> HandlePayment(string endpoint, HandlePaymentRequestMicroservice request);
	}
	public class PaymentService : IPaymentService
	{
		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _options;
		private readonly ILogService _logger;

		public PaymentService(HttpClient httpClient, IConfiguration configuration, ILogService logService)
		{
			_httpClient = httpClient;
			_options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};
			_logger= logService;
		}

		public async Task<HttpResponseModel> CreateAccount(string endpoint, CreateAccountRequestMicroservice request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<HttpResponseModel>(content, _options);
		}

		public async Task<HttpResponseModel> HandlePayment(string endpoint, HandlePaymentRequestMicroservice request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			_logger.Log(endpoint, resp.StatusCode);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<HttpResponseModel>(content, _options);
		}
	}
}
