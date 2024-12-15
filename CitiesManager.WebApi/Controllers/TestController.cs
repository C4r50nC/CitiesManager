using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hello, world!";
        }
    }
}
