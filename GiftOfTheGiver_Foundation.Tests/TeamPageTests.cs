// GiftOfTheGiver.Tests/TeamPageTests.cs
using Bunit;
using Bunit.TestDoubles;
using GiftOfTheGiver_Foundation.Components.Account.Shared;
using GiftOfTheGiver_Foundation.Components.Pages;
using GiftOfTheGiver_Foundation.Data;
using GiftOfTheGiver_Foundation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using VolunteerModel = GiftOfTheGiver_Foundation.Models.Volunteer;

namespace GiftOfTheGiver_Foundation.Tests
{
    public class TeamPageTests : TestContext
    {
        // Mocks for UserManager
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IUserStore<ApplicationUser>> _mockUserStore;
        private readonly List<ApplicationUser> _testUsers = new List<ApplicationUser>();

        public TeamPageTests()
        {
            // 1. Setup an InMemory Database Factory for LoadVolunteers
            var dbName = System.Guid.NewGuid().ToString();
            Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: dbName));

            // 2. Setup Mocks for Identity (to avoid scoped service error)
            _mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                _mockUserStore.Object, null, null, null, null, null, null, null, null);

            // Configure the mock to return an empty list for LoadUsers
            _mockUserManager.Setup(m => m.Users).Returns(_testUsers.AsQueryable());

            // Register the MOCKED object
            Services.AddSingleton(_mockUserManager.Object);

            // 3. Setup Test Authorization
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestAdmin");
            authContext.SetRoles("Admin");

            // 4. Stub the child modal component
            ComponentFactories.AddStub<LinkUserModal>();
        }

        // Test 1: Verifies that volunteers are loaded and buttons states are correct
        [Fact]
        public async Task TeamPage_OnInitialized_LoadsAndDisplaysVolunteers()
        {
            // Arrange
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await SeedVolunteers(dbFactory);

            // Act
            var cut = RenderComponent<Team>();

            
            // We must wait for the *specific content* to appear, not just a row count.
            // This ensures the test only proceeds *after* the async LoadVolunteers()
            // has finished and the component has re-rendered with the data.
            cut.WaitForState(() => cut.Markup.Contains("Jessica Motaung"), TimeSpan.FromSeconds(5));
            // --- END FIX ---

            // Assert: Now that we know the data is present, we can safely query the DOM.
            var rows = cut.FindAll("tbody tr");

            // Assert (Pending Row - Jessica Motaung)
            // This line (previously 96) will now succeed.
            var pendingRow = rows.First(r => r.TextContent.Contains("Jessica Motaung"));
            Assert.Contains("0810001111", pendingRow.TextContent);
            Assert.Contains("Weekdays - 08:00am to 12:00am", pendingRow.TextContent);
            Assert.Contains("Fundraising", pendingRow.TextContent);
            Assert.NotNull(pendingRow.QuerySelector("button.btn-success")); // Approve button
            Assert.Null(pendingRow.QuerySelector("button.btn-secondary[disabled]")); // Approved button

            // Assert (Approved Row - Thato Radebe)
            var approvedRow = rows.First(r => r.TextContent.Contains("Thato Radebe"));
            Assert.Contains("0821112222", approvedRow.TextContent);
            Assert.Contains("Emergency Response", approvedRow.TextContent);
            Assert.Null(approvedRow.QuerySelector("button.btn-success")); // Approve button
            Assert.NotNull(approvedRow.QuerySelector("button.btn-secondary[disabled]")); // Approved button
        }

        // Helper method to seed database
        private async Task SeedVolunteers(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            await using var dbContext = await dbFactory.CreateDbContextAsync();
            dbContext.Volunteers.AddRange(
                new VolunteerModel // Pending
                {
                    VolunteerName = "Jessica Motaung",
                    VolunteerEmail = "motaung@example.com",
                    VolunteerContact = "0810001111",
                    HelpType = "Fundraising",
                    Availability = "Weekdays-AM",
                    ApprovedDate = null // Pending
                },
                new VolunteerModel // Approved
                {
                    VolunteerName = "Thato Radebe",
                    VolunteerEmail = "radebe@example.com",
                    VolunteerContact = "0821112222",
                    HelpType = "Emergency Response",
                    Availability = "Saturday-All",
                    ApprovedDate = DateTime.UtcNow.AddDays(-10) // Approved
                }
            );
            await dbContext.SaveChangesAsync();
        }
    }
}