using api_gateway.models.DTOs;
using api_gateway.services;
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
		public async Task<IActionResult> RegisterUser(RegisterRequest request)
		{
			var user = await _gatewayService.RegisterUser("account/User/auth/register", request);
			return Created("",user);
		}
	}
}
