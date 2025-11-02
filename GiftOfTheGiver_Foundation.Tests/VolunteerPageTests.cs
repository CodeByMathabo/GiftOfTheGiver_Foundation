using Bunit;
using Bunit.TestDoubles;
using GiftOfTheGiver_Foundation.Data;
using GiftOfTheGiver_Foundation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using VolunteerPage = GiftOfTheGiver_Foundation.Components.Pages.Volunteer;

namespace GiftOfTheGiver_Foundation.Tests
{
    public class VolunteerPageTests : TestContext
    {
        
        public VolunteerPageTests()
        {
            // 1. Setup an InMemory Database Factory
            // Use a unique name for each test run to ensure isolation
            var dbName = System.Guid.NewGuid().ToString();
            Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: dbName));

            // 2. Setup Test Authorization. This satisfies the [Authorize] attribute.
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Volunteer");
        }

        // Test 1: Verifies a successful form submission
        [Fact]
        public async Task HandleSubmit_WhenFormIsValid_SavesToDatabaseAndShowsSuccess()
        {
            // Arrange
            // Get the factory from the service provider
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var cut = RenderComponent<VolunteerPage>();

            // Act
            // Find and fill all required form inputs
            cut.Find("input[id='volunteerName']").Change("Sandra Lukeman");
            cut.Find("input[id='volunteerEmail']").Change("sandra@example.com");
            cut.Find("select[id='helpType']").Change("Fundraising");
            cut.Find("input[id='volunteerContact']").Change("0821234567");
            cut.Find("select[id='availability']").Change("Weekdays-AM");

            // Find the form using a stable child element
            var form = cut.Find("input[id='volunteerName']").Closest("form");
            await form.SubmitAsync();

            // Assert (UI): Check that the success message is displayed
            var successAlert = cut.Find("div.alert-success");
            Assert.Contains("Thank you for volunteering!", successAlert.TextContent);

            // Assert (UI): Check that the form fields were reset
            var nameInput = cut.Find("input[id='volunteerName']");
            Assert.Null(nameInput.GetAttribute("value"));

            // Assert (Database): 
            // Create a NEW, separate context for assertions
            await using var dbContext = await dbFactory.CreateDbContextAsync();
            var savedEntry = dbContext.Volunteers.FirstOrDefault();
            Assert.NotNull(savedEntry);
            Assert.Equal("Sandra Lukeman", savedEntry.VolunteerName);
            Assert.Equal("Fundraising", savedEntry.HelpType);
        }

        // Test 2: Verifies that validation messages appear for an empty form
        [Fact]
        public async Task HandleSubmit_WhenFormIsInvalid_ShowsValidationMessages()
        {
            // Arrange
            // Get the factory from the service provider
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var cut = RenderComponent<VolunteerPage>();

            // Act
            // Submit the form without filling in any data
            // Find the form using a stable child element
            var form = cut.Find("input[id='volunteerName']").Closest("form");
            await form.SubmitAsync();

            // Assert (UI): Check that validation messages are present
            var validationMessages = cut.FindAll(".validation-message");
            Assert.True(validationMessages.Count > 0);

            // Assert (UI): Check for a specific required field message
            var nameValidation = cut.Find("div.mb-3:first-child .validation-message");
            Assert.Contains("The Full Name field is required.", nameValidation.TextContent);

            // Assert (UI): Ensure no success message is shown
            Assert.Empty(cut.FindAll("div.alert-success"));

            // Assert (Database): 
            // Create a NEW, separate context for assertions
            await using var dbContext = await dbFactory.CreateDbContextAsync();
            var entryCount = dbContext.Volunteers.Count();
            Assert.Equal(0, entryCount);
        }

    }
}