using Microsoft.EntityFrameworkCore;
using Luftfartshinder.DataContext;
using Luftfartshinder.Models;
using Luftfartshinder.Repository;
using Microsoft.AspNetCore.Identity;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDataRepocs, ObstacleDataRepo>();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DbConnection"),
        new MySqlServerVersion(new Version(11, 8, 3))));

//(legg til pakkene, og arv)
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("AuthConnection"),
        new MySqlServerVersion(new Version(11, 8, 3))));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


//
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedRolesAndSuperadmin(roleManager, userManager);
}

string[] roles = { "Superadmin", "Registerforer", "Flybesetning" };
    
    static async Task SeedRolesAndSuperadmin(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        string[] roles = { "Superadmin", "Registerforer", "Flybesetning" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    
    //  Oppretter en superadmin-bruker for testing:
    string superadminEmail = "superadmin@kartverket.no";
    string superadminBrukernavn = "superadmin";
    string superadminPassord = "Test123!"; 

    if (await userManager.FindByEmailAsync(superadminEmail) == null)
    {
        //  Lagrer opplysningene i databasen 
        var superadminBruker = new ApplicationUser
        {
            UserName = superadminBrukernavn,
            Email = superadminEmail,
            Fornavn = "Test",
            Etternavn = "Tester"
        };
        
        // Lagerer passordet som hash i bakgrunnen 
        var result = await userManager.CreateAsync(superadminBruker, superadminPassord);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(superadminBruker, "Superadmin");
        }
    }
}


app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();


