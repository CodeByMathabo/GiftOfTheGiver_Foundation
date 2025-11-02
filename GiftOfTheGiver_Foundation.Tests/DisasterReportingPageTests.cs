using Bunit;
using Bunit.TestDoubles;
using GiftOfTheGiver_Foundation.Components.Pages;
using GiftOfTheGiver_Foundation.Data;
using GiftOfTheGiver_Foundation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GiftOfTheGiver_Foundation.Tests
{
    public class DisasterReportingPageTests : TestContext
    {
        public DisasterReportingPageTests()
        {
            // 1. Setup an InMemory Database Factory
            // This ensures each test run has a fresh, isolated database.
            var dbName = System.Guid.NewGuid().ToString();
            Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: dbName));

            // 2. Setup Test Authorization for the [Authorize(Roles = "Volunteer")] attribute
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Volunteer");

            // 3. Register a mock ILogger, as the component injects it
            Services.AddSingleton(Mock.Of<ILogger<DisasterReporting>>());
        }

        // Test 1: Verifies a successful form submission
        [Fact]
        public async Task HandleSubmit_WhenFormIsValid_SavesToDatabaseAndShowsSuccess()
        {
            // Arrange
            // Get the factory so to create a context for assertions
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var cut = RenderComponent<DisasterReporting>();

            // Truncate testDate to the minute to match the precision of the input
            var now = DateTime.Now;
            var testDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            var testDateString = testDate.ToString("yyyy-MM-ddTHH:mm"); // Format for <InputDate Type="DateTimeLocal">

            // Act
            // Find and fill all required form inputs
            cut.Find("select[id='incidentType']").Change("Wildfire");
            cut.Find("select[id='urgencyLevel']").Change("High");
            cut.Find("input[id='incidentDate']").Change(testDateString);
            cut.Find("textarea[id='description']").Change("A large fire has been spotted near the Helderberg mountains.");

            // Find the form and submit it asynchronously
            var form = cut.Find("select[id='incidentType']").Closest("form");
            await form.SubmitAsync();

            // Assert (UI): Check that the success message is displayed
            var successAlert = cut.Find("div.alert-success");
            Assert.Contains("Your disaster report has been submitted successfully.", successAlert.TextContent);

            // Assert (UI): Check that form fields were reset
            // The code resets to 'new DisasterReport()', so text fields are null
            Assert.Null(cut.Find("textarea[id='description']").GetAttribute("value"));
            Assert.Null(cut.Find("select[id='incidentType']").GetAttribute("value")); // Selects reset to null 

            // Assert (Database): Create a new context to check if the data was saved
            await using var dbContext = await dbFactory.CreateDbContextAsync();
            var savedEntry = dbContext.DisasterReports.FirstOrDefault();

            Assert.NotNull(savedEntry);
            Assert.Equal("Wildfire", savedEntry.IncidentType);
            Assert.Equal(testDate, savedEntry.IncidentDate); // Should be an exact match now
            Assert.Equal("A large fire has been spotted near the Helderberg mountains.", savedEntry.Description);
            Assert.Equal(testDate, savedEntry.IncidentDate, TimeSpan.FromSeconds(1)); // Allow 1s tolerance for DateTime
        }

        // Test 2: Verifies that validation messages appear for an empty form
        [Fact]
        public async Task HandleSubmit_WhenFormIsInvalid_ShowsValidationMessages()
        {
            // Arrange
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var cut = RenderComponent<DisasterReporting>();

            // Act
            // Find the form and submit it without filling in any required data
            var form = cut.Find("select[id='incidentType']").Closest("form");
            await form.SubmitAsync();

            // Assert (UI): Check that validation messages are present
            var validationMessages = cut.FindAll(".validation-message");
            Assert.True(validationMessages.Count > 0);

            // Assert (UI): Check for a specific required field message
            // The model uses [Display(Name = "Detailed Description")]
            var descriptionValidation = validationMessages.FirstOrDefault(m => m.TextContent.Contains("Detailed Description"));
            Assert.NotNull(descriptionValidation);
            Assert.Contains("The Detailed Description field is required.", descriptionValidation.TextContent);

            // Assert (UI): Ensure no success message is shown
            Assert.Empty(cut.FindAll("div.alert-success"));

            // Assert (Database): Ensure nothing was written to the database
            await using var dbContext = await dbFactory.CreateDbContextAsync();
            var entryCount = dbContext.DisasterReports.Count();
            Assert.Equal(0, entryCount);
        }
    }
}