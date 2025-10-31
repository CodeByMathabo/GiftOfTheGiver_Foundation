using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGiver_Foundation.Models
{
    [Table("Volunteers")]
    public class Volunteer
    {
        [Key]
        public int VolunteerId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Full Name")]
        public string VolunteerName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "How would you like to help?")]
        public string HelpType { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Contact Number")]
        public string VolunteerContact { get; set; }

        [Required]
        [StringLength(50)]
        public string Availability { get; set; }
        public DateTime? ApprovedDate { get; set; } // Nullable, as it's 'Pending' at first
    }
}