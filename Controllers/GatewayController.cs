using api_gateway.models.DTOs;
using api_gateway.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

		[Authorize]
		[HttpPost("account/User/profile/update-password")]
		public async Task<IActionResult> UpdateUserPassword(ChangeUserPasswordRequest passwordRequest)
		{
			UserInfoResponse userInfo = await _gatewayService.GetUserInfo("account/UserSession/profile/userInfo", passwordRequest.SessionId);

			if (!userInfo.Success)
			{

			}


		} 

		[Authorize(Roles = "Admin")]
		[HttpGet("adm/users")]
		public async Task<IActionResult> GetAllUsers()
		{
			return Ok(await _gatewayService.GetAllUsers("account/User/adm/users"));
		}
	}
}
