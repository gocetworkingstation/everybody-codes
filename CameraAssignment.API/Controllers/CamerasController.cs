using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraAssignment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CamerasController : ControllerBase
{
    private readonly ICameraService _cameraService;

    public CamerasController(ICameraService cameraService)
    {
        _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
    }

    /// <summary>
    /// Gets all cameras
    /// </summary>
    /// <returns>List of all cameras</returns>
    [HttpGet]
    public ActionResult<IEnumerable<Camera>> GetAll()
    {
        try
        {
            var cameras = _cameraService.GetAll();
            return Ok(cameras);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve cameras", details = ex.Message });
        }
    }

    /// <summary>
    /// Searches cameras by name
    /// </summary>
    /// <param name="name">Search term for camera name</param>
    /// <returns>List of cameras matching the search term</returns>
    [HttpGet("search")]
    public ActionResult<IEnumerable<Camera>> Search([FromQuery] string? name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { error = "Search name parameter is required" });
            }

            var cameras = _cameraService.Search(name);
            return Ok(cameras);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to search cameras", details = ex.Message });
        }
    }

    /// <summary>
    /// Gets a specific camera by number
    /// </summary>
    /// <param name="number">Camera number</param>
    /// <returns>Camera with the specified number</returns>
    [HttpGet("{number:int}")]
    public ActionResult<Camera> GetByNumber(int number)
    {
        try
        {
            var cameras = _cameraService.GetAll();
            var camera = cameras.FirstOrDefault(c => c.Number == number);
            
            if (camera == null)
            {
                return NotFound(new { error = $"Camera with number {number} not found" });
            }

            return Ok(camera);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve camera", details = ex.Message });
        }
    }
}
