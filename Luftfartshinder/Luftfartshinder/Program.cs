using Microsoft.EntityFrameworkCore;
using Luftfartshinder.DataContext;
using Luftfartshinder.Models;
using Luftfartshinder.Repository;
using System;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDataRepocs, ObstacleDataRepo>();

builder.Services.AddDbContext<ApplicationContext>(options =>
               options.UseMySql(builder.Configuration.GetConnectionString("DbConnection"),
               new MySqlServerVersion(new Version(11, 8, 3))));

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();