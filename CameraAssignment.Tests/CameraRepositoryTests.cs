using CameraAssignment.Core;
using Assert = NUnit.Framework.Assert;

namespace CameraAssignment.Tests;

[TestFixture]
    public class CameraRepositoryTests
    {
        private string _testCsvPath;

        [SetUp]
        public void SetUp()
        {
            _testCsvPath = Path.GetTempFileName();
            CreateTestCsv(_testCsvPath);
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
        public void LoadCameras_ReturnsAllCamerasFromCsv()
        {
            // Arrange
            var repository = new CameraRepository(_testCsvPath);

            // Act
            var cameras = repository.LoadCameras().ToList();

            // Assert
            Assert.That(cameras.Count, Is.EqualTo(2));
            Assert.That(cameras.Any(c => c.Number == 501 && c.Name == "UTR-CM-501 Neude rijbaan voor Postkantoor"));
            Assert.That(cameras.Any(c => c.Number == 502 && c.Name == "UTR-CM-502 Vinkenburgstraat"));
        }

        [Test]
        public void SearchCameras_WithName_ReturnsMatchingCameras()
        {
            // Arrange
            var repository = new CameraRepository(_testCsvPath);

            // Act
            var cameras = repository.SearchCameras("Neude").ToList();

            // Assert
            Assert.That(cameras.Count, Is.EqualTo(1));
            Assert.That(cameras[0].Number, Is.EqualTo(501));
            Assert.That(cameras[0].Name, Is.EqualTo("UTR-CM-501 Neude rijbaan voor Postkantoor"));
        }

        [Test]
        public void SearchCameras_WithEmptyName_ReturnsAllCameras()
        {
            // Arrange
            var repository = new CameraRepository(_testCsvPath);

            // Act
            var cameras = repository.SearchCameras(null).ToList();

            // Assert
            Assert.That(cameras.Count, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_WithNullFilePath_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CameraRepository(null));
        }

        private void CreateTestCsv(string filePath)
        {
            using var writer = new StreamWriter(filePath);
            // Write CSV with semicolon delimiter to match actual data format
            writer.WriteLine("Camera;Latitude;Longitude");
            writer.WriteLine("UTR-CM-501 Neude rijbaan voor Postkantoor;52.093421;5.118278");
            writer.WriteLine("UTR-CM-502 Vinkenburgstraat;52.092378;5.117902");
        }
    }