namespace Vroom.Dto;

public class VehicleModelDto
{
    public int Id { get; set; }
    public int MakeId { get; set; }
    public string MakeName { get; set; } = "";
    public string Name { get; set; } = "";
    public string Abbreviation { get; set; } = "";
}
