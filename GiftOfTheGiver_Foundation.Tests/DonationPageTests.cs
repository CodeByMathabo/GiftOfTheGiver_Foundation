using Bunit;
using GiftOfTheGiver_Foundation.Components.Pages;
using GiftOfTheGiver_Foundation.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using AngleSharp.Dom;

namespace GiftOfTheGiver_Foundation.Tests
{
    public class DonationPageTests : TestContext
    {
        public DonationPageTests()
        {
            try
            {
                var dbName = System.Guid.NewGuid().ToString();
                Services.AddDbContextFactory<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName: dbName));

                Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private class MockNavigationManager : NavigationManager
        {
            public MockNavigationManager()
            {
                Initialize("http://localhost/", "http://localhost/");
            }
            protected override void NavigateToCore(string uri, bool forceLoad) { }
            protected override void NavigateToCore(string uri, NavigationOptions options) { }
        }

        [Fact]
        public void DonationPage_RendersCorrectly_InitialState()
        {
            var cut = RenderComponent<Donation>();

            var h3Elements = cut.FindAll("h3.card-header-title");
            Assert.Equal(2, h3Elements.Count);

            Assert.Contains(h3Elements, h => h.TextContent.Trim() == "Monetary Donations");
            Assert.Contains(h3Elements, h => h.TextContent.Trim() == "Resource Donations");
        }

        [Fact]
        public async Task DonationPage_HandleMonetarySubmit_AddsDonationAndShowsMessage()
        {
            // Arrange
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var cut = RenderComponent<Donation>();
            var testData = new
            {
                FullName = "Peter Smith",
                Amount = 500.75m // Use 'm' for decimal
            };

            // Act
            // Find elements from the root 'cut' and use synchronous .Change()
            cut.Find("input[id='monetaryDonorName']").Change(testData.FullName);

            // Convert decimal to string for the input
            cut.Find("input[id='amount']").Change(testData.Amount.ToString(CultureInfo.InvariantCulture));

            // Find the form via a unique element inside it and submit
            var monetaryForm = cut.Find("input[id='monetaryDonorName']").Closest("form");
            await monetaryForm.SubmitAsync();

            // Assert
            // 1. Check for the success message
            // Re-find the form after the re-render caused by submission
            var formAfterSubmit = cut.Find("input[id='monetaryDonorName']").Closest("form");
            var successMessageElement = formAfterSubmit.QuerySelector(".alert-success");

            Assert.NotNull(successMessageElement);
            Assert.Equal("Thank you for your generous donation!", successMessageElement.TextContent.Trim());

            // 2. Check the database
            using (var context = dbFactory.CreateDbContext())
            {
                // Verify monetary donation was added
                Assert.Equal(1, context.MonetaryDonation.Count());
                var donationInDb = context.MonetaryDonation.First();

                Assert.Equal(testData.FullName, donationInDb.DonorFullName);
                Assert.Equal(testData.Amount, donationInDb.Amount);
                Assert.NotEqual(default(System.DateTime), donationInDb.DonationDate);

                // Verify resource donation was NOT added
                Assert.Equal(0, context.ResourceDonation.Count());
            }
        }

        [Fact]
        public async Task DonationPage_HandleResourceSubmit_AddsDonationAndShowsMessage()
        {
            var dbFactory = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

            var cut = RenderComponent<Donation>();

            var testData = new
            {
                FullName = "Jane Doe",
                Contact = "0812345678",
                ItemType = "Clothing",
                Quantity = 50,
                Delivery = "DropOff"
            };

            // Find each element from the root 'cut' immediately before calling .Change() to avoid stale element references from re-renders.
            cut.Find("input[id='resourceDonorName']").Change(testData.FullName);
            cut.Find("input[id='donorContact']").Change(testData.Contact);
            cut.Find("select[id='donationType']").Change(testData.ItemType);
            cut.Find("input[id='quantity']").Change(testData.Quantity.ToString());
            cut.Find("select[id='deliveryOption']").Change(testData.Delivery);

            // Find the form just to submit it
            var resourceForm = cut.Find("input[id='resourceDonorName']").Closest("form");
            await resourceForm.SubmitAsync();

            // After submission, the component re-renders.
            // find the form again to look for the success message inside it.
            var formAfterSubmit = cut.Find("input[id='resourceDonorName']").Closest("form");
            var successMessageElement = formAfterSubmit.QuerySelector(".alert-success");

            Assert.NotNull(successMessageElement);
            Assert.Equal("Thank you! Our team will be in touch soon to coordinate.", successMessageElement.TextContent.Trim());

            using (var context = dbFactory.CreateDbContext())
            {
                Assert.Equal(1, context.ResourceDonation.Count());
                var donationInDb = context.ResourceDonation.First();

                Assert.Equal(testData.FullName, donationInDb.DonorFullName);
                Assert.Equal(testData.Contact, donationInDb.ContactNumber);
                Assert.Equal(testData.ItemType, donationInDb.ItemType);
                Assert.Equal(testData.Quantity, donationInDb.Quantity);
                Assert.Equal(testData.Delivery, donationInDb.DeliveryOption);
                Assert.NotEqual(default(System.DateTime), donationInDb.PledgeDate);

                Assert.Equal(0, context.MonetaryDonation.Count());
            }
        }
    }
}