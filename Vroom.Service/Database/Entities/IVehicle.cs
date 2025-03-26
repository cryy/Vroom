namespace Vroom.Service.Database.Entities;

public interface IVehicle
{
    public int Id { get; }
    public string Name { get; }
    public string Abbreviation { get; }
}
