using api_gateway.models.DTOs;
using System.Text.Json;

namespace api_gateway.services
{
	public interface IAccountService
	{
		public Task<UserCreatedResponse> RegisterUser(string endpoint, UserRegisterRequest request);
		public Task<UserLoginResponse> LoginUser(string endpoint, UserLoginRequest request);
		public Task<Guid> CreateUserSession(string endpoint, UserCreateSessionRequest request);
		public Task<UserProfileResponse> GetUserProfile(string endpoint, Guid userId);
		public Task<List<UserProfileResponse>> GetAllUsers(string endpoint);
	}

	public class AccountService : IAccountService
	{

		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _options;

		public AccountService(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};
		}

		public async Task<List<UserProfileResponse>> GetAllUsers(string endpoint)
		{
			var resp = await _httpClient.GetAsync(endpoint);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<List<UserProfileResponse>>(content, _options);
		}

		public async Task<UserCreatedResponse> RegisterUser (string endpoint, UserRegisterRequest request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<UserCreatedResponse>(content, _options);
		}

		public async Task<UserLoginResponse> LoginUser(string endpoint, UserLoginRequest request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<UserLoginResponse>(content, _options);
		}

		public async Task<Guid> CreateUserSession(string endpoint,  UserCreateSessionRequest request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<Guid>(content, _options);
		}

		public async Task<UserProfileResponse> GetUserProfile(string endpoint,  Guid userId)
		{
			var resp = await _httpClient.GetAsync($"{endpoint}/{userId}");
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<UserProfileResponse>(content, _options);
		}
	}
}
