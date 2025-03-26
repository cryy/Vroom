using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vroom.Dto;
using Vroom.Service.Pagination;
using Vroom.Service.Services;

namespace Vroom.Controllers;

public class VehicleMakeController : Controller
{
    private readonly IVehicleService _vehicleService;
    private readonly IMapper _mapper;

    public VehicleMakeController(IVehicleService vehicleService, IMapper mapper)
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

        var makes = await _vehicleService.GetVehicleMakesAsync(filterParams);
        return View(makes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var make = await _vehicleService.GetVehicleMakeByIdAsync(id);

        if (make == null)
        {
            return NotFound();
        }

        return View(make);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVehicleMakeDto makeDto)
    {
        if (!ModelState.IsValid) return View(makeDto);
        
        try
        {
            var createdMake = await _vehicleService.CreateVehicleMakeAsync(makeDto);
            return RedirectToAction(nameof(Details), new { id = createdMake.Id });
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Unable to create vehicle.");
        }
        return View(makeDto);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var make = await _vehicleService.GetVehicleMakeByIdAsync(id);

        if (make == null)
        {
            return NotFound();
        }

        var updateDto = _mapper.Map<UpdateVehicleMakeDto>(make);
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateVehicleMakeDto makeDto)
    {
        if (!ModelState.IsValid) return View(makeDto);
        
        var updated = await _vehicleService.UpdateVehicleMakeAsync(id, makeDto);

        if (updated)
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        ModelState.AddModelError(string.Empty, "Unable to update vehicle.");
        return View(makeDto);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var make = await _vehicleService.GetVehicleMakeByIdAsync(id);

        if (make == null)
        {
            return NotFound();
        }

        return View(make);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _vehicleService.DeleteVehicleMakeAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
