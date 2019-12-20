using Microsoft.AspNetCore.Mvc;
using Security.Attributes;
using System.Collections.Generic;

namespace SecuredHttpClient.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<string>> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		[TrustedHostsOnly]
		public ActionResult<string> Get(int id)
		{
			return $"value - {id}";
		}

		// POST api/values
		[HttpPost]
		[TrustedHostsOnly]
		public IActionResult Post([FromBody] string value)
		{
			return Ok(value);
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
