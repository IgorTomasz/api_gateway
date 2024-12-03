using api_gateway.models.DTOs;

namespace api_gateway.services
{
	public interface IGatewayService
	{
		public Task<UserCreatedResponse> RegisterUser(string endpoint, RegisterRequest request);
	}
	public class GatewayService : IGatewayService
	{
		private readonly IPaymentService _paymentService;
		private readonly IAccountService _accountService;

		public GatewayService(IAccountService accountService, IPaymentService paymentService)
		{
			_paymentService = paymentService;
			_accountService = accountService;
		}

		public async Task<UserCreatedResponse> RegisterUser(string endpoint, RegisterRequest request)
		{
			return await _accountService.RegisterUser(endpoint, request);
		}
	}
}
