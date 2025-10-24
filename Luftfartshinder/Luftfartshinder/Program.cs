using Microsoft.EntityFrameworkCore;
using Luftfartshinder.DataContext;
<<<<<<< HEAD
using Luftfartshinder.Repository;
using System;
=======
>>>>>>> 445730baa88f7cdccb9ead394803ef9045d87d32

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

<<<<<<< HEAD
builder.Services.AddScoped<IDataRepocs, ObstacleDataRepo>();

builder.Services.AddDbContext<ApplicationContext>(options =>
               options.UseMySql(builder.Configuration.GetConnectionString("DbConnection"),
               new MySqlServerVersion(new Version(11, 8, 3))));

//(legg til pakkene, og arv)
//builder.Services.AddDbContext<AuthDbContext>(options =>
//               options.UseMySql(builder.Configuration.GetConnectionString("AuthConnection"),
//               new MySqlServerVersion(new Version(11, 8, 3))));

var app = builder.Build();

=======

builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(11, 5, 2))));


var app = builder.Build();



>>>>>>> 445730baa88f7cdccb9ead394803ef9045d87d32
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
