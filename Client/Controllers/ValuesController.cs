using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		private readonly IHttpClientFactory _factory;

		public ValuesController(IHttpClientFactory factory)
		{
			_factory = factory;
		}

		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<string>> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var client = _factory.CreateClient("Host");
			var result = await client.GetAsync($"values/{id}");
			var res = await result.Content.ReadAsStringAsync();
			return Ok(res);
		}

		// POST api/values
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] string value)
		{
			var client = _factory.CreateClient("Host");
			var result = await client.PostAsJsonAsync($"values", value + " WOW");
			var res = await result.Content.ReadAsStringAsync();
			return Ok(res);
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
