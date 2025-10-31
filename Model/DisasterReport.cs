using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGiver_Foundation.Models
{
    [Table("DisasterReports")]
    public class DisasterReport
    {
        [Key]
        public int DisasterReportId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Type of Incident")]
        public string IncidentType { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Urgency Level")]
        public string UrgencyLevel { get; set; }

        [Required]
        [Display(Name = "Incident Date & Time")]
        public DateTime IncidentDate { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Province { get; set; }

        [Required]
        [Display(Name = "Detailed Description")]
        public string Description { get; set; }
    }
}