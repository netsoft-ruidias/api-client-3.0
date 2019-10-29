namespace Demo.ServiceConsumer.API.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Demo.DemoClient.Model;
    using Demo.ServiceConsumer.Gateway.Gateway;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IDemoServiceConsumerGateway demoServiceConsumerGateway;

        private static readonly string[] values = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public ValuesController(IDemoServiceConsumerGateway demoServiceConsumerGateway)
        {
            this.demoServiceConsumerGateway = demoServiceConsumerGateway;
        }

        /// <summary>
        /// Get Dummy Values from array.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return values;
        }

        /// <summary>
        /// Get Dummy Value from service.
        /// </summary>
        /// <param name="id">The Value Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDummyValueAsync(int id)
        {
            var result = await this.demoServiceConsumerGateway.GetDummyValueAsync(id);

            return this.Ok(result);
        }

        /// <summary>
        /// Create Dummy User from service.
        /// </summary>
        /// <param name="id">The Value Id.</param>
        /// <param name="userRequest">The User model request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("{id}/users")]
        public async Task<IActionResult> PostTestAsync(int id, DummyUserRequestDTO userRequest)
        {
            var result = await this.demoServiceConsumerGateway.CreateDummyUserAsync(userRequest);

            return this.Ok(result);
        }
    }
}
