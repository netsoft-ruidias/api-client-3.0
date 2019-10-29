namespace Demo.DemoClient.Client
{
    using System.Net.Http;
    using System.Threading.Tasks;
	using Demo.DemoClient.Model;
	using Netsoft.Core.Clients;

	public class DemoClientSDK : HttpClientBase, IDemoClientSDK
	{
		public DemoClientSDK(HttpClient httpClient)
			: base(httpClient)
		{ }

		public async Task<DummyValueDTO> GetDummyValueAsync(int id)
		{
			var result = await this.GetAsync<DummyValueDTO>($"api/Values/{id}");
			return result.Body;
		}

		public async Task<DummyUserResponseDTO> CreateDummyUserAsync(DummyUserRequestDTO userRequest)
		{
			var result = await this.PostAsync<DummyUserResponseDTO, DummyUserRequestDTO>($"api/Users", userRequest);
			return result.Body;
		}
	}
}
