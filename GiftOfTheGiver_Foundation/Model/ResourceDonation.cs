using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGiver_Foundation.Models
{
    public class ResourceDonation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string DonorFullName { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Required]
        [Display(Name = "Item Type")]
        public string ItemType { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Delivery Option")]
        public string DeliveryOption { get; set; }

        public DateTime PledgeDate { get; set; } = DateTime.UtcNow;

        public DateTime? AuditDate { get; set; } // Nullable, as it's 'Pending' at first
    }
}