using System.Globalization;
using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;
using CsvHelper;

namespace CameraAssignment.Core;

public class CameraRepository(string filePath) : ICameraRepository
{
    private readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

    public IEnumerable<Camera> LoadCameras()
    {
        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<Camera>().ToList();
    }

    public IEnumerable<Camera> SearchCameras(string name)
    {
        var cameras = LoadCameras();
        return string.IsNullOrEmpty(name)
            ? cameras
            : cameras.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }
}