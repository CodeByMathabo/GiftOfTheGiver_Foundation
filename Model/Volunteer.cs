using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGiver_Foundation.Models
{
    [Table("Volunteers")] // This explicitly names your database table
    public class Volunteer
    {
        [Key] // Marks this as the Primary Key
        public int VolunteerId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Full Name")]
        public string VolunteerName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "How would you like to help?")]
        public string HelpType { get; set; } // Based on id="helpType" [cite: 3]

        [Required]
        [StringLength(50)]
        [Display(Name = "Contact Number")]
        public string VolunteerContact { get; set; }

        [Required]
        [StringLength(50)]
        public string Availability { get; set; } // Based on id="availability" [cite: 6]
    }
}