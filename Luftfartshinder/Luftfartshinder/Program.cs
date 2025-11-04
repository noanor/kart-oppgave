using Microsoft.EntityFrameworkCore;
using Luftfartshinder.DataContext;
using Luftfartshinder.Repository;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDataRepocs, ObstacleDataRepo>();

builder.Services.AddDbContext<ApplicationContext>(options =>
               options.UseMySql(builder.Configuration.GetConnectionString("DbConnection"),
               new MariaDbServerVersion(new Version(11, 8, 3))));
builder.Services.AddSession();

//(legg til pakkene, og arv)
//builder.Services.AddDbContext<AuthDbContext>(options =>
//               options.UseMySql(builder.Configuration.GetConnectionString("AuthConnection"),
//               new MySqlServerVersion(new Version(11, 8, 3))));

var app = builder.Build();
Console.WriteLine("[EF DB] " + builder.Configuration.GetConnectionString("DbConnection"));

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
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
