using CameraAssignment.Core.Models;

namespace CameraAssignment.Core.Interfaces;

public interface ICameraService
{
    IEnumerable<Camera> GetAll();
    IEnumerable<Camera> Search(string name);
}