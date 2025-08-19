using CameraAssignment.Core.Models;

namespace CameraAssignment.Core.Interfaces;

public interface ICameraRepository
{
    IEnumerable<Camera> LoadCameras();
    IEnumerable<Camera> SearchCameras(string name);
}