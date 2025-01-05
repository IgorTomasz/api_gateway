using api_gateway.models;
using api_gateway.models.AccountDTOs;
using api_gateway.models.DTOs;
using System.Text.Json;

namespace api_gateway.services
{
	public interface IAccountService
	{
		public Task<HttpResponseModel> RegisterUser(string endpoint, UserRegisterRequest request);
		public Task<UserLoginResponse> LoginUser(string endpoint, UserLoginRequest request);
		public Task<Guid> CreateUserSession(string endpoint, UserCreateSessionRequest request);
		public Task<UserProfileResponse> GetUserProfile(string endpoint, Guid userId);
		public Task<List<UserResponse>> GetAllUsers(string endpoint);
		public Task<HttpResponseModel> GetUserInfo(string endpoint, Guid sessionId);
		public Task<HttpResponseModel> ChangeUserPassword(string endpoint, ChangeUserPasswordUserMicroservice request);
		public Task<HttpResponseModel> UpdateSessionOnRefreshToken(string endpoint, UpdateSessionWithRefTokenRequestMicroservice request);

		public Task<HttpResponseModel> GetUserRefToken(string endpoint, UserRefreshTokenRequestMicroservice request);
	}

	public class AccountService : IAccountService
	{

		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _options;
		private readonly ILogService _logger;

		public AccountService(HttpClient httpClient, ILogService logService)
		{
			_httpClient = httpClient;
			_options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};
			_logger = logService;
		}

		public async Task<List<UserResponse>> GetAllUsers(string endpoint)
		{
			_logger.Log(endpoint, _httpClient.BaseAddress);
			var resp = await _httpClient.GetAsync(endpoint);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<List<UserResponse>>(content, _options);
		}

		public async Task<HttpResponseModel> RegisterUser (string endpoint, UserRegisterRequest request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<HttpResponseModel>(content, _options);
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

		public async Task<HttpResponseModel> GetUserInfo(string endpoint, Guid sessionId)
		{
            var resp = await _httpClient.GetAsync($"{endpoint}/{sessionId}");
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<HttpResponseModel>(content, _options);
        }

		public async Task<HttpResponseModel> ChangeUserPassword(string endpoint, ChangeUserPasswordUserMicroservice request)
		{
			var resp = await _httpClient.PatchAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<HttpResponseModel>(content, _options);
		}

		public async Task<HttpResponseModel> UpdateSessionOnRefreshToken(string endpoint, UpdateSessionWithRefTokenRequestMicroservice request)
		{
			var resp = await _httpClient.PatchAsJsonAsync(endpoint,request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<HttpResponseModel>(content, _options);
		}

		public async Task<HttpResponseModel> GetUserRefToken(string endpoint, UserRefreshTokenRequestMicroservice request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<HttpResponseModel>(content, _options);
		}
	}
}
