namespace Demo.DemoClient.Client
{
	using System.Threading.Tasks;
	using Demo.DemoClient.Model;

	public interface IDemoClientSDK
	{
		Task<DummyValueDTO> GetDummyValueAsync(int id);

		Task<DummyUserResponseDTO> CreateDummyUserAsync(DummyUserRequestDTO userRequest);
	}
}