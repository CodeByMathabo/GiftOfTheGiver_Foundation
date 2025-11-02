using Bunit;
using Bunit.TestDoubles;
using GiftOfTheGiver_Foundation.Components.Pages;
using GiftOfTheGiver_Foundation.Data;
using GiftOfTheGiver_Foundation.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GiftOfTheGiver_Foundation.Tests
{
    public class AuditDonationPageTests : TestContext
    {
        // ADDED: Inner class for mocking NavigationManager
        private class MockNavigationManager : NavigationManager
        {
            public MockNavigationManager()
            {
                Initialize("http://localhost/", "http://localhost/");
            }
            protected override void NavigateToCore(string uri, bool forceLoad) { }
            protected override void NavigateToCore(string uri, NavigationOptions options) { }
        }

        public AuditDonationPageTests()
        {
            // 1. Setup an InMemory Database Factory
            var dbName = System.Guid.NewGuid().ToString();
            Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: dbName));

            // 2. Setup Test Authorization for [Authorize(Roles = "Admin")]
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestAdmin");
            authContext.SetRoles("Admin");

            // 3. Register the MockNavigationManager
            Services.AddSingleton<NavigationManager>(new MockNavigationManager());
        }

        // Test 1: Verifies that donations are loaded and displayed correctly on init
        [Fact]
        public async Task AuditDonationsPage_OnInitialized_LoadsAndDisplaysDonations()
        {
            // Arrange
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var zaCulture = new CultureInfo("en-ZA");
            var testDate = DateTime.UtcNow.Date;

            // Seed the database with one of each donation type
            await using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                dbContext.MonetaryDonation.Add(new MonetaryDonation
                {
                    Id = 1,
                    DonorFullName = "Test Monetary Donor",
                    Amount = 123.45m,
                    DonationDate = testDate,
                    AuditDate = null // Pending
                });
                dbContext.ResourceDonation.Add(new ResourceDonation
                {
                    Id = 1,
                    DonorFullName = "Test Resource Donor",
                    ContactNumber = "0812345678", // Add required field
                    ItemType = "Canned Goods",
                    Quantity = 50,
                    DeliveryOption = "DropOff", // Add required field
                    PledgeDate = testDate,
                    AuditDate = null // Pending
                });
                await dbContext.SaveChangesAsync();
            }

            // Act
            var cut = RenderComponent<AuditDonation>();

            // Assert: Wait for the component to load data
            cut.WaitForState(() => cut.FindAll("tbody tr").Count > 0, TimeSpan.FromSeconds(5));

            // Assert (Monetary Table)
            // Assert: Find both tables reliably
            var tables = cut.FindAll("table.table-striped");
            Assert.Equal(2, tables.Count); // Ensure both tables are rendered

            var monetaryTable = tables[0];
            var resourceTable = tables[1];

            // Assert (Monetary Table)
            var monetaryRow = monetaryTable.QuerySelector("tbody tr");
            Assert.NotNull(monetaryRow); // Make sure the row was found
            Assert.Contains("MON-001", monetaryRow.TextContent);
            Assert.Contains("Test Monetary Donor", monetaryRow.TextContent);
            Assert.Contains(123.45m.ToString("C", zaCulture), monetaryRow.TextContent);
            Assert.Contains("(Pending)", monetaryRow.TextContent);
            Assert.NotNull(monetaryRow.QuerySelector("button.btn-success")); // "Approve" button exists

            // Assert (Resource Table)
            var resourceRow = resourceTable.QuerySelector("tbody tr");
            Assert.NotNull(resourceRow); // Make sure the row was found
            Assert.Contains("RES-001", resourceRow.TextContent);
            Assert.Contains("Test Resource Donor", resourceRow.TextContent);
            Assert.Contains("Canned Goods", resourceRow.TextContent);
            Assert.Contains("(Pending)", resourceRow.TextContent);
            Assert.NotNull(resourceRow.QuerySelector("button.btn-success")); // "Approve" button exists
        }

        // Test 2: Verifies that clicking "Approve" updates the UI and database
        [Fact]
        public async Task ApproveMonetary_WhenClicked_UpdatesUIAndDatabase()
        {
            // Arrange
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var testId = 7;

            // Seed the database
            await using (var seedContext = await dbFactory.CreateDbContextAsync())
            {
                seedContext.MonetaryDonation.Add(new MonetaryDonation
                {
                    Id = testId,
                    DonorFullName = "Donor To Approve",
                    Amount = 500m,
                    DonationDate = DateTime.UtcNow.AddDays(-1),
                    AuditDate = null // Pending
                });
                await seedContext.SaveChangesAsync();
            }

            var cut = RenderComponent<AuditDonation>();

            // Wait for data to load
            cut.WaitForState(() => cut.FindAll("table:first-of-type tbody tr").Count > 0);

            var approveButton = cut.Find("table:first-of-type tbody tr button.btn-success");
            Assert.NotNull(approveButton);

            // Act
            // Click the "Approve" button
            // Pass new MouseEventArgs() to satisfy the method signature
            await approveButton.ClickAsync(new MouseEventArgs());

            // Assert (UI)
            // The UI should update to show "Approved" and the button is disabled
            var rowAfterClick = cut.Find("table:first-of-type tbody tr");
            var disabledButton = rowAfterClick.QuerySelector("button.btn-secondary");

            Assert.NotNull(disabledButton);
            Assert.Equal("Approved", disabledButton.TextContent);
            Assert.DoesNotContain("(Pending)", rowAfterClick.TextContent); // Pending text is gone
            Assert.Contains(DateTime.UtcNow.ToShortDateString(), rowAfterClick.TextContent); // Today's date is shown

            // Assert (Database)
            // Create a new context to verify the database was updated
            await using (var assertContext = await dbFactory.CreateDbContextAsync())
            {
                var updatedDonation = await assertContext.MonetaryDonation.FindAsync(testId);
                Assert.NotNull(updatedDonation);
                Assert.NotNull(updatedDonation.AuditDate);
                Assert.Equal(DateTime.UtcNow.Date, updatedDonation.AuditDate.Value.Date);
            }
        }
    }
}