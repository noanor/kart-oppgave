using Microsoft.EntityFrameworkCore;
using Luftfartshinder.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Luftfartshinder.DataContext
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        { 
        }
        public DbSet<ObstacleData> Obstacles { get; set; } //Table in the database
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Primary keys, remember to think of .Entity as THIS => table in the database

            modelBuilder.Entity<ObstacleData>()
                .HasKey(pk => pk.ObstacleID); //Primary key for ObstacleData
        }
    }
}
