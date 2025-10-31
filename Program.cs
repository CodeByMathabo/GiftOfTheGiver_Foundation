using GiftOfTheGiver_Foundation.Components;
using GiftOfTheGiver_Foundation.Components.Account;
using GiftOfTheGiver_Foundation.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GiftOfTheGiver_Foundation.Models;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// --- ADD NEW DOWNLOAD ENDPOINTS HERE ---

// Endpoint for Monetary Donations CSV
app.MapGet("/download/monetary-csv", async (IDbContextFactory<ApplicationDbContext> dbFactory) =>
{
    await using var dbContext = await dbFactory.CreateDbContextAsync();
    var donations = await dbContext.MonetaryDonation.ToListAsync();

    var builder = new StringBuilder();

    // Add Header
    builder.AppendLine("DonationID,DonorFullName,Amount(ZAR),PledgeDate,AuditDate");

    // Add Rows
    foreach (var donation in donations)
    {
        // Use fixed-point for CSV (e.g., 1200.00), not currency symbol
        var amount = donation.Amount.ToString("F2", CultureInfo.InvariantCulture);
        var pledgeDate = donation.DonationDate.ToString("yyyy-MM-dd");
        var auditDate = donation.AuditDate.HasValue ? donation.AuditDate.Value.ToString("yyyy-MM-dd") : "Pending";

        builder.AppendLine($"MON-{donation.Id:D3},{donation.DonorFullName},{amount},{pledgeDate},{auditDate}");
    }

    var fileName = $"monetary-donations-{DateTime.UtcNow:yyyyMMdd}.csv";
    return Results.File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", fileName);
});

// Endpoint for Resource Donations CSV
app.MapGet("/download/resource-csv", async (IDbContextFactory<ApplicationDbContext> dbFactory) =>
{
    await using var dbContext = await dbFactory.CreateDbContextAsync();
    var donations = await dbContext.ResourceDonation.ToListAsync();

    var builder = new StringBuilder();

    // Add Header
    builder.AppendLine("PledgeID,DonorFullName,ContactNumber,Item,Quantity,Delivery,PledgeDate,AuditDate");

    // Add Rows
    foreach (var donation in donations)
    {
        var pledgeDate = donation.PledgeDate.ToString("yyyy-MM-dd");
        var auditDate = donation.AuditDate.HasValue ? donation.AuditDate.Value.ToString("yyyy-MM-dd") : "Pending";

        // Simple CSV escaping: wrap fields in quotes to handle potential commas
        var donorName = $"\"{donation.DonorFullName.Replace("\"", "\"\"")}\"";
        var itemType = $"\"{donation.ItemType.Replace("\"", "\"\"")}\"";

        builder.AppendLine($"RES-{donation.Id:D3},{donorName},{donation.ContactNumber},{itemType},{donation.Quantity},{donation.DeliveryOption},{pledgeDate},{auditDate}");
    }

    var fileName = $"resource-donations-{DateTime.UtcNow:yyyyMMdd}.csv";
    return Results.File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", fileName);
});

app.Run();
