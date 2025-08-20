using CameraAssignment.Core;
using Assert = NUnit.Framework.Assert;

namespace CameraAssignment.Tests;

[TestFixture]
public class CameraNumberExtractionTests
{
    private string _testCsvPath;

    [SetUp]
    public void SetUp()
    {
        _testCsvPath = Path.GetTempFileName();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_testCsvPath))
        {
            File.Delete(_testCsvPath);
        }
    }

    [Test]
    public void LoadCameras_ExtractsCorrectCameraNumbers()
    {
        // Arrange
        CreateCsvWithVariousCameraNumbers(_testCsvPath);
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.LoadCameras().ToList();

        // Assert
        Assert.That(cameras.Count, Is.EqualTo(5));
        
        var camera501 = cameras.FirstOrDefault(c => c.Number == 501);
        Assert.That(camera501, Is.Not.Null);
        Assert.That(camera501.Name, Is.EqualTo("UTR-CM-501 Neude rijbaan voor Postkantoor"));
        
        var camera1234 = cameras.FirstOrDefault(c => c.Number == 1234);
        Assert.That(camera1234, Is.Not.Null);
        Assert.That(camera1234.Name, Is.EqualTo("UTR-CM-1234 Some location"));
        
        var camera9 = cameras.FirstOrDefault(c => c.Number == 9);
        Assert.That(camera9, Is.Not.Null);
        Assert.That(camera9.Name, Is.EqualTo("UTR-CM-9 Single digit"));
    }

    [Test]
    public void LoadCameras_WithNonStandardCameraNames_AssignsZeroNumber()
    {
        // Arrange
        CreateCsvWithNonStandardNames(_testCsvPath);
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.LoadCameras().ToList();

        // Assert
        var nonStandardCameras = cameras.Where(c => c.Number == 0).ToList();
        Assert.That(nonStandardCameras.Count, Is.EqualTo(2));
        Assert.That(nonStandardCameras.Any(c => c.Name == "CAM-ABC-123 Different format"), Is.True);
        Assert.That(nonStandardCameras.Any(c => c.Name == "Regular Camera Name"), Is.True);
    }

    [Test]
    public void LoadCameras_WithMixedFormats_HandlesCorrectly()
    {
        // Arrange
        CreateCsvWithMixedFormats(_testCsvPath);
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.LoadCameras().ToList();

        // Assert
        Assert.That(cameras.Count, Is.EqualTo(4));
        
        // Standard format should extract number
        var standardCamera = cameras.FirstOrDefault(c => c.Number == 501);
        Assert.That(standardCamera, Is.Not.Null);
        
        // Non-standard formats should get number 0
        var nonStandardCameras = cameras.Where(c => c.Number == 0).ToList();
        Assert.That(nonStandardCameras.Count, Is.EqualTo(3));
    }

    [Test]
    public void SearchCameras_FindsCamerasByPartialName()
    {
        // Arrange
        CreateCsvWithVariousCameraNumbers(_testCsvPath);
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var neudeCamera = repository.SearchCameras("Neude").ToList();
        var digitCameras = repository.SearchCameras("digit").ToList();

        // Assert
        Assert.That(neudeCamera.Count, Is.EqualTo(1));
        Assert.That(neudeCamera[0].Number, Is.EqualTo(501));
        
        Assert.That(digitCameras.Count, Is.EqualTo(2)); // "Single digit" and "Three digit"
    }

    private void CreateCsvWithVariousCameraNumbers(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Camera;Latitude;Longitude");
        writer.WriteLine("UTR-CM-501 Neude rijbaan voor Postkantoor;52.093421;5.118278");
        writer.WriteLine("UTR-CM-1234 Some location;52.093421;5.118278");
        writer.WriteLine("UTR-CM-9 Single digit;52.093421;5.118278");
        writer.WriteLine("UTR-CM-999 Three digit;52.093421;5.118278");
        writer.WriteLine("UTR-CM-42 Answer to everything;52.093421;5.118278");
    }

    private void CreateCsvWithNonStandardNames(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Camera;Latitude;Longitude");
        writer.WriteLine("UTR-CM-501 Standard format;52.093421;5.118278");
        writer.WriteLine("CAM-ABC-123 Different format;52.093421;5.118278");
        writer.WriteLine("Regular Camera Name;52.093421;5.118278");
    }

    private void CreateCsvWithMixedFormats(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Camera;Latitude;Longitude");
        writer.WriteLine("UTR-CM-501 Standard format;52.093421;5.118278");
        writer.WriteLine("DIFF-CM-123 Different prefix;52.093421;5.118278");
        writer.WriteLine("UTR-XY-456 Different middle;52.093421;5.118278");
        writer.WriteLine("No numbers here;52.093421;5.118278");
    }
}
