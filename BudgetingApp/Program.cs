
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; 
using BudgetingApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add MVC Controller with Views support.
builder.Services.AddControllersWithViews();

// Register ApplicationDbContext with EF Core and SQL Server.
// (uses connection string from app settings.json)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration for user authentication.
builder.Services 
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        // Password requirements for user accounts, so users must have secure passwords.
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole>() // Add role management.
    .AddEntityFrameworkStores<ApplicationDbContext>() // Use EF Core for user data storage.
    // .AddDefaultTokenProviders() // Add token providers for password reset, email confirmation, etc.
    .AddDefaultUI();
// Add default UI for Identity (login, register, etc.)
// This provides basic UI for users.
// Reference: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identitybuilderuiextensions.adddefaultui?view=aspnetcore-9.0
// Reference: https://medium.com/@ashrafulislam_1167/getting-started-with-asp-net-core-identity-in-an-mvc-project-a-beginners-guide-4d804ed79183

// Google Authentication configuration. 
builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

    });


// Configure HTTP request pipeline, plus error handling.
   // if (!app.Environment.IsDevelopment())
//    {
  //      app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //    app.UseHsts();
    // }

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // An error page shown if unexpected errors occur.
    app.UseHsts();
    // Use exception handling middleware to catch errors and show error page.
}
// Redirect all HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Serve static files (like CSS, JS, images, favicons, etc.) from wwwroot folder.
app.UseStaticFiles();

// Allow for routing of requests.
app.UseRouting();

// Enable authentication, for sign-in and sign-out functionality.
// Must be before UseAuthorization -_-.
app.UseAuthentication();

// Enable authorization, to restrict access to certain parts of app for non-authenticated users.
app.UseAuthorization();

// Set up default route for MVC controllers.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages if/when needed.
app.MapRazorPages();

// Self explanatory.
app.Run();
