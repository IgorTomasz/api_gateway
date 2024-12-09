using api_gateway.models.DTOs;
using api_gateway.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api_gateway.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GatewayController : ControllerBase
	{
		private readonly IGatewayService _gatewayService;
		public GatewayController(IGatewayService gatewayService)
		{
			_gatewayService = gatewayService;
		}

		[HttpPost("user/register")]
		public async Task<IActionResult> RegisterUser(UserRegisterRequest registerRequest)
		{
			var user = await _gatewayService.RegisterUser("account/User/auth/register", registerRequest);
			return Created("",user);
		}

		[HttpPost("user/login")]
		public async Task<IActionResult> LoginUser(UserLoginRequest loginRequest)
		{
			var isLogged = await _gatewayService.LoginUser("account/User/auth/login", loginRequest);

			if (!isLogged.Success && isLogged.UserId==Guid.Empty)
			{
				return BadRequest(isLogged.Error);
			}

			UserProfileResponse user = await _gatewayService.GetUserProfile("account/User/profile",isLogged.UserId);

			UserTokensResponse tokens = _gatewayService.GenerateJwtTokens(user);

			UserCreateSessionRequest sessionRequest = new UserCreateSessionRequest
			{
				UserId = isLogged.UserId,
				DeviceInfo = Request.Headers["User-Agent"].ToString(),
				IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
				RefToken = tokens.refToken				
			};

			Guid sessionId = await _gatewayService.CreateUserSession("account/UserSession/auth/session/create", sessionRequest);

			return Created("", new
			{
				SessionId = sessionId,
				Token = tokens.jwtToken,
				RefToken = tokens.refToken
			});
		}

		//[Authorize]
		[HttpPost("profile/update-password")]
		public async Task<IActionResult> UpdateUserPassword(ChangeUserPasswordRequest passwordRequest)
		{
			UserInfoResponse userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", passwordRequest.SessionId);

			if (!userInfo.Success)
			{
				return BadRequest(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			HttpResponseModel responseModel = await _gatewayService.ChangeUserPassword("account/User/profile/update-password", new ChangeUserPasswordUserMicroservice { userId = userInfo.UserId, newPassword = passwordRequest.Password });

			if (!responseModel.Success)
			{
				return Conflict(new HttpResponseModel
				{
					Success = false,
					Error = "Something went wrong while changing user password"
				});
			}

			return Ok(responseModel);
		} 

		//[Authorize]
		[HttpGet("profile/{sessionId}")]
		public async Task<IActionResult> GetUserProfile(Guid sessionId)
		{
			UserInfoResponse userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", sessionId);

			if (!userInfo.Success)
			{
				return BadRequest(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			UserProfileResponse user = await _gatewayService.GetUserProfile("account/User/profile", userInfo.UserId);

			return Ok(user);

		}

		[HttpPost("profile/refresh")]
		public async Task<IActionResult> GetNewToken(UserRefreshTokenRequest refRequest)
		{
			UserInfoResponse userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", refRequest.SessionId);

			if (!userInfo.Success)
			{
				return BadRequest(new HttpResponseModel
				{
					Success = false,
					Error = "There is no user connected to that session"
				});
			}

			UserRefreshTokenRequestMicroservice request = new UserRefreshTokenRequestMicroservice
			{
				SessionId = refRequest.SessionId,
				UserId = userInfo.UserId
			};

			HttpResponseModel resp = await _gatewayService.GetUserRefToken("account/UserSession/auth/refresh-token", request);

			if (!resp.Success)
			{
				return BadRequest(new HttpResponseModel
				{
					Success = false,
					Error = resp.Error
				});
			}

			UserProfileResponse user = await _gatewayService.GetUserProfile("account/User/profile", userInfo.UserId);

			if (resp.Message.Equals(refRequest.RefToken))
			{
				UserTokensResponse tokens = _gatewayService.GenerateJwtTokens(user);

				UserCreateSessionRequest sessionRequest = new UserCreateSessionRequest
				{
					UserId = user.userId,
					DeviceInfo = Request.Headers["User-Agent"].ToString(),
					IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
					RefToken = tokens.refToken
				};

				Guid sessionId = await _gatewayService.CreateUserSession("account/UserSession/auth/session/create", sessionRequest);

				return Created("", new
				{
					SessionId = sessionId,
					Token = tokens.jwtToken,
					RefToken = tokens.refToken
				});

			}

			return BadRequest(new HttpResponseModel
			{
				Success = false,
				Error = "There is no matching refresh token"
			});

		}

		//[Authorize(Roles = "Admin")]
		[HttpGet("adm/users")]
		public async Task<IActionResult> GetAllUsers()
		{
			return Ok(await _gatewayService.GetAllUsers("account/User/adm/users"));
		}
	}
}
