using Asp.Versioning;
using CitiesManager.WebApi.DatabaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.WebApi.Controllers.V2
{
    /// <summary>
    /// Action methods for Cities
    /// </summary>
    [ApiVersion("2.0")]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initialize Cities controller with its DbContext
        /// </summary>
        /// <param name="context">DbContext for Cities</param>
        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        /// <summary>
        /// To get list of cities names from Cities table
        /// </summary>
        /// <returns>List of city names</returns>
        [HttpGet]
        [Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<string>>> GetCities()
        {
            return await _context.Cities
                .OrderBy(city => city.CityName)
                .Select(city => city.CityName)
                .ToListAsync();
        }
    }
}
