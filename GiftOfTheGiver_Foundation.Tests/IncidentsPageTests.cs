using Bunit;
using Bunit.TestDoubles;
using GiftOfTheGiver_Foundation.Components.Account.Shared;
using GiftOfTheGiver_Foundation.Components.Pages;
using GiftOfTheGiver_Foundation.Data;
using GiftOfTheGiver_Foundation.Models;
using Microsoft.AspNetCore.Components.Web; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Bunit;

namespace GiftOfTheGiver_Foundation.Tests
{
    public class IncidentsPageTests : TestContext
    {
        public IncidentsPageTests()
        {
            // 1. Setup an InMemory Database Factory
            var dbName = System.Guid.NewGuid().ToString();
            Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: dbName));

            // 2. Setup Test Authorization for [Authorize(Roles = "Admin")]
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestAdmin");
            authContext.SetRoles("Admin");
        }

        // Test 1: Verifies that incidents are loaded and button states are correct
        [Fact]
        public async Task IncidentsPage_OnInitialized_LoadsAndDisplaysIncidentsCorrectly()
        {
            // Arrange
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var testDate = DateTime.Now;

            // Seed the database with incidents in different states
            await using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                dbContext.DisasterReports.AddRange(
                    new DisasterReport // Pending
                    {
                        DisasterReportId = 1,
                        IncidentType = "Flood",
                        UrgencyLevel = "High",
                        Description = "Pending incident",
                        IncidentDate = testDate.AddHours(-1),
                        AcknowledgedDate = null,
                        ResolvedDate = null
                    },
                    new DisasterReport // Acknowledged
                    {
                        DisasterReportId = 2,
                        IncidentType = "Wildfire",
                        UrgencyLevel = "Medium",
                        Description = "Acknowledged incident",
                        IncidentDate = testDate.AddHours(-2),
                        AcknowledgedDate = testDate,
                        ResolvedDate = null
                    },
                    new DisasterReport // Resolved
                    {
                        DisasterReportId = 3,
                        IncidentType = "Drought",
                        UrgencyLevel = "Low",
                        Description = "Resolved incident",
                        IncidentDate = testDate.AddHours(-3),
                        AcknowledgedDate = testDate.AddHours(-1),
                        ResolvedDate = testDate
                    }
                );
                await dbContext.SaveChangesAsync();
            }

            // Act
            var cut = RenderComponent<Incidents>();

            // Assert: Wait for the 3 incidents to be loaded and rendered
            cut.WaitForState(() => cut.FindAll("tbody tr").Count == 3, TimeSpan.FromSeconds(5));
            var rows = cut.FindAll("tbody tr");

            // Assert 
            var pendingRow = rows.First(r => r.TextContent.Contains("INC-001"));
            Assert.Contains("Pending", pendingRow.TextContent);
            Assert.False(pendingRow.QuerySelector("button.btn-primary").HasAttribute("disabled")); 
            Assert.True(pendingRow.QuerySelector("button.btn-success").HasAttribute("disabled")); 

            // Assert
            var ackRow = rows.First(r => r.TextContent.Contains("INC-002"));
            Assert.Contains("Acknowledged", ackRow.TextContent);
            Assert.True(ackRow.QuerySelector("button.btn-primary").HasAttribute("disabled")); 
            Assert.False(ackRow.QuerySelector("button.btn-success").HasAttribute("disabled")); 

            // Assert 
            var resRow = rows.First(r => r.TextContent.Contains("INC-003"));
            Assert.Contains("Resolved", resRow.TextContent);
            Assert.True(resRow.QuerySelector("button.btn-primary").HasAttribute("disabled"));
            Assert.True(resRow.QuerySelector("button.btn-secondary").HasAttribute("disabled")); 
        }

        // Test 2: Verifies that clicking "Acknowledge" opens the Assign modal
        [Fact]
        public async Task IncidentsPage_ClickAcknowledge_OpensAssignModal()
        {
            // Arrange
            // Stub the modal to make bUnit render a placeholder tag
            ComponentFactories.AddStub<AssignVolunteersModal>();

            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

            // Seed a single "Pending" incident
            await using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                dbContext.DisasterReports.Add(
                    new DisasterReport // Pending
                    {
                        DisasterReportId = 5,
                        IncidentType = "Flood",
                        UrgencyLevel = "High",
                        Description = "Incident to acknowledge",
                        IncidentDate = DateTime.Now.AddHours(-1),
                        AcknowledgedDate = null,
                        ResolvedDate = null
                    });
                await dbContext.SaveChangesAsync();
            }

            var cut = RenderComponent<Incidents>();
            cut.WaitForState(() => cut.FindAll("tbody tr").Count == 1);

            // Assert: Modal component STUB is not visible initially
            // search for the Stub<T>, not the component T
            Assert.Empty(cut.FindComponents<Stub<AssignVolunteersModal>>());

            // Find the "Acknowledge" button
            var ackButton = cut.Find("button.btn-primary");
            Assert.NotNull(ackButton);
            Assert.False(ackButton.HasAttribute("disabled")); // Ensure it's clickable

            // Act
            await ackButton.ClickAsync(new MouseEventArgs()); // Click the button

            // Assert: The modal STUB is now rendered
            // Use WaitForState to wait for the STUB to appear
            cut.WaitForState(() => cut.FindComponents<Stub<AssignVolunteersModal>>().Count == 1, TimeSpan.FromSeconds(5));

            // Find the STUB
            var modalStub = cut.FindComponent<Stub<AssignVolunteersModal>>();
            Assert.NotNull(modalStub);

            // Verify the correct IncidentId was passed to the stub's parameters
            var passedIncidentId = modalStub.Instance.Parameters.Get<int>(m => m.IncidentId);
            Assert.Equal(5, passedIncidentId);
        }
    }
}