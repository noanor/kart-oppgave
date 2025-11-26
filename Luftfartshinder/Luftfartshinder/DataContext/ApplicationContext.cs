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
            
            // LinePointsJson is optional and nullable
            modelBuilder.Entity<Obstacle>()
                .Property(o => o.LinePointsJson)
                .HasColumnType("longtext")
                .IsRequired(false);

            modelBuilder.Entity<Report>()
                .HasKey(pk => pk.Id); //Primary key for Data

            modelBuilder.Entity<Report>() 
                .HasMany(r => r.Obstacles)
                .WithOne(o => o.Report)
                .HasForeignKey(o => o.ReportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization is in AuthDbContext, not here
            // Ignore the navigation property to avoid FK conflicts
            modelBuilder.Entity<Report>()
                .Ignore(r => r.Organization);
        }
    }
}
