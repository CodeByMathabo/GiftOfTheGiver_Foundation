using GiftOfTheGiver_Foundation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGiver_Foundation.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // DbSets for the model files
        public DbSet<MonetaryDonation> MonetaryDonation { get; set; }
        public DbSet<ResourceDonation> ResourceDonation { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<DisasterReport> DisasterReports { get; set; }
        public DbSet<IncidentAssignment> IncidentAssignments { get; set; }
    }
}
