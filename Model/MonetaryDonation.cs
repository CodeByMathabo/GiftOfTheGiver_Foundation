using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGiver_Foundation.Models
{
    public class MonetaryDonation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string DonorFullName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Amount (ZAR)")]
        public decimal Amount { get; set; }

        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        public DateTime? AuditDate { get; set; } // Nullable, as it's 'Pending' at first
    }
}