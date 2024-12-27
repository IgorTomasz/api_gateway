using api_gateway.models;
using api_gateway.models.DTOs;
using api_gateway.models.GameDTOs;
using System.Text.Json;

namespace api_gateway.services
{
	public interface IGameService
	{
		public Task<GameResponse> GetAllGames(string endpoint);
		public Task<GameResponse> GetAllGamesByCategory(string endpoint, GameCategory category);
		public Task<ProcessGameResponse> ProcessGame(string endpoint, ProcessGameRequestMicroservice request);
		public Task<HttpResponseModel> GetGameSessionIdByUser(string endpoint, UserGameSessionRequestMicroservice request);
		public Task<HttpResponseModel> CheckIfGameAlreadyEnded(string endpoint, Guid gameSessionId);
		public Task<ProcessGameStartResponse> ProcessGameStart(string endpoint, ProcessGameRequestMicroservice request);
	}
	public class GameService : IGameService
	{
		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _options;
		private readonly ILogService _logger;

		public GameService(HttpClient httpClient, IConfiguration configuration, ILogService logService)
		{
			_httpClient = httpClient;
			_options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};
			_logger = logService;
		}

		public async Task<GameResponse> GetAllGames(string endpoint)
		{
			var resp = await _httpClient.GetAsync(endpoint);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<GameResponse>(content, _options);
		}

		public async Task<GameResponse> GetAllGamesByCategory(string endpoint, GameCategory category)
		{
			var resp = await _httpClient.GetAsync($"{endpoint}/{((int)category)}");
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<GameResponse>(content, _options);
		}

		public async Task<HttpResponseModel> CheckIfGameAlreadyEnded(string endpoint, Guid gameSessionId)
		{
			var resp = await _httpClient.GetAsync($"{endpoint}/{gameSessionId}");
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<HttpResponseModel>(content, _options);
		}

		public async Task<ProcessGameResponse> ProcessGame(string endpoint, ProcessGameRequestMicroservice request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<ProcessGameResponse>(content, _options);
		}

		public async Task<ProcessGameStartResponse> ProcessGameStart(string endpoint, ProcessGameRequestMicroservice request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<ProcessGameStartResponse>(content, _options);
		}

		public async Task<HttpResponseModel> GetGameSessionIdByUser(string endpoint, UserGameSessionRequestMicroservice request)
		{
			var resp = await _httpClient.PostAsJsonAsync(endpoint, request);
			resp.EnsureSuccessStatusCode();
			var content = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<HttpResponseModel>(content, _options);
		}
	}
}
