using System.ComponentModel.DataAnnotations;

namespace CitiesManager.WebApi.Models
{
    public class City
    {
        [Key]
        public Guid CityId { get; set; }
        [Required]
        public required string CityName { get; set; }
    }
}
