using RentalCar.Models.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalCar.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        //[Required]
        //public string UserID { get; set; } // Foreign key to ApplicationUser

        //[Required]
        //public int CarID { get; set; } // Foreign key to Car

        [Required]
        public string UserName { get; set; } // Store the username

        [Required]
        public string CarImage { get; set; } // Store the car image URL

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } // Single booking date

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public string UserID { get; set; }

        [ForeignKey("CarID")]
        public int CarID { get; set; }
    }
}