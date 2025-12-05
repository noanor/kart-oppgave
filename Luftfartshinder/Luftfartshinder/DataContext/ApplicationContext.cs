using Microsoft.EntityFrameworkCore;
using Luftfartshinder.Models.Domain;

namespace Luftfartshinder.DataContext
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        { 
        }
        public DbSet<Obstacle> Obstacles { get; set; } //Table in the database
        public DbSet<Report> Reports { get; set; } //Table in the databa


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Primary keys, remember to think of .Entity as THIS => table in the database

            modelBuilder.Entity<Obstacle>()
                .HasKey(pk => pk.Id); //Primary key for Data

            modelBuilder.Entity<Obstacle>()
                .Property(o => o.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Report>()
                .HasKey(pk => pk.Id); //Primary key for Data

            modelBuilder.Entity<Report>()
                .Property(r => r.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Report>() 
                .HasMany(r => r.Obstacles)
                .WithOne(o => o.Report)
                .HasForeignKey(o => o.ReportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization is in AuthDbContext, not here
            // Ignore the navigation property to avoid FK conflicts
            modelBuilder.Entity<Report>()
                .Ignore(r => r.Organization);

            // Seed Reports
            var pilotId = "1d3b44cf-5507-444f-b84c-842539f13e02"; // From AuthDbContext
            var registrarId = "322acd53-a201-47c6-a7e0-6695690ce677"; // From AuthDbContext
            var newPilotId = "2e4c55df-6618-5550-c95d-953640f24e13"; // New pilot from AuthDbContext

            var reports = new List<Report>
            {
                new Report
                {
                    Id = 1,
                    Title = "Powerline Obstacle Report - Oslo Area",
                    Summary = "Multiple powerline obstacles detected during flight survey",
                    Author = "pilot",
                    AuthorId = pilotId,
                    OrganizationId = 4, // Norwegian Air Ambulance
                    ReportDate = DateTime.UtcNow.AddDays(-5)
                },
                new Report
                {
                    Id = 2,
                    Title = "Mast and Tower Report - Bergen Region",
                    Summary = "Communication masts and towers identified in flight path",
                    Author = "pilot",
                    AuthorId = pilotId,
                    OrganizationId = 4, // Norwegian Air Ambulance
                    ReportDate = DateTime.UtcNow.AddDays(-3)
                },
                new Report
                {
                    Id = 3,
                    Title = "Urban Obstacle Survey - Trondheim",
                    Summary = "Various urban obstacles including buildings and structures",
                    Author = "registrar",
                    AuthorId = registrarId,
                    OrganizationId = 1, // Kartverket
                    ReportDate = DateTime.UtcNow.AddDays(-1)
                },
                // New reports for new pilot
                new Report
                {
                    Id = 4,
                    Title = "Northern Route Obstacle Survey - Tromsø",
                    Summary = "Obstacles identified during emergency flight route to Tromsø",
                    Author = "pilot2",
                    AuthorId = newPilotId,
                    OrganizationId = 4, // Norwegian Air Ambulance
                    ReportDate = DateTime.UtcNow.AddDays(-2)
                },
                new Report
                {
                    Id = 5,
                    Title = "Stavanger Area Powerline Report",
                    Summary = "Powerline obstacles in Stavanger region affecting flight paths",
                    Author = "pilot2",
                    AuthorId = newPilotId,
                    OrganizationId = 4, // Norwegian Air Ambulance
                    ReportDate = DateTime.UtcNow.AddDays(-4)
                }
            };

            modelBuilder.Entity<Report>().HasData(reports);

            // Seed Obstacles - All coordinates verified to be within Norway
            // Norway boundaries: Latitude 58.0° to 71.08° N, Longitude 4.56° to 31.01° E
            var obstacles = new List<Obstacle>
            {
                // Obstacles for Report 1 (Powerline Report - Oslo Area)
                new Obstacle
                {
                    Id = 1,
                    ReportId = 1,
                    OrganizationId = 4,
                    Type = "powerline",
                    Name = "High Voltage Powerline",
                    Height = 45.5,
                    Latitude = 59.9139,  // Oslo, Norway
                    Longitude = 10.7522,
                    Description = "High voltage powerline crossing flight path near Oslo",
                    Status = Obstacle.Statuses.Pending
                },
                new Obstacle
                {
                    Id = 2,
                    ReportId = 1,
                    OrganizationId = 4,
                    Type = "powerline",
                    Name = "Distribution Line",
                    Height = 25.0,
                    Latitude = 59.9200,  // Oslo area, Norway
                    Longitude = 10.7600,
                    Description = "Distribution powerline near residential area in Oslo",
                    Status = Obstacle.Statuses.Pending
                },
                new Obstacle
                {
                    Id = 3,
                    ReportId = 1,
                    OrganizationId = 4,
                    Type = "line",
                    Name = "Cable Line",
                    Height = 30.0,
                    Latitude = 59.9150,  // Oslo area, Norway
                    Longitude = 10.7550,
                    Description = "Overhead cable line in Oslo region",
                    Status = Obstacle.Statuses.Approved
                },
                // Obstacles for Report 2 (Mast and Tower Report - Bergen Region)
                new Obstacle
                {
                    Id = 4,
                    ReportId = 2,
                    OrganizationId = 4,
                    Type = "mast",
                    Name = "Communication Mast",
                    Height = 120.0,
                    Latitude = 60.3913,  // Bergen, Norway
                    Longitude = 5.3221,
                    Description = "Tall communication mast in Bergen area",
                    Status = Obstacle.Statuses.Pending
                },
                new Obstacle
                {
                    Id = 5,
                    ReportId = 2,
                    OrganizationId = 4,
                    Type = "mast",
                    Name = "Radio Tower",
                    Height = 95.5,
                    Latitude = 60.4000,  // Bergen area, Norway
                    Longitude = 5.3300,
                    Description = "Radio transmission tower near Bergen",
                    Status = Obstacle.Statuses.Pending
                },
                new Obstacle
                {
                    Id = 6,
                    ReportId = 2,
                    OrganizationId = 4,
                    Type = "point",
                    Name = "Weather Station Mast",
                    Height = 15.0,
                    Latitude = 60.3900,  // Bergen area, Norway
                    Longitude = 5.3200,
                    Description = "Weather monitoring station in Bergen region",
                    Status = Obstacle.Statuses.Rejected
                },
                // Obstacles for Report 3 (Urban Survey - Trondheim)
                new Obstacle
                {
                    Id = 7,
                    ReportId = 3,
                    OrganizationId = 1,
                    Type = "point",
                    Name = "Building Antenna",
                    Height = 12.5,
                    Latitude = 63.4305,  // Trondheim, Norway
                    Longitude = 10.3951,
                    Description = "Antenna on top of building in Trondheim",
                    Status = Obstacle.Statuses.Pending
                },
                new Obstacle
                {
                    Id = 8,
                    ReportId = 3,
                    OrganizationId = 1,
                    Type = "area",
                    Name = "Industrial Complex",
                    Height = 35.0,
                    Latitude = 63.4350,  // Trondheim area, Norway
                    Longitude = 10.4000,
                    Description = "Large industrial building complex in Trondheim",
                    Status = Obstacle.Statuses.Pending
                },
                // Obstacles for Report 4 (New Pilot - Tromsø)
                new Obstacle
                {
                    Id = 9,
                    ReportId = 4,
                    OrganizationId = 4,
                    Type = "mast",
                    Name = "Communication Tower",
                    Height = 85.0,
                    Latitude = 69.6492,  // Tromsø, Norway
                    Longitude = 18.9553,
                    Description = "Communication tower near Tromsø airport",
                    Status = Obstacle.Statuses.Pending
                },
                new Obstacle
                {
                    Id = 10,
                    ReportId = 4,
                    OrganizationId = 4,
                    Type = "point",
                    Name = "Weather Mast",
                    Height = 20.0,
                    Latitude = 69.6500,  // Tromsø area, Norway
                    Longitude = 18.9600,
                    Description = "Weather monitoring mast in Tromsø region",
                    Status = Obstacle.Statuses.Pending
                },
                new Obstacle
                {
                    Id = 11,
                    ReportId = 4,
                    OrganizationId = 4,
                    Type = "powerline",
                    Name = "High Voltage Line",
                    Height = 50.0,
                    Latitude = 69.6450,  // Tromsø area, Norway
                    Longitude = 18.9500,
                    Description = "High voltage powerline crossing flight path",
                    Status = Obstacle.Statuses.Approved
                },
                // Obstacles for Report 5 (New Pilot - Stavanger)
                new Obstacle
                {
                    Id = 12,
                    ReportId = 5,
                    OrganizationId = 4,
                    Type = "powerline",
                    Name = "Main Powerline",
                    Height = 40.0,
                    Latitude = 58.9700,  // Stavanger, Norway
                    Longitude = 5.7331,
                    Description = "Main powerline in Stavanger area",
                    Status = Obstacle.Statuses.Pending
                },
                new Obstacle
                {
                    Id = 13,
                    ReportId = 5,
                    OrganizationId = 4,
                    Type = "line",
                    Name = "Cable Line",
                    Height = 28.5,
                    Latitude = 58.9750,  // Stavanger area, Norway
                    Longitude = 5.7400,
                    Description = "Overhead cable line near Stavanger",
                    Status = Obstacle.Statuses.Pending
                }
            };

            modelBuilder.Entity<Obstacle>().HasData(obstacles);
        }
    }
}
