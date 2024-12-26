using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebApi.Controllers
{
    /// <summary>
    /// Default base controller with additional routing and controller attributes
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    // [Route("api/[controller]")]
    [ApiController]
    public class CustomControllerBase : ControllerBase { }
}