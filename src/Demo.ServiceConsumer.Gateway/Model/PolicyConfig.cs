namespace Demo.ServiceConsumer.Gateway.Model
{
    using System;
    using Netsoft.Core.Model;

	public class PolicyConfig : IPolicyConfig
	{
		public bool HasResilicence => false;

		public TimeSpan HandlerLifetime { get; set; }
	}
}
