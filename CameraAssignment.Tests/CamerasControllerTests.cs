using CameraAssignment.API.Controllers;
using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CameraAssignment.Tests;

[TestFixture]
public class CamerasControllerTests
{
    private Mock<ICameraService> _mockCameraService;
    private CamerasController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockCameraService = new Mock<ICameraService>();
        _controller = new CamerasController(_mockCameraService.Object);
    }

    #region GetAll Tests

    [Test]
    public void GetAll_WithCameras_ReturnsOkWithCameras()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421, Longitude = 5.118278 },
            new Camera { Number = 502, Name = "UTR-CM-502 Potterstraat", Latitude = 52.093599, Longitude = 5.118325 }
        };
        _mockCameraService.Setup(s => s.GetAll()).Returns(cameras);

        // Act
        var result = _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(cameras));
        _mockCameraService.Verify(s => s.GetAll(), Times.Once);
    }

    [Test]
    public void GetAll_WithNoCameras_ReturnsOkWithEmptyList()
    {
        // Arrange
        var cameras = new List<Camera>();
        _mockCameraService.Setup(s => s.GetAll()).Returns(cameras);

        // Act
        var result = _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(cameras));
    }

    [Test]
    public void GetAll_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockCameraService.Setup(s => s.GetAll()).Throws(new Exception("Database error"));

        // Act
        var result = _controller.GetAll();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value.ToString(), Does.Contain("Failed to retrieve cameras"));
    }

    #endregion

    #region Search Tests

    [Test]
    public void Search_WithValidName_ReturnsOkWithMatchingCameras()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421, Longitude = 5.118278 },
            new Camera { Number = 503, Name = "UTR-CM-503 Neude plein", Latitude = 52.093448, Longitude = 5.118536 }
        };
        _mockCameraService.Setup(s => s.Search("Neude")).Returns(cameras);

        // Act
        var result = _controller.Search("Neude");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(cameras));
        _mockCameraService.Verify(s => s.Search("Neude"), Times.Once);
    }

    [Test]
    public void Search_WithNoMatches_ReturnsOkWithEmptyList()
    {
        // Arrange
        var cameras = new List<Camera>();
        _mockCameraService.Setup(s => s.Search("NonExistent")).Returns(cameras);

        // Act
        var result = _controller.Search("NonExistent");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(cameras));
    }

    [Test]
    public void Search_WithNullName_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Search(null);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value.ToString(), Does.Contain("Search name parameter is required"));
        _mockCameraService.Verify(s => s.Search(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Search_WithEmptyName_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Search("");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value.ToString(), Does.Contain("Search name parameter is required"));
    }

    [Test]
    public void Search_WithWhitespaceName_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Search("   ");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public void Search_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockCameraService.Setup(s => s.Search("test")).Throws(new Exception("Search error"));

        // Act
        var result = _controller.Search("test");

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value.ToString(), Does.Contain("Failed to search cameras"));
    }

    #endregion

    #region GetByNumber Tests

    [Test]
    public void GetByNumber_WithExistingCamera_ReturnsOkWithCamera()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421, Longitude = 5.118278 },
            new Camera { Number = 502, Name = "UTR-CM-502 Potterstraat", Latitude = 52.093599, Longitude = 5.118325 }
        };
        _mockCameraService.Setup(s => s.GetAll()).Returns(cameras);

        // Act
        var result = _controller.GetByNumber(501);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnedCamera = okResult.Value as Camera;
        Assert.That(returnedCamera.Number, Is.EqualTo(501));
        Assert.That(returnedCamera.Name, Is.EqualTo("UTR-CM-501 Neude rijbaan voor Postkantoor"));
    }

    [Test]
    public void GetByNumber_WithNonExistentCamera_ReturnsNotFound()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421, Longitude = 5.118278 }
        };
        _mockCameraService.Setup(s => s.GetAll()).Returns(cameras);

        // Act
        var result = _controller.GetByNumber(9999);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.That(notFoundResult.Value.ToString(), Does.Contain("Camera with number 9999 not found"));
    }

    [Test]
    public void GetByNumber_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockCameraService.Setup(s => s.GetAll()).Throws(new Exception("Database error"));

        // Act
        var result = _controller.GetByNumber(501);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value.ToString(), Does.Contain("Failed to retrieve camera"));
    }

    [Test]
    public void GetByNumber_WithZeroNumber_ReturnsNotFound()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421, Longitude = 5.118278 }
        };
        _mockCameraService.Setup(s => s.GetAll()).Returns(cameras);

        // Act
        var result = _controller.GetByNumber(0);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public void GetByNumber_WithNegativeNumber_ReturnsNotFound()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421, Longitude = 5.118278 }
        };
        _mockCameraService.Setup(s => s.GetAll()).Returns(cameras);

        // Act
        var result = _controller.GetByNumber(-1);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    #endregion

    #region Constructor Tests

    [Test]
    public void Constructor_WithNullCameraService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new CamerasController(null));
        Assert.That(ex.ParamName, Is.EqualTo("cameraService"));
    }

    #endregion

    #region Edge Case Tests

    [Test]
    public void Search_WithSpecialCharacters_CallsServiceCorrectly()
    {
        // Arrange
        var searchTerm = "Neude / Schoutenstraat";
        var cameras = new List<Camera>
        {
            new Camera { Number = 504, Name = "UTR-CM-504 Neude / Schoutenstraat", Latitude = 52.092995, Longitude = 5.119088 }
        };
        _mockCameraService.Setup(s => s.Search(searchTerm)).Returns(cameras);

        // Act
        var result = _controller.Search(searchTerm);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        _mockCameraService.Verify(s => s.Search(searchTerm), Times.Once);
    }

    [Test]
    public void GetByNumber_WithDuplicateNumbers_ReturnsFirstMatch()
    {
        // Arrange - This shouldn't happen in real data, but testing edge case
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "UTR-CM-501 First", Latitude = 52.093421, Longitude = 5.118278 },
            new Camera { Number = 501, Name = "UTR-CM-501 Second", Latitude = 52.093422, Longitude = 5.118279 }
        };
        _mockCameraService.Setup(s => s.GetAll()).Returns(cameras);

        // Act
        var result = _controller.GetByNumber(501);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnedCamera = okResult.Value as Camera;
        Assert.That(returnedCamera.Name, Is.EqualTo("UTR-CM-501 First"));
    }

    #endregion
}
