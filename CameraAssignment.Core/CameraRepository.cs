using System.Globalization;
using System.Text.RegularExpressions;
using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace CameraAssignment.Core;

public class CameraRepository(string filePath) : ICameraRepository
{
    private readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

    public IEnumerable<Camera> LoadCameras()
    {
        using var reader = new StreamReader(_filePath);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            MissingFieldFound = null, // Ignore missing fields
            HeaderValidated = null,   // Skip header validation
            BadDataFound = null       // Skip bad data
        };
        using var csv = new CsvReader(reader, config);
        
        var cameras = new List<Camera>();
        
        csv.Read();
        csv.ReadHeader();
        
        while (csv.Read())
        {
            try
            {
                var record = csv.GetRecord<CameraCsvRecord>();
                if (record != null && !string.IsNullOrEmpty(record.Camera) && !record.Camera.StartsWith("ERROR"))
                {
                    cameras.Add(new Camera
                    {
                        Number = ExtractCameraNumber(record.Camera),
                        Name = record.Camera,
                        Latitude = record.Latitude,
                        Longitude = record.Longitude
                    });
                }
            }
            catch
            {
                // Skip invalid rows
                continue;
            }
        }
        
        return cameras;
    }

    public IEnumerable<Camera> SearchCameras(string name)
    {
        var cameras = LoadCameras();
        return string.IsNullOrEmpty(name)
            ? cameras
            : cameras.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    private static int ExtractCameraNumber(string cameraName)
    {
        // Extract number from camera name like "UTR-CM-501 Neude rijbaan voor Postkantoor"
        var match = Regex.Match(cameraName, @"UTR-CM-(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
}