using GiftOfTheGiver_Foundation.Data;
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
        [EmailAddress]
        [StringLength(255)]
        [Display(Name = "Email Address")]
        public string VolunteerEmail { get; set; }

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

        // This links the Volunteer record to the ASP.NET Identity user
        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}