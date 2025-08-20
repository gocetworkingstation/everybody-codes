using CsvHelper.Configuration.Attributes;

namespace CameraAssignment.Core.Models;

public class CameraCsvRecord
{
    [Name("Camera")]
    public string Camera { get; set; } = string.Empty;
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
}
