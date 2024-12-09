using api_gateway.models;
using api_gateway.models.DTOs;
using api_gateway.models.PaymentDTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace api_gateway.services
{
	public interface IGatewayService
	{
		public Task<HttpResponseModel> RegisterUser(string endpoint, UserRegisterRequest request);
		public Task<UserLoginResponse> LoginUser(string endpoint, UserLoginRequest request);
		public Task<Guid> CreateUserSession(string endpoint, UserCreateSessionRequest request);
		public Task<UserProfileResponse> GetUserProfile(string endpoint, Guid userId);
		public UserTokensResponse GenerateJwtTokens(UserProfileResponse user);
		public Task<List<UserProfileResponse>> GetAllUsers(string endpoint);
		public Task<UserInfoResponse> GetUserInfo(string endpoint, Guid sessionId);
		public Task<HttpResponseModel> ChangeUserPassword(string endpoint, ChangeUserPasswordUserMicroservice request);
		public Task<HttpResponseModel> GetUserRefToken(string endpoint, UserRefreshTokenRequestMicroservice request);
		public Task<HttpResponseModel> CreateAccount(string endpoint, CreateAccountRequestMicroservice request);
		public Task<HttpResponseModel> HandlePayment(string endpoint, HandlePaymentRequestMicroservice request);
	}
	public class GatewayService : IGatewayService
	{
		private readonly IPaymentService _paymentService;
		private readonly IAccountService _accountService;
		private readonly IConfiguration _configuration;

		public GatewayService(IAccountService accountService, IPaymentService paymentService, IConfiguration configuration)
		{
			_paymentService = paymentService;
			_accountService = accountService;
			_configuration = configuration;
		}

		public async Task<HttpResponseModel> CreateAccount(string endpoint, CreateAccountRequestMicroservice request)
		{
			return await _paymentService.CreateAccount(endpoint, request);
		}

		public async Task<HttpResponseModel> HandlePayment(string endpoint, HandlePaymentRequestMicroservice request)
		{
			return await _paymentService.HandlePayment(endpoint, request);
		}

		public async Task<HttpResponseModel> RegisterUser(string endpoint, UserRegisterRequest request)
		{
			return await _accountService.RegisterUser(endpoint, request);
		}

		public async Task<UserLoginResponse> LoginUser(string endpoint, UserLoginRequest request)
		{
			return await _accountService.LoginUser(endpoint, request);
		}

		public async Task<Guid> CreateUserSession(string endpoint, UserCreateSessionRequest request)
		{
			return await _accountService.CreateUserSession(endpoint, request);
		}

		public async Task<UserProfileResponse> GetUserProfile(string endpoint, Guid userId)
		{
			return await _accountService.GetUserProfile(endpoint, userId);
		}

		public async Task<List<UserProfileResponse>> GetAllUsers(string endpoint)
		{
			return await _accountService.GetAllUsers(endpoint);
		}

		public async Task<UserInfoResponse> GetUserInfo(string endpoint, Guid sessionId)
		{
			return await _accountService.GetUserInfo(endpoint, sessionId);
		}

		public async Task<HttpResponseModel> ChangeUserPassword(string endpoint, ChangeUserPasswordUserMicroservice request)
		{
			return await _accountService.ChangeUserPassword(endpoint, request);
		}

		public async Task<HttpResponseModel> GetUserRefToken(string endpoint, UserRefreshTokenRequestMicroservice request)
		{
			return await _accountService.GetUserRefToken(endpoint, request);
		}

		public UserTokensResponse GenerateJwtTokens(UserProfileResponse user)
		{
			var jwtToken = GenerateJwt(user);
			var refToken = GenerateRefToken();

			return new UserTokensResponse
			{
				jwtToken = jwtToken,
				refToken = refToken
			};
		}

		private string GenerateJwt(UserProfileResponse user)
		{

			var userType = "";
			switch (user.userType)
			{
				case UserType.Admin: userType="Admin"; break;
				case UserType.Client: userType="Client"; break;
			}
			var jwtKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

			var policy = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
				new Claim(ClaimTypes.Name, user.username),
				new Claim(ClaimTypes.Role, userType)
			};

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: policy,
				expires: DateTime.UtcNow.AddHours(1).AddMinutes(15),
				signingCredentials: new SigningCredentials(
					new SymmetricSecurityKey(jwtKey),
					SecurityAlgorithms.HmacSha256
					)
				);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private string GenerateRefToken()
		{
			var random = new byte[32];
			using var rand = RandomNumberGenerator.Create();
			rand.GetBytes(random);
			return Convert.ToBase64String(random);
		}
	}
}
