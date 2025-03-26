using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vroom.Service.Database.Entities;

namespace Vroom.Service.Database.Configuration;

public class VehicleMakeConfiguration : IEntityTypeConfiguration<VehicleMake>
{
    public void Configure(EntityTypeBuilder<VehicleMake> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id).ValueGeneratedOnAdd();
        builder.Property(v => v.Name).IsRequired();
        builder.Property(v => v.Abbreviation).IsRequired();

        Seed(builder);
    }

    private void Seed(EntityTypeBuilder<VehicleMake> builder)
    {
        builder.HasData(
            new
            {
                Id = 1,
                Name = "BMW",
                Abbreviation = "BMW",
            },
            new
            {
                Id = 2,
                Name = "Ford",
                Abbreviation = "FORD",
            },
            new
            {
                Id = 3,
                Name = "Volkswagen",
                Abbreviation = "VW",
            }
        );
    }
}
