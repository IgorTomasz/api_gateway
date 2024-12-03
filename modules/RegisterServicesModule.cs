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
			}).ConfigureHttpClient((provider, c) =>
			{
				c.DefaultRequestHeaders.Add("X-Int-Secret", configuration["ApiKey:Secret"]);
			});
			services.AddHttpClient<IAccountService, AccountService>(c =>
			{
				c.BaseAddress = new Uri(configuration["Services:AccountService"]);
			}).ConfigureHttpClient((provider, c) =>
			{
				c.DefaultRequestHeaders.Add("X-Int-Secret", configuration["ApiKey:Secret"]);
			});

			return services;
		}
	}
}
