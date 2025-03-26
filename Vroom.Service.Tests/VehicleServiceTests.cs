using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vroom.Dto;
using Vroom.Service.Database;
using Vroom.Service.Database.Entities;
using Vroom.Service.Pagination;
using Vroom.Service.Services;

namespace Vroom.Services.Tests;

public class VehicleServiceTests
{
    private VroomContext CreateTestContext()
    {
        var options = new DbContextOptionsBuilder<VroomContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new VroomContext(options);
    }

    private VehicleService CreateVehicleService(VroomContext context)
    {
        var contextFactoryMock = new Mock<IDbContextFactory<VroomContext>>();
        contextFactoryMock.Setup(x => x.CreateDbContextAsync(default)).ReturnsAsync(context);

        var loggerMock = new Mock<ILogger<VehicleService>>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<VehicleMake, VehicleMakeDto>().ReverseMap();
            cfg.CreateMap<CreateVehicleMakeDto, VehicleMake>();
            cfg.CreateMap<UpdateVehicleMakeDto, VehicleMake>();

            cfg.CreateMap<VehicleModel, VehicleModelDto>()
                .ForMember(dest => dest.MakeName, opt => opt.MapFrom(src => src.Make.Name))
                .ReverseMap();
            cfg.CreateMap<CreateVehicleModelDto, VehicleModel>();
            cfg.CreateMap<UpdateVehicleModelDto, VehicleModel>();
        });
        var mapper = mapperConfig.CreateMapper();

        return new VehicleService(contextFactoryMock.Object, loggerMock.Object, mapper);
    }

    [Fact]
    public async Task GetVehicleMakeByIdAsync_ExistingId_ReturnsVehicleMake()
    {
        await using var context = CreateTestContext();
        var vehicleMake = new VehicleMake
        {
            Id = 1,
            Name = "Toyota",
            Abbreviation = "TOYOTA",
        };
        context.Vehicles.Add(vehicleMake);
        await context.SaveChangesAsync();

        var service = CreateVehicleService(context);

        var result = await service.GetVehicleMakeByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Toyota", result.Name);
        Assert.Equal("TOYOTA", result.Abbreviation);
    }

    [Fact]
    public async Task GetVehicleMakeByIdAsync_InvalidId_ReturnsNull()
    {
        await using var context = CreateTestContext();
        var service = CreateVehicleService(context);

        var result = await service.GetVehicleMakeByIdAsync(-1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetVehicleMakesAsync_ReturnsPagedResults()
    {
        await using var context = CreateTestContext();
        context.Vehicles.AddRange(
            new VehicleMake
            {
                Id = 1,
                Name = "Toyota",
                Abbreviation = "TOYOTA",
            },
            new VehicleMake
            {
                Id = 2,
                Name = "Honda",
                Abbreviation = "HONDA",
            }
        );
        await context.SaveChangesAsync();

        var service = CreateVehicleService(context);
        var filteringParams = new FilteringParams
        {
            SearchQuery = "",
            SortBy = Sort.Name,
            Descending = false,
            PageNumber = 1,
            PageSize = 10,
        };

        var result = await service.GetVehicleMakesAsync(filteringParams);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task CreateVehicleMakeAsync_ValidDto_CreatesVehicleMake()
    {
        await using var context = CreateTestContext();
        var service = CreateVehicleService(context);
        var createDto = new CreateVehicleMakeDto { Name = "Mazda", Abbreviation = "MAZDA" };

        var result = await service.CreateVehicleMakeAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("Mazda", result.Name);
        Assert.Equal("MAZDA", result.Abbreviation);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task UpdateVehicleMakeAsync_ExistingMake_UpdatesSuccessfully()
    {
        await using var context = CreateTestContext();
        var vehicleMake = new VehicleMake
        {
            Id = 1,
            Name = "Toyota",
            Abbreviation = "TOYOTA",
        };
        context.Vehicles.Add(vehicleMake);
        await context.SaveChangesAsync();

        var service = CreateVehicleService(context);
        var updateDto = new UpdateVehicleMakeDto
        {
            Name = "Updated Toyota",
            Abbreviation = "UPDATED_TOYOTA",
        };

        var result = await service.UpdateVehicleMakeAsync(1, updateDto);

        Assert.True(result);
        var updatedMake = await context.Vehicles.FindAsync(1);
        Assert.Equal("Updated Toyota", updatedMake.Name);
        Assert.Equal("UPDATED_TOYOTA", updatedMake.Abbreviation);
    }

    [Fact]
    public async Task DeleteVehicleMakeAsync_ExistingMake_DeletesSuccessfully()
    {
        await using var context = CreateTestContext();
        var vehicleMake = new VehicleMake
        {
            Id = 1,
            Name = "Toyota",
            Abbreviation = "TOYOTA",
        };
        context.Vehicles.Add(vehicleMake);
        await context.SaveChangesAsync();

        var service = CreateVehicleService(context);

        var result = await service.DeleteVehicleMakeAsync(1);

        Assert.True(result);

        // context is disposed here, so a new one is created
        await using var assertContext = CreateTestContext();
        Assert.Null(await assertContext.Vehicles.FindAsync(1));
    }

    [Fact]
    public async Task GetVehicleModelByIdAsync_ExistingId_ReturnsVehicleModel()
    {
        await using var context = CreateTestContext();
        var make = new VehicleMake
        {
            Id = 1,
            Name = "Toyota",
            Abbreviation = "TOYOTA",
        };
        var vehicleModel = new VehicleModel
        {
            Id = 1,
            Name = "Camry",
            Abbreviation = "CAMRY",
            MakeId = 1,
            Make = make,
        };
        context.Vehicles.Add(make);
        context.VehicleModels.Add(vehicleModel);
        await context.SaveChangesAsync();

        var service = CreateVehicleService(context);

        var result = await service.GetVehicleModelByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Camry", result.Name);
        Assert.Equal("CAMRY", result.Abbreviation);
        Assert.Equal("Toyota", result.MakeName);
    }

    [Fact]
    public async Task GetVehicleModelByIdAsync_InvalidId_ReturnsNull()
    {
        await using var context = CreateTestContext();
        var service = CreateVehicleService(context);

        var result = await service.GetVehicleModelByIdAsync(-1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetVehicleModelsAsync_ReturnsPagedResults()
    {
        await using var context = CreateTestContext();
        var make = new VehicleMake
        {
            Id = 1,
            Name = "Toyota",
            Abbreviation = "TOYOTA",
        };
        context.Vehicles.Add(make);
        context.VehicleModels.AddRange(
            new VehicleModel
            {
                Id = 1,
                Name = "Camry",
                Abbreviation = "CAMRY",
                MakeId = 1,
            },
            new VehicleModel
            {
                Id = 2,
                Name = "Corolla",
                Abbreviation = "COROLLA",
                MakeId = 1,
            }
        );
        await context.SaveChangesAsync();

        var service = CreateVehicleService(context);
        var filteringParams = new FilteringParams
        {
            SearchQuery = "",
            SortBy = Sort.Name,
            Descending = false,
            PageNumber = 1,
            PageSize = 10,
        };

        var result = await service.GetVehicleModelsAsync(filteringParams);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task CreateVehicleModelAsync_ValidDto_CreatesVehicleModel()
    {
        await using var context = CreateTestContext();
        var make = new VehicleMake
        {
            Id = 1,
            Name = "Toyota",
            Abbreviation = "TOYOTA",
        };
        context.Vehicles.Add(make);
        await context.SaveChangesAsync();

        var service = CreateVehicleService(context);
        var createDto = new CreateVehicleModelDto
        {
            Name = "Camry",
            Abbreviation = "CAMRY",
            MakeId = 1,
        };

        var result = await service.CreateVehicleModelAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("Camry", result.Name);
        Assert.Equal("CAMRY", result.Abbreviation);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task UpdateVehicleModelAsync_ExistingModel_UpdatesSuccessfully()
    {
        await using var context = CreateTestContext();
        var make = new VehicleMake
        {
            Id = 1,
            Name = "Toyota",
            Abbreviation = "TOYOTA",
        };
        var vehicleModel = new VehicleModel
        {
            Id = 1,
            Name = "Camry",
            Abbreviation = "CAMRY",
            MakeId = 1,
        };
        context.Vehicles.Add(make);
        context.VehicleModels.Add(vehicleModel);
        await context.SaveChangesAsync();

        var service = CreateVehicleService(context);
        var updateDto = new UpdateVehicleModelDto
        {
            Name = "Updated Camry",
            Abbreviation = "UPDATED_CAMRY",
        };

        var result = await service.UpdateVehicleModelAsync(1, updateDto);

        Assert.True(result);
        var updatedModel = await context.VehicleModels.FindAsync(1);
        Assert.Equal("Updated Camry", updatedModel.Name);
        Assert.Equal("UPDATED_CAMRY", updatedModel.Abbreviation);
    }

    [Fact]
    public async Task DeleteVehicleModelAsync_ExistingModel_DeletesSuccessfully()
    {
        await using var context = CreateTestContext();
        var make = new VehicleMake
        {
            Id = 1,
            Name = "Toyota",
            Abbreviation = "TOYOTA",
        };
        var vehicleModel = new VehicleModel
        {
            Id = 1,
            Name = "Camry",
            Abbreviation = "CAMRY",
            MakeId = 1,
        };
        context.Vehicles.Add(make);
        context.VehicleModels.Add(vehicleModel);
        await context.SaveChangesAsync();

        var service = CreateVehicleService(context);

        var result = await service.DeleteVehicleModelAsync(1);

        Assert.True(result);
        await using var assertContext = CreateTestContext();
        Assert.Null(await assertContext.VehicleModels.FindAsync(1));
    }
}
