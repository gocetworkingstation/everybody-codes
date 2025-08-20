using CameraAssignment.API.Controllers;
using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CameraAssignment.Tests;

[TestFixture]
public class ApiTestValidation
{
    [Test]
    public void ValidateControllerInstantiation_Works()
    {
        // Arrange
        var mockService = new Mock<ICameraService>();
        
        // Act & Assert
        Assert.DoesNotThrow(() => new CamerasController(mockService.Object));
    }

    [Test]
    public void ValidateBasicControllerFunctionality_Works()
    {
        // Arrange
        var mockService = new Mock<ICameraService>();
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "Test Camera", Latitude = 52.0, Longitude = 5.0 }
        };
        mockService.Setup(s => s.GetAll()).Returns(cameras);
        var controller = new CamerasController(mockService.Object);

        // Act
        var result = controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(cameras));
    }
}
