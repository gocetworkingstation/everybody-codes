using System.Globalization;
using CameraAssignment.Core;
using CameraAssignment.Core.Models;
using CsvHelper;
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
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(new List<Camera>
            {
                new Camera { Number = 501, Name = "UTR-CM-501 Neude rijbaan voor Postkantoor", Latitude = 52.093421, Longitude = 5.118278 },
                new Camera { Number = 502, Name = "UTR-CM-502 Vinkenburgstraat", Latitude = 52.092378, Longitude = 5.117902 }
            });
        }
    }