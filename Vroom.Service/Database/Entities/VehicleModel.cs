namespace Vroom.Service.Database.Entities;

public class VehicleModel : IVehicle
{
    public int Id { get; set; }

    public int MakeId { get; set; }
    public VehicleMake Make { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
}
