using CameraAssignment.Core;
using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;
using Moq;

namespace CameraAssignment.Tests;

[TestFixture]
public class CameraServiceTests
{
    private Mock<ICameraRepository> _mockRepository;
    private CameraService _service;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICameraRepository>();
        _service = new CameraService(_mockRepository.Object);
    }

    [Test]
    public void GetAll_CallsRepositoryLoadCameras_ReturnsCameras()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera
            {
                Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421,
                Longitude = 5.118278
            }
        };
        _mockRepository.Setup(r => r.LoadCameras()).Returns(cameras);

        // Act
        var result = _service.GetAll().ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Number, Is.EqualTo(501));
        _mockRepository.Verify(r => r.LoadCameras(), Times.Once());
    }

    [Test]
    public void Search_CallsRepositorySearchCameras_ReturnsMatchingCameras()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera
            {
                Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421,
                Longitude = 5.118278
            }
        };
        _mockRepository.Setup(r => r.SearchCameras("Neude")).Returns(cameras);

        // Act
        var result = _service.Search("Neude").ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Number, Is.EqualTo(501));
        _mockRepository.Verify(r => r.SearchCameras("Neude"), Times.Once());
    }

    [Test]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CameraService(null));
    }
}