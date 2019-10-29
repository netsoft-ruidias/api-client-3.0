namespace Demo.ServiceConsumer.API
{
	using System;
    using Demo.ServiceConsumer.Gateway.Model;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Netsoft.Core.Model;

    public static class ApiConfigExtensions
	{
		public static IServiceCollection AddSettings(this IServiceCollection services)
		{
			services.AddSingleton<IIdentityConfig>(serviceProvider =>
			{
				var config = serviceProvider.GetRequiredService<IConfiguration>();
				var identityConfig = new IdentityConfig();

				config.Bind("IdentityService", identityConfig);

				return identityConfig;
			});

			services.AddSingleton<IPolicyConfig>(serviceProvider =>
			{
				var config = serviceProvider.GetRequiredService<IConfiguration>();
				var policyConfig = new PolicyConfig();

				config.Bind("ClientPolicy", policyConfig);

				return policyConfig;
			});

			return services;
		}
	}
}
