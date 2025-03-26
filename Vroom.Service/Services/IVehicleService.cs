using Vroom.Dto;
using Vroom.Service.Pagination;

namespace Vroom.Service.Services;

public interface IVehicleService
{
    // vehicle make
    Task<VehicleMakeDto?> GetVehicleMakeByIdAsync(int id);
    Task<PagedResult<VehicleMakeDto>> GetVehicleMakesAsync(FilteringParams filteringParams);
    Task<VehicleMakeDto> CreateVehicleMakeAsync(CreateVehicleMakeDto dto);
    Task<bool> UpdateVehicleMakeAsync(int id, UpdateVehicleMakeDto dto);
    Task<bool> DeleteVehicleMakeAsync(int id);

    // vehicle model
    Task<VehicleModelDto?> GetVehicleModelByIdAsync(int id);
    Task<PagedResult<VehicleModelDto>> GetVehicleModelsAsync(FilteringParams filteringParams);
    Task<VehicleModelDto> CreateVehicleModelAsync(CreateVehicleModelDto dto);
    Task<bool> UpdateVehicleModelAsync(int id, UpdateVehicleModelDto dto);
    Task<bool> DeleteVehicleModelAsync(int id);
}
