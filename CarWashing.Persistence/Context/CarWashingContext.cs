using System;
using System.Collections.Generic;
using CarWashing.Domain.Models;
// using CarWashing.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CarWashing.Persistence.Context;

public partial class CarWashingContext : DbContext
{
    private readonly IConfiguration _configuration;

    public CarWashingContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public CarWashingContext(DbContextOptions<CarWashingContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Default"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("RoleUsers"));
        
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Services)
            .WithMany(s => s.Orders)
            .UsingEntity(j => j.ToTable("OrderServices"));
    }
}
