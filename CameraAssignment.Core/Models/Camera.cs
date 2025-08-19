namespace CameraAssignment.Core.Models;

public class Camera
{
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}