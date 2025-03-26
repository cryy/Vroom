using Microsoft.EntityFrameworkCore;
using Vroom.Service.Database.Entities;

namespace Vroom.Service.Database;

public class VroomContext : DbContext
{
    public VroomContext(DbContextOptions<VroomContext> options)
        : base(options) { }

    public DbSet<VehicleMake> Vehicles { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VroomContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
