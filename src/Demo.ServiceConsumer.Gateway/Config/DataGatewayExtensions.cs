namespace Demo.ServiceConsumer.Gateway
{
	using System;
    using Demo.DemoClient;
	using Demo.ServiceConsumer.Gateway.Gateway;
	using Microsoft.Extensions.DependencyInjection;
    using Netsoft.Core.Model;

    public static class DataGatewayExtensions
	{
		public static IServiceCollection AddDataGateways(this IServiceCollection services)
		{
			services
				.AddDemoClient()
				.AddGateway();

			return services;
		}

		private static IServiceCollection AddDemoClient(this IServiceCollection services)
		{
			var serviceProvider = services.BuildServiceProvider();

			var policyConfig = serviceProvider.GetRequiredService<IPolicyConfig>();
			var identityConfig = serviceProvider.GetRequiredService<IIdentityConfig>();

			services.AddDemoClient(
				new Uri("https://localhost:44348"),
				policyConfig,
				identityConfig);

			return services;
		}

		private static IServiceCollection AddGateway(this IServiceCollection services)
		{
			services.AddTransient<
				IDemoServiceConsumerGateway, 
				DemoServiceConsumerGateway>();

			return services;
		}
	}
}
