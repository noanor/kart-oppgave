using Microsoft.EntityFrameworkCore;
using Oppgave1.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Oppgave1.DataContext
{
    public class ApplicationContext : DbContext
    {
        DbSet<ObstacleData> Obstacles { get; set; } //Table in the database
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
