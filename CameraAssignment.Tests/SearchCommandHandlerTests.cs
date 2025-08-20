using CameraAssignment.Cli;
using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;
using Moq;

namespace CameraAssignment.Tests;

[TestFixture]
public class SearchCommandHandlerTests
{
    private Mock<ICameraService> _mockCameraService;
    private SearchCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockCameraService = new Mock<ICameraService>();
        _handler = new SearchCommandHandler(_mockCameraService.Object);
    }

    [Test]
    public void ExecuteSearch_WithValidName_ReturnsFormattedResults()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421, Longitude = 5.118278 },
            new Camera { Number = 503, Name = "UTR-CM-503 Neude plein", Latitude = 52.093448, Longitude = 5.118536 }
        };
        _mockCameraService.Setup(s => s.Search("Neude")).Returns(cameras);

        // Act
        var results = _handler.ExecuteSearch("Neude").ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results[0], Is.EqualTo("501 | UTR-CM-501 Neude rijbaan voor Postkantoor | 52.093421 | 5.118278"));
        Assert.That(results[1], Is.EqualTo("503 | UTR-CM-503 Neude plein | 52.093448 | 5.118536"));
        _mockCameraService.Verify(s => s.Search("Neude"), Times.Once);
    }

    [Test]
    public void ExecuteSearch_WithNoCameras_ReturnsEmptyResults()
    {
        // Arrange
        _mockCameraService.Setup(s => s.Search("NonExistent")).Returns(new List<Camera>());

        // Act
        var results = _handler.ExecuteSearch("NonExistent").ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(0));
        _mockCameraService.Verify(s => s.Search("NonExistent"), Times.Once);
    }

    [Test]
    public void ExecuteSearch_WithSingleCamera_ReturnsCorrectFormat()
    {
        // Arrange
        var camera = new Camera { Number = 507, Name = "UTR-CM-507 Vinkenburgstraat richting Neude", Latitude = 52.092234, Longitude = 5.117766 };
        _mockCameraService.Setup(s => s.Search("Vinkenburgstraat")).Returns(new List<Camera> { camera });

        // Act
        var results = _handler.ExecuteSearch("Vinkenburgstraat").ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0], Is.EqualTo("507 | UTR-CM-507 Vinkenburgstraat richting Neude | 52.092234 | 5.117766"));
    }

    [Test]
    public void ExecuteSearch_WithNullName_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _handler.ExecuteSearch(null));
        Assert.That(ex.ParamName, Is.EqualTo("name"));
        Assert.That(ex.Message, Does.Contain("Search name cannot be null or empty"));
    }

    [Test]
    public void ExecuteSearch_WithEmptyName_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _handler.ExecuteSearch(""));
        Assert.That(ex.ParamName, Is.EqualTo("name"));
    }

    [Test]
    public void ExecuteSearch_WithWhiteSpaceName_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _handler.ExecuteSearch("   "));
        Assert.That(ex.ParamName, Is.EqualTo("name"));
    }

    [Test]
    public void Constructor_WithNullCameraService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new SearchCommandHandler(null));
        Assert.That(ex.ParamName, Is.EqualTo("cameraService"));
    }

    [Test]
    public void ExecuteSearch_CaseInsensitiveSearch_CallsServiceCorrectly()
    {
        // Arrange
        var cameras = new List<Camera>
        {
            new Camera { Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421, Longitude = 5.118278 }
        };
        _mockCameraService.Setup(s => s.Search("neude")).Returns(cameras);

        // Act
        var results = _handler.ExecuteSearch("neude").ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(1));
        _mockCameraService.Verify(s => s.Search("neude"), Times.Once);
    }

    [Test]
    public void ExecuteSearch_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var camera = new Camera { Number = 504, Name = "UTR-CM-504 Neude / Schoutenstraat", Latitude = 52.092995, Longitude = 5.119088 };
        _mockCameraService.Setup(s => s.Search("Neude /")).Returns(new List<Camera> { camera });

        // Act
        var results = _handler.ExecuteSearch("Neude /").ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0], Does.Contain("UTR-CM-504 Neude / Schoutenstraat"));
    }
}
