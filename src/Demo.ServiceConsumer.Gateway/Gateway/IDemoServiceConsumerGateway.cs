namespace Demo.ServiceConsumer.Gateway.Gateway
{
	using System.Threading.Tasks;
	using Demo.DemoClient.Model;

	public interface IDemoServiceConsumerGateway
	{
		Task<DummyValueDTO> GetDummyValueAsync(int id);

		Task<DummyUserResponseDTO> CreateDummyUserAsync(DummyUserRequestDTO userRequest);
	}
}