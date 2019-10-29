namespace Demo.DemoClient
{
	using System;
    using Demo.DemoClient.Client;
    using Demo.DemoClient.OAuth;
    using Microsoft.Extensions.DependencyInjection;
    using Netsoft.Core.Extensions;
    using Netsoft.Core.Model;

    public static class ServiceConfigExtensions
	{
		public static IServiceCollection AddDemoClient(this IServiceCollection services, Uri serviceBaseAddress, IPolicyConfig policyConfig, IIdentityConfig identityConfig)
		{
			services.AddHttpClient<IDemoClientSDK, DemoClientSDK>(x =>
			{
				x.BaseAddress = serviceBaseAddress;
				x.DefaultRequestHeaders.Add("Accept", "application/json");
				x.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
				x.DefaultRequestVersion = new Version(2, 0);
			})
			.AddResilience(policyConfig)
			.AddIdentity(identityConfig, new TokenStrategy());

			return services;
		}
	}


}
