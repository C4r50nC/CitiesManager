using Asp.Versioning;
using CitiesManager.WebApi.DatabaseContext;
using CitiesManager.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.WebApi.Controllers.V1
{
    /// <summary>
    /// Action methods for Cities
    /// </summary>
    [ApiVersion("1.0")]
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
        /// To get list of cities from Cities table
        /// </summary>
        /// <returns>List of Cities</returns>
        [HttpGet]
        // [Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await _context.Cities.OrderBy(city => city.CityName).ToListAsync();
        }

        // GET: api/Cities/5
        /// <summary>
        /// Get city by CityId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>City object that matches with given CityId</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(Guid id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return Problem(detail: "Invalid CityId", statusCode: StatusCodes.Status400BadRequest, title: "City Search");
                // return NotFound();
            }

            return city;
        }

        // Convert ActionResult<T> to IActionResult
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetCity(Guid id)
        //{
        //    var city = await _context.Cities.FindAsync(id);

        //    if (city == null)
        //    {
        //        return Problem(detail: "Invalid CityId", statusCode: StatusCodes.Status400BadRequest, title: "City Search");
        //        // return NotFound();
        //    }

        //    return Ok(city);
        //}

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Update City that matches the provided CityId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="city"></param>
        /// <returns>204 No Content if update is successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(Guid id, [Bind(nameof(City.CityId), nameof(City.CityName))] City city)
        {
            if (id != city.CityId)
            {
                return BadRequest();
            }

            _context.Entry(city).State = EntityState.Modified; // Updates all fields, can be optimized to update needed fields only

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Upload new city object
        /// </summary>
        /// <param name="city"></param>
        /// <returns>Uploaded City object</returns>
        [HttpPost]
        public async Task<ActionResult<City>> PostCity([Bind(nameof(City.CityId), nameof(City.CityName))] City city)
        {
            // Automatically done by [ApiController] attribute
            //if (!ModelState.IsValid)
            //{
            //    return ValidationProblem(ModelState);
            //}

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.CityId }, city);
        }

        // DELETE: api/Cities/5
        /// <summary>
        /// Delete City object with given ID from all cities
        /// </summary>
        /// <param name="id"></param>
        /// <returns>204 No Content if deletion is successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(Guid id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(Guid id)
        {
            return _context.Cities.Any(e => e.CityId == id);
        }
    }
}
