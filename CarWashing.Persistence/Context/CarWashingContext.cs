using System;
using System.Collections.Generic;
using CarWashing.Domain.Models;
using CarWashing.Persistence.Entities;
// using CarWashing.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CarWashing.Persistence.Context;

public class CarWashingContext : DbContext
{
    private readonly IConfiguration _configuration = null!;
    public CarWashingContext()
    {
    }
    public CarWashingContext(DbContextOptions<CarWashingContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Default"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("RoleUsers"));
        
        modelBuilder.Entity<OrderEntity>()
            .HasMany(o => o.Services)
            .WithMany(s => s.Orders)
            .UsingEntity(j => j.ToTable("OrderServices"));
        
        modelBuilder.Entity<ServiceEntity>().OwnsOne(s => s.Price);
        modelBuilder.Entity<ServiceEntity>().OwnsOne(s => s.Time);
    }
    
    public DbSet<BrandEntity> Brands { get; set; }
    public DbSet<CarEntity> Cars { get; set; }
    public DbSet<CustomerCarEntity> CustomerCars { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<ServiceEntity> Services { get; set; }
    public DbSet<UserEntity> Users { get; set; }
}
