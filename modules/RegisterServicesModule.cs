using api_gateway.services;

namespace api_gateway.modules
{
	public static class RegisterServicesModule
	{
		public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
		{

			services.AddHttpClient<IPaymentService, PaymentService>(c =>
			{
				c.BaseAddress = new Uri(configuration["Services:PaymentService"]);
			});
			services.AddHttpClient<IAccountService, AccountService>(c =>
			{
				c.BaseAddress = new Uri(configuration["Services:AccountService"]);
			});

			return services;
		}
	}
}
