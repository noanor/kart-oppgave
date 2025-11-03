using Microsoft.EntityFrameworkCore;
using Luftfartshinder.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Luftfartshinder.Models.ViewModel;

namespace Luftfartshinder.DataContext
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Obstacle> Obstacles { get; set; } //Table in the database
        public ApplicationContext(DbContextOptions<ApplicationContext> dbContextopt) : base(dbContextopt)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Primary keys, remember to think of .Entity as THIS => table in the database

            modelBuilder.Entity<Obstacle>()
                .HasKey(pk => pk.Id); //Primary key for Data
        }
    }
}
