namespace Demo.ServiceConsumer.Gateway.Gateway
{
    using System.Threading.Tasks;
    using Demo.DemoClient.Client;
    using Demo.DemoClient.Model;

    public class DemoServiceConsumerGateway : IDemoServiceConsumerGateway
	{
		private readonly IDemoClientSDK demoClientSDK;

		public DemoServiceConsumerGateway(IDemoClientSDK demoClientSDK)
		{
			this.demoClientSDK = demoClientSDK;
		}

		public async Task<DummyValueDTO> GetDummyValueAsync(int id)
		{
			return await this.GetDummyValueAsync(id);
		}

		public async Task<DummyUserResponseDTO> CreateDummyUserAsync(DummyUserRequestDTO userRequest)
		{
			return await this.CreateDummyUserAsync(userRequest);
		}
	}
}
