using Luftfartshinder.DataContext;
using Luftfartshinder.Models.Domain;
using Luftfartshinder.Repository;
using Luftfartshinder.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

});

builder.Services.AddDbContext<ApplicationContext>(options =>
               options.UseMySql(builder.Configuration.GetConnectionString("DbConnection"),
               new MariaDbServerVersion(new Version(11, 8, 3))));
builder.Services.AddSession();

builder.Services.AddDbContext<AuthDbContext>(options =>
           options.UseMySql(builder.Configuration.GetConnectionString("AuthConnection"),
             new MySqlServerVersion(new Version(11, 8, 3))));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Repository pattern
builder.Services.AddScoped<IObstacleRepository, ObstacleRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();

// Services
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();
Console.WriteLine("[EF DB] " + builder.Configuration.GetConnectionString("DbConnection"));

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var authContext = services.GetRequiredService<AuthDbContext>();
        var appplicationContext = services.GetRequiredService<ApplicationContext>();

        // Try to migrate AuthDbContext
        try
        {
            authContext.Database.Migrate();
        }
        catch (Exception ex) when (ex.Message.Contains("already exists") || ex.InnerException?.Message?.Contains("already exists") == true)
        {
            // Tables exist but migrations not recorded - mark them as applied
            Console.WriteLine("Tables already exist in AuthDbContext. Marking migrations as applied...");
            var migrations = authContext.Database.GetMigrations().ToList();
            foreach (var migration in migrations)
            {
                try
                {
                    // Try to mark migration as applied
                    var sql = $"INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ('{migration}', '9.0.9');";
                    authContext.Database.ExecuteSqlRaw(sql);
                }
                catch { /* Ignore if already exists */ }
            }
            Console.WriteLine("Migrations marked as applied for AuthDbContext.");
        }

        // Try to migrate ApplicationContext
        try
        {
            appplicationContext.Database.Migrate();
        }
        catch (Exception ex) when (ex.Message.Contains("already exists") || ex.InnerException?.Message?.Contains("already exists") == true)
        {
            // Tables exist but migrations not recorded - mark them as applied
            Console.WriteLine("Tables already exist in ApplicationContext. Marking migrations as applied...");
            var migrations = appplicationContext.Database.GetMigrations().ToList();
            foreach (var migration in migrations)
            {
                try
                {
                    // Try to mark migration as applied
                    var sql = $"INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ('{migration}', '9.0.9');";
                    appplicationContext.Database.ExecuteSqlRaw(sql);
                }
                catch { /* Ignore if already exists */ }
            }
            Console.WriteLine("Migrations marked as applied for ApplicationContext.");
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occured in the database: {ex}");
        throw;
    }

}

// Always use exception handler to prevent exposing error details
app.UseExceptionHandler("/Home/Error");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

// Add Security Headers Middleware
app.Use(async (context, next) =>
{
    // Strict-Transport-Security (HSTS)
    // What: Forces browsers to use HTTPS for all future connections to this domain
    // Why: Prevents man-in-the-middle attacks and ensures all traffic is encrypted
    //      The browser remembers this for the specified time period (max-age)
    //      includeSubDomains applies HSTS to all subdomains
    //      preload allows the domain to be included in browser HSTS preload lists
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append("Strict-Transport-Security",
            "max-age=31536000; includeSubDomains; preload");
    }

    // X-Content-Type-Options: nosniff
    // What: Prevents browsers from guessing file types (MIME type sniffing)
    // Why: Stops attackers from serving malicious scripts as text files, which browsers might execute
    //      Forces browser to respect the Content-Type header sent by the server
    //      Without this, a file served as "text/plain" could be executed as JavaScript
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // X-XSS-Protection header (legacy, but required for assignment)
    // What: Tells old browsers to enable their built-in XSS filter
    // Why: Provides basic protection against cross-site scripting attacks in older browsers
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    // X-Frame-Options: DENY
    // What: Prevents this website from being loaded inside an iframe on other websites
    // Why: Protects against clickjacking attacks where attackers embed your site in a hidden frame
    //      and trick users into clicking on things they can't see
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // Content-Security-Policy
    // What: Controls which resources (scripts, styles, images, etc.) the browser is allowed to load
    // Why: Prevents XSS attacks by blocking unauthorized scripts and resources from loading
    //      This is the modern replacement for X-XSS-Protection
    var csp = "default-src 'self'; " +
              "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://kit.fontawesome.com https://unpkg.com https://cdn.jsdelivr.net; " +
              "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net https://unpkg.com; " +
              "font-src 'self' https://fonts.gstatic.com https://cdn.jsdelivr.net https://use.fontawesome.com https://ka-f.fontawesome.com; " +
              "img-src 'self' data: https: blob:; " +
              "connect-src 'self' https:; " +
              "frame-ancestors 'none';";
    context.Response.Headers.Append("Content-Security-Policy", csp);

    // Referrer-Policy: strict-origin-when-cross-origin
    // What: Controls how much referrer information is sent to other websites
    // Why: Protects user privacy by limiting what information is leaked when users navigate away
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    await next();
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();

// Make Program class accessible for integration testing
public partial class Program { }
