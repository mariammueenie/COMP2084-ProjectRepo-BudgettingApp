
using Microsoft.EntityFrameworkCore;
using BudgetingApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add MVC Controller with Views support.
builder.Services.AddControllersWithViews();

// Register ApplicationDbContext with EF Core and SQL Server.
// (uses connection string from appsettings.json)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure HTTP request pipeline, plus error handling.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Redirect all HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Serve static files (like CSS, JS, images, favicons, etc.) from wwwroot folder.
app.UseStaticFiles();

// Allow for routing of requests.
app.UseRouting();

// Enable authentication and authorization middleware.
app.UseAuthorization();

// Set up default route for MVC controllers.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");




// Self explanatory.
app.Run();
