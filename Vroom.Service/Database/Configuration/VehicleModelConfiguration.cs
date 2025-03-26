using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vroom.Service.Database.Entities;

namespace Vroom.Service.Database.Configuration;

public class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.HasKey(vm => vm.Id);

        builder.Property(v => v.Id).ValueGeneratedOnAdd();
        builder.Property(vm => vm.Name).IsRequired();
        builder.Property(vm => vm.Abbreviation).IsRequired();

        builder
            .HasOne(vm => vm.Make)
            .WithMany(v => v.Models)
            .HasForeignKey(vm => vm.MakeId)
            .OnDelete(DeleteBehavior.Cascade);

        Seed(builder);
    }

    private void Seed(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.HasData(
            new
            {
                Id = 1,
                Name = "128",
                Abbreviation = "128",
                MakeId = 1,
            },
            new
            {
                Id = 2,
                Name = "325",
                Abbreviation = "325",
                MakeId = 1,
            },
            new
            {
                Id = 3,
                Name = "X5",
                Abbreviation = "X5",
                MakeId = 1,
            },
            new
            {
                Id = 4,
                Name = "Fiesta",
                Abbreviation = "Fiesta",
                MakeId = 2,
            },
            new
            {
                Id = 5,
                Name = "Focus",
                Abbreviation = "Focus",
                MakeId = 2,
            }
        );
    }
}
