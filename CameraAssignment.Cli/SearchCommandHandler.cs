using System.Globalization;
using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;

namespace CameraAssignment.Cli;

public class SearchCommandHandler
{
    private readonly ICameraService _cameraService;

    public SearchCommandHandler(ICameraService cameraService)
    {
        _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
    }

    public IEnumerable<string> ExecuteSearch(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Search name cannot be null or empty", nameof(name));
        }

        var cameras = _cameraService.Search(name);
        return cameras.Select(FormatCameraOutput);
    }

    private static string FormatCameraOutput(Camera camera)
    {
        return $"{camera.Number} | {camera.Name} | {camera.Latitude.ToString(CultureInfo.InvariantCulture)} | {camera.Longitude.ToString(CultureInfo.InvariantCulture)}";
    }
}
