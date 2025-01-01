using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Entities
{
    public class City
    {
        [Key]
        public Guid CityId { get; set; }
        [Required]
        public required string CityName { get; set; }
    }
}
