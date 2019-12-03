﻿using clearlyApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace clearlyApi
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AccountSession> AccountSessions { get; set; }
        public DbSet<ActivationCode> ActivationCodes { get; set; }
        public DbSet<LocalizedString> LocalizedStrings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Package> Packages { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=127.0.0.1;UserId=root;Password=albert1997;database=clearly-new;");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<User>()
                .HasOne(p => p.Person)
                .WithOne(u => u.User)
                .HasForeignKey<Person>(p => p.UserId);

            modelBuilder.Entity<User>()
                    .HasMany(u => u.UserMessages)
                    .WithOne(m => m.User)
                    .HasForeignKey(m => m.UserId);

            modelBuilder.Entity<User>()
                    .HasMany(u => u.AdminMessages)
                    .WithOne(m => m.Admin)
                    .HasForeignKey(m => m.AdminId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();
        }
    }
}
