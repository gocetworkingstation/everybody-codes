using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;

namespace CameraAssignment.Core;

public class CameraService : ICameraService
{
    private readonly ICameraRepository _repository;

    public CameraService(ICameraRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public IEnumerable<Camera> GetAll()
    {
        return _repository.LoadCameras();
    }

    public IEnumerable<Camera> Search(string name)
    {
        return _repository.SearchCameras(name);
    }
}