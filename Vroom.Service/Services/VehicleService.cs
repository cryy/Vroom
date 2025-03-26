using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vroom.Dto;
using Vroom.Service.Database;
using Vroom.Service.Database.Entities;
using Vroom.Service.Extensions;
using Vroom.Service.Pagination;

namespace Vroom.Service.Services;

public class VehicleService : IVehicleService
{
    private readonly IDbContextFactory<VroomContext> _contextFactory;
    private readonly ILogger<VehicleService> _logger;
    private readonly IMapper _mapper;

    public VehicleService(
        IDbContextFactory<VroomContext> contextFactory,
        ILogger<VehicleService> logger,
        IMapper mapper
    )
    {
        _contextFactory = contextFactory;
        _logger = logger;
        _mapper = mapper;
    }

    //
    // Vehicle Makes
    //

    public async Task<VehicleMakeDto?> GetVehicleMakeByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid ID provided: {Id}", id);
            return null;
        }

        await using var ctx = await _contextFactory.CreateDbContextAsync();
        var dbMake = await ctx.FindAsync<VehicleMake>(id);

        if (dbMake == null)
        {
            _logger.LogWarning("{Type} with ID: {Id} not found", typeof(VehicleMake), id);
            return null;
        }

        _logger.LogInformation(
            "Successfully retrieved {Type} with ID: {Id}",
            typeof(VehicleMake),
            id
        );
        return _mapper.Map<VehicleMakeDto>(dbMake);
    }

    public async Task<PagedResult<VehicleMakeDto>> GetVehicleMakesAsync(
        FilteringParams filteringParams
    )
    {
        var (searchQuery, sortBy, descending, pageNumber, pageSize) = filteringParams;
        _logger.LogInformation(
            "Getting {Type}, with SearchQuery: {SearchQuery}, SortBy: {SortBy}, Descending: {Descending}, PageNumber: {PageNumber}, PageSize: {PageSize}",
            typeof(VehicleMake),
            searchQuery,
            sortBy,
            descending,
            pageNumber,
            pageSize
        );

        try
        {
            await using var ctx = await _contextFactory.CreateDbContextAsync();

            var result = await ctx
                .Vehicles.AsAsyncEnumerable()
                .FilterAsyncEnumerable(searchQuery)
                .SortAsyncEnumerable(sortBy, descending)
                .ToPagedResultAsync<VehicleMake, VehicleMakeDto>(pageNumber, pageSize, _mapper);

            _logger.LogInformation(
                "Successfully retrieved {TotalCount} {Type}s",
                result.TotalCount,
                typeof(VehicleMake)
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting {Type}s", typeof(VehicleMake));
            throw;
        }
    }

    public async Task<VehicleMakeDto> CreateVehicleMakeAsync(CreateVehicleMakeDto dto)
    {
        _logger.LogInformation(
            "Creating new {Type} with Name: {Name}",
            typeof(VehicleMake),
            dto.Name
        );

        var vehicleMake = _mapper.Map<VehicleMake>(dto);

        await using var ctx = await _contextFactory.CreateDbContextAsync();
        ctx.Vehicles.Add(vehicleMake);

        try
        {
            await ctx.SaveChangesAsync();
            _logger.LogInformation(
                "Successfully created {Type} with ID: {Id}",
                typeof(VehicleMake),
                vehicleMake.Id
            );
            return _mapper.Map<VehicleMakeDto>(vehicleMake);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while creating {Type} with Name: {Name}",
                typeof(VehicleMake),
                dto.Name
            );
            throw;
        }
    }

    public async Task<bool> UpdateVehicleMakeAsync(int id, UpdateVehicleMakeDto dto)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid ID provided: {Id}", id);
            return false;
        }

        _logger.LogInformation("Updating {Type} with ID: {Id}", typeof(VehicleMake), id);

        await using var ctx = await _contextFactory.CreateDbContextAsync();
        var vehicleMake = await ctx.Vehicles.FindAsync(id);

        if (vehicleMake == null)
        {
            _logger.LogWarning("{Type} with ID: {Id} not found", typeof(VehicleMake), id);
            return false;
        }

        _mapper.Map(dto, vehicleMake);

        try
        {
            await ctx.SaveChangesAsync();
            _logger.LogInformation(
                "Successfully updated {Type} with ID: {Id}",
                typeof(VehicleMake),
                id
            );
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while updating {Type} with ID: {Id}",
                typeof(VehicleMake),
                id
            );
            return false;
        }
    }

    public async Task<bool> DeleteVehicleMakeAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid ID provided: {Id}", id);
            return false;
        }

        _logger.LogInformation("Deleting {Type} with ID: {Id}", typeof(VehicleMake), id);

        await using var ctx = await _contextFactory.CreateDbContextAsync();
        var vehicleMake = await ctx.Vehicles.FindAsync(id);

        if (vehicleMake == null)
        {
            _logger.LogWarning("{Type} with ID: {Id} not found", typeof(VehicleMake), id);
            return false;
        }

        ctx.Vehicles.Remove(vehicleMake);

        try
        {
            await ctx.SaveChangesAsync();
            _logger.LogInformation(
                "Successfully deleted {Type} with ID: {Id}",
                typeof(VehicleMake),
                id
            );
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while deleting {Type} with ID: {Id}",
                typeof(VehicleMake),
                id
            );
            return false;
        }
    }

    //
    // Vehicle Models
    //

    public async Task<VehicleModelDto?> GetVehicleModelByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid ID provided: {Id}", id);
            return null;
        }

        _logger.LogInformation("Getting {Type} with ID: {Id}", typeof(VehicleModel), id);

        await using var ctx = await _contextFactory.CreateDbContextAsync();
        var dbModel = await ctx
            .VehicleModels.Include(vm => vm.Make)
            .FirstOrDefaultAsync(vm => vm.Id == id);

        if (dbModel == null)
        {
            _logger.LogWarning("{Type} with ID: {Id} not found", typeof(VehicleModel), id);
            return null;
        }

        _logger.LogInformation(
            "Successfully retrieved {Type} with ID: {Id}",
            typeof(VehicleModel),
            id
        );
        return _mapper.Map<VehicleModelDto>(dbModel);
    }

    public async Task<PagedResult<VehicleModelDto>> GetVehicleModelsAsync(
        FilteringParams filteringParams
    )
    {
        var (searchQuery, sortBy, descending, pageNumber, pageSize) = filteringParams;
        _logger.LogInformation(
            "Getting {Type}s with SearchQuery: {SearchQuery}, SortBy: {SortBy}, Descending: {Descending}, PageNumber: {PageNumber}, PageSize: {PageSize}",
            typeof(VehicleModel),
            searchQuery,
            sortBy,
            descending,
            pageNumber,
            pageSize
        );

        try
        {
            await using var ctx = await _contextFactory.CreateDbContextAsync();

            var result = await ctx
                .VehicleModels.Include(vm => vm.Make)
                .AsAsyncEnumerable()
                .FilterAsyncEnumerable(searchQuery)
                .SortAsyncEnumerable(sortBy, descending)
                .ToPagedResultAsync<VehicleModel, VehicleModelDto>(pageNumber, pageSize, _mapper);

            _logger.LogInformation(
                "Successfully retrieved {TotalCount} {Type}s",
                result.TotalCount,
                typeof(VehicleModel)
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting {Type}s", typeof(VehicleModel));
            throw;
        }
    }

    public async Task<VehicleModelDto> CreateVehicleModelAsync(CreateVehicleModelDto dto)
    {
        _logger.LogInformation(
            "Creating new {Type} with Name: {Name}",
            typeof(VehicleModel),
            dto.Name
        );

        var vehicleModel = _mapper.Map<VehicleModel>(dto);

        await using var ctx = await _contextFactory.CreateDbContextAsync();
        ctx.VehicleModels.Add(vehicleModel);

        try
        {
            await ctx.SaveChangesAsync();
            _logger.LogInformation(
                "Successfully created {Type} with ID: {Id}",
                typeof(VehicleModel),
                vehicleModel.Id
            );
            return _mapper.Map<VehicleModelDto>(vehicleModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while creating {Type} with Name: {Name}",
                typeof(VehicleModel),
                dto.Name
            );
            throw;
        }
    }

    public async Task<bool> UpdateVehicleModelAsync(int id, UpdateVehicleModelDto dto)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid ID provided: {Id}", id);
            return false;
        }

        _logger.LogInformation("Updating {Type} with ID: {Id}", typeof(VehicleModel), id);

        await using var ctx = await _contextFactory.CreateDbContextAsync();
        var vehicleModel = await ctx.VehicleModels.FindAsync(id);

        if (vehicleModel == null)
        {
            _logger.LogWarning("{Type} with ID: {Id} not found", typeof(VehicleModel), id);
            return false;
        }

        _mapper.Map(dto, vehicleModel);

        try
        {
            await ctx.SaveChangesAsync();
            _logger.LogInformation(
                "Successfully updated {Type} with ID: {Id}",
                typeof(VehicleModel),
                id
            );
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while updating {Type} with ID: {Id}",
                typeof(VehicleModel),
                id
            );
            return false;
        }
    }

    public async Task<bool> DeleteVehicleModelAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid ID provided: {Id}", id);
            return false;
        }

        _logger.LogInformation("Deleting {Type} with ID: {Id}", typeof(VehicleModel), id);

        await using var ctx = await _contextFactory.CreateDbContextAsync();
        var vehicleModel = await ctx.VehicleModels.FindAsync(id);

        if (vehicleModel == null)
        {
            _logger.LogWarning("{Type} with ID: {Id} not found", typeof(VehicleModel), id);
            return false;
        }

        ctx.VehicleModels.Remove(vehicleModel);

        try
        {
            await ctx.SaveChangesAsync();
            _logger.LogInformation(
                "Successfully deleted {Type} with ID: {Id}",
                typeof(VehicleModel),
                id
            );
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while deleting {Type} with ID: {Id}",
                typeof(VehicleModel),
                id
            );
            return false;
        }
    }
}
