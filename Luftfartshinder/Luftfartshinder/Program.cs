using Luftfartshinder.DataContext;
using Luftfartshinder.Models;
using Luftfartshinder.Repository;
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

//////(legg til pakkene, og arv)
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

var app = builder.Build();
Console.WriteLine("[EF DB] " + builder.Configuration.GetConnectionString("DbConnection"));

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var authContext = services.GetRequiredService<AuthDbContext>();
        var appplicationContext = services.GetRequiredService<ApplicationContext>();

        authContext.Database.Migrate();
        appplicationContext.Database.Migrate();
        
    } catch(Exception ex)
    {
        Console.WriteLine($"An error occured in the database: {ex}");
        Environment.Exit(1);
    }

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

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
