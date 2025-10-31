using GiftOfTheGiver_Foundation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGiver_Foundation.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<MonetaryDonation> MonetaryDonation { get; set; }
        public DbSet<ResourceDonation> ResourceDonation { get; set; }
    }
}
