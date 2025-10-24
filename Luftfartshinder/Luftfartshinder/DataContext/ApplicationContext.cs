using Microsoft.EntityFrameworkCore;
using Luftfartshinder.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Luftfartshinder.DataContext
{
    public class ApplicationContext : DbContext
    {
<<<<<<< HEAD
        public DbSet<ObstacleData> Obstacles { get; set; } //Table in the database
=======
        DbSet<ObstacleData> Obstacles { get; set; } //Table in the database
>>>>>>> 445730baa88f7cdccb9ead394803ef9045d87d32
        public ApplicationContext(DbContextOptions<ApplicationContext> dbContextopt) : base(dbContextopt)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Primary keys, remember to think of .Entity as THIS => table in the database

            modelBuilder.Entity<ObstacleData>()
                .HasKey(pk => pk.ObstacleID); //Primary key for ObstacleData
        }
    }
}
