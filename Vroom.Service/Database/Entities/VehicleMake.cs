namespace Vroom.Service.Database.Entities;

public class VehicleMake : IVehicle
{
    public int Id { get; set; }

    public List<VehicleModel> Models { get; set; } = new();
    public string Name { get; set; }
    public string Abbreviation { get; set; }
}
