using CameraAssignment.Core;
using Assert = NUnit.Framework.Assert;

namespace CameraAssignment.Tests;

[TestFixture]
public class CameraRepositoryErrorHandlingTests
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
    public void LoadCameras_WithErrorRows_SkipsErrorEntries()
    {
        // Arrange
        CreateCsvWithErrors(_testCsvPath);
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.LoadCameras().ToList();

        // Assert
        Assert.That(cameras.Count, Is.EqualTo(2)); // Should skip the ERROR row
        Assert.That(cameras.All(c => !c.Name.StartsWith("ERROR")), Is.True);
        Assert.That(cameras.Any(c => c.Number == 501), Is.True);
        Assert.That(cameras.Any(c => c.Number == 503), Is.True);
    }

    [Test]
    public void LoadCameras_WithMalformedRows_SkipsMalformedEntries()
    {
        // Arrange
        CreateCsvWithMalformedRows(_testCsvPath);
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.LoadCameras().ToList();

        // Assert
        Assert.That(cameras.Count, Is.EqualTo(1)); // Should skip malformed rows
        Assert.That(cameras[0].Number, Is.EqualTo(501));
    }

    [Test]
    public void LoadCameras_WithEmptyFile_ReturnsEmptyList()
    {
        // Arrange
        File.WriteAllText(_testCsvPath, "Camera;Latitude;Longitude\n");
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.LoadCameras().ToList();

        // Assert
        Assert.That(cameras.Count, Is.EqualTo(0));
    }

    [Test]
    public void LoadCameras_WithOnlyHeaderRow_ReturnsEmptyList()
    {
        // Arrange
        File.WriteAllText(_testCsvPath, "Camera;Latitude;Longitude");
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.LoadCameras().ToList();

        // Assert
        Assert.That(cameras.Count, Is.EqualTo(0));
    }

    [Test]
    public void LoadCameras_WithMissingCameraNumber_AssignsZero()
    {
        // Arrange
        CreateCsvWithNonStandardNames(_testCsvPath);
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.LoadCameras().ToList();

        // Assert
        var nonStandardCamera = cameras.FirstOrDefault(c => c.Name == "Some Camera Without Number");
        Assert.That(nonStandardCamera, Is.Not.Null);
        Assert.That(nonStandardCamera.Number, Is.EqualTo(0));
    }

    [Test]
    public void SearchCameras_WithErrorInData_ReturnsValidResults()
    {
        // Arrange
        CreateCsvWithErrors(_testCsvPath);
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.SearchCameras("Neude").ToList();

        // Assert
        Assert.That(cameras.Count, Is.EqualTo(2));
        Assert.That(cameras.All(c => !c.Name.StartsWith("ERROR")), Is.True);
    }

    [Test]
    public void LoadCameras_WithFileNotFound_ThrowsFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = "non-existent-file.csv";
        var repository = new CameraRepository(nonExistentPath);

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => repository.LoadCameras().ToList());
    }

    [Test]
    public void LoadCameras_WithInvalidCoordinates_SkipsInvalidRows()
    {
        // Arrange
        CreateCsvWithInvalidCoordinates(_testCsvPath);
        var repository = new CameraRepository(_testCsvPath);

        // Act
        var cameras = repository.LoadCameras().ToList();

        // Assert
        Assert.That(cameras.Count, Is.EqualTo(1)); // Should skip rows with invalid coordinates
        Assert.That(cameras[0].Number, Is.EqualTo(501));
    }

    private void CreateCsvWithErrors(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Camera;Latitude;Longitude");
        writer.WriteLine("UTR-CM-501 Neude rijbaan voor Postkantoor;52.093421;5.118278");
        writer.WriteLine("ERROR 1207 Could not read camera information from database");
        writer.WriteLine("UTR-CM-503 Neude plein;52.093448;5.118536");
    }

    private void CreateCsvWithMalformedRows(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Camera;Latitude;Longitude");
        writer.WriteLine("UTR-CM-501 Neude rijbaan voor Postkantoor;52.093421;5.118278");
        writer.WriteLine("UTR-CM-502 Incomplete row;52.093421"); // Missing longitude
        writer.WriteLine("UTR-CM-503 Too many;fields;52.093421;5.118536;extra"); // Too many fields
    }

    private void CreateCsvWithNonStandardNames(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Camera;Latitude;Longitude");
        writer.WriteLine("UTR-CM-501 Neude rijbaan voor Postkantoor;52.093421;5.118278");
        writer.WriteLine("Some Camera Without Number;52.093421;5.118278");
    }

    private void CreateCsvWithInvalidCoordinates(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Camera;Latitude;Longitude");
        writer.WriteLine("UTR-CM-501 Neude rijbaan voor Postkantoor;52.093421;5.118278");
        writer.WriteLine("UTR-CM-502 Invalid Coords;not_a_number;5.118278");
        writer.WriteLine("UTR-CM-503 Invalid Coords;52.093421;not_a_number");
    }
}
