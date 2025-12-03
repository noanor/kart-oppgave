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

//s(legg til pakkene, og arv)
builder.Services.AddDbContext<AuthDbContext>(options =>
           options.UseMySql(builder.Configuration.GetConnectionString("AuthConnection"),
             new MySqlServerVersion(new Version(11, 8, 3))));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Dette er default settings for passord
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add Security Headers Middleware
app.Use(async (context, next) =>
{
    // X-XSS-Protection header (legacy, but required for assignment)
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    
    // Additional recommended security headers
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
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
