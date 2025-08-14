
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

// disable google keys temporarily, so it only runs if keys are actually present!
// BEFORE app.Build();
var googleId     = builder.Configuration["Authentication:Google:ClientId"];
var googleSecret = builder.Configuration["Authentication:Google:ClientSecret"];

if (!string.IsNullOrWhiteSpace(googleId) && !string.IsNullOrWhiteSpace(googleSecret))
{
    builder.Services
        .AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleId!;
            options.ClientSecret = googleSecret!;
        });
}
// If keys are missing, Google is simply not added and won’t break startup.
// If keys are present, Google authentication is added to the app.

// Configure HTTP request pipeline, plus error handling.
// if (!app.Environment.IsDevelopment())
//    {
//      app.UseExceptionHandler("/Home/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
// }

var app = builder.Build();

// TEMP TO MAKE SURE DB IS CREATED
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BudgetingApp.Data.ApplicationDbContext>();
    db.Database.Migrate();
}


// TEMPORARY
// ===== SEED ROLES + ADMIN USER ON STARTUP =====
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Ensure roles exist
        string[] roles = new[] { "Admin", "Customer" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var r = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!r.Succeeded)
                {
                    throw new Exception("Failed creating role '" + roleName + "': " +
                        string.Join("; ", r.Errors.Select(e => e.Description)));
                }
            }
        }

        // Ensure admin user exists
        var adminEmail = "dario@gc.ca";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var create = await userManager.CreateAsync(adminUser, "Test123$");
            if (!create.Succeeded)
            {
                throw new Exception("Failed creating admin user: " +
                    string.Join("; ", create.Errors.Select(e => e.Description)));
            }
        }

        // Ensure admin is in Admin role
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var addRole = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (!addRole.Succeeded)
            {
                throw new Exception("Failed adding admin to role: " +
                    string.Join("; ", addRole.Errors.Select(e => e.Description)));
            }
        }

        Console.WriteLine("✅ Seed complete: roles + dario@gc.ca (Admin).");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Seed error: " + ex.Message);
    }
}
// ===== END SEED =====







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
