using System;
using Microsoft.EntityFrameworkCore;
using p48_lessons_asp_main.Models; 

namespace p48_lessons_asp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Tasks> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tasks>()
                .Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}