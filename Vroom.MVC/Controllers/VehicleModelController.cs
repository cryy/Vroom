using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vroom.Dto;
using Vroom.Service.Pagination;
using Vroom.Service.Services;

namespace Vroom.Controllers;

public class VehicleModelController : Controller
{
    private readonly IVehicleService _vehicleService;
    private readonly IMapper _mapper;

    public VehicleModelController(IVehicleService vehicleService, IMapper mapper)
    {
        _vehicleService = vehicleService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(
        [FromQuery] string? searchQuery,
        [FromQuery] string? sortBy,
        [FromQuery] bool descending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var filterParams = new FilteringParams
        {
            SearchQuery = searchQuery,
            SortBy = Enum.TryParse<Sort>(sortBy, out var sort) ? sort : Sort.Id,
            Descending = descending,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var models = await _vehicleService.GetVehicleModelsAsync(filterParams);
        return View(models);
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await _vehicleService.GetVehicleModelByIdAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateVehicleMakesDropdown();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVehicleModelDto modelDto)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var createdModel = await _vehicleService.CreateVehicleModelAsync(modelDto);
                return RedirectToAction(nameof(Details), new { id = createdModel.Id });
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Unable to create vehicle model.");
            }
        }

        await PopulateVehicleMakesDropdown();
        return View(modelDto);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _vehicleService.GetVehicleModelByIdAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        var updateDto = _mapper.Map<UpdateVehicleModelDto>(model);

        await PopulateVehicleMakesDropdown();
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateVehicleModelDto modelDto)
    {
        if (ModelState.IsValid)
        {
            var updated = await _vehicleService.UpdateVehicleModelAsync(id, modelDto);

            if (updated)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            ModelState.AddModelError(string.Empty, "Unable to update vehicle model.");
        }

        await PopulateVehicleMakesDropdown();
        return View(modelDto);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await _vehicleService.GetVehicleModelByIdAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _vehicleService.DeleteVehicleModelAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateVehicleMakesDropdown()
    {
        var filterParams = new FilteringParams
        {
            SearchQuery = null,
            SortBy = Sort.Id,
            Descending = false,
            PageNumber = 1,
            PageSize = 100,
        };

        var makes = await _vehicleService.GetVehicleMakesAsync(filterParams);
        ViewBag.VehicleMakes = makes.Items;
    }
}
