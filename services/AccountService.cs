using api_gateway.models.DTOs;
using System.Text.Json;

namespace api_gateway.services
{
	public interface IAccountService
	{
		public Task<UserCreatedResponse> RegisterUser(string endpoint, RegisterRequest request);
	}

	public class AccountService : IAccountService
	{

		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _options;
		private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

		public AccountService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_headers.Add("X-Int-Secret", configuration["ExtraHeaders:X-Int-Secret"]);
			_options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};
		}

		public async Task<UserCreatedResponse> RegisterUser (string endpoint, RegisterRequest request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<UserCreatedResponse>(content, _options);
		}
	}
}
