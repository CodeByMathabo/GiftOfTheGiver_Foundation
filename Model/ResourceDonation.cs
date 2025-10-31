using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGiver_Foundation.Models
{
    public class ResourceDonation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string DonorFullName { get; set; } // [cite: 6]

        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } // [cite: 7]

        [Required]
        [Display(Name = "Item Type")]
        public string ItemType { get; set; } // [cite: 8, 9]

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Delivery Option")]
        public string DeliveryOption { get; set; } // [cite: 10, 11]

        public DateTime PledgeDate { get; set; } = DateTime.UtcNow;
    }
}