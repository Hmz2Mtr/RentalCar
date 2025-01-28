using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RentalCar.Models
{
    public class Car
    {
        [Key]
        public int CarID { get; set; }

        [Required]
        [StringLength(50)]
        public required string Marque { get; set; }

        [Required]
        [StringLength(50)]
        public required string Model { get; set; }

        [StringLength(20)]
        public string? Color { get; set; }
        
       
        [StringLength(255)]
        public string? ImageURL { get; set; }


        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }


    }
}
