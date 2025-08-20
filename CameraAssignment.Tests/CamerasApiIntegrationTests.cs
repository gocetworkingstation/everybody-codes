using System.Net;
using System.Text.Json;
using CameraAssignment.Core;
using CameraAssignment.Core.Interfaces;
using CameraAssignment.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CameraAssignment.Tests;

[TestFixture]
public class CamerasApiIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private string _testCsvPath;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Create a test CSV file
        _testCsvPath = Path.GetTempFileName();
        CreateTestCsv(_testCsvPath);

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    // Remove the existing camera services
                    var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICameraRepository));
                    if (serviceDescriptor != null)
                        services.Remove(serviceDescriptor);
                    
                    serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICameraService));
                    if (serviceDescriptor != null)
                        services.Remove(serviceDescriptor);
                    
                    // Add test services with test CSV file
                    services.AddSingleton<ICameraRepository>(new CameraRepository(_testCsvPath));
                    services.AddSingleton<ICameraService, CameraService>();
                });
            });

        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
        
        if (File.Exists(_testCsvPath))
        {
            File.Delete(_testCsvPath);
        }
    }

    #region GET /api/cameras Tests

    [Test]
    public async Task GetAllCameras_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }

    [Test]
    public async Task GetAllCameras_ReturnsExpectedCameras()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras");
        var content = await response.Content.ReadAsStringAsync();
        var cameras = JsonSerializer.Deserialize<Camera[]>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(cameras, Is.Not.Null);
        Assert.That(cameras.Length, Is.EqualTo(3));
        
        var camera501 = cameras.FirstOrDefault(c => c.Number == 501);
        Assert.That(camera501, Is.Not.Null);
        Assert.That(camera501.Name, Is.EqualTo("UTR-CM-501 Neude rijbaan voor Postkantoor"));
        Assert.That(camera501.Latitude, Is.EqualTo(52.093421).Within(0.000001));
        Assert.That(camera501.Longitude, Is.EqualTo(5.118278).Within(0.000001));
    }

    #endregion

    #region GET /api/cameras/search Tests

    [Test]
    public async Task SearchCameras_WithValidName_ReturnsMatchingCameras()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras/search?name=Neude");
        var content = await response.Content.ReadAsStringAsync();
        var cameras = JsonSerializer.Deserialize<Camera[]>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(cameras, Is.Not.Null);
        Assert.That(cameras.Length, Is.EqualTo(2));
        Assert.That(cameras.All(c => c.Name.Contains("Neude")), Is.True);
    }

    [Test]
    public async Task SearchCameras_WithNonExistentName_ReturnsEmptyArray()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras/search?name=NonExistent");
        var content = await response.Content.ReadAsStringAsync();
        var cameras = JsonSerializer.Deserialize<Camera[]>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(cameras, Is.Not.Null);
        Assert.That(cameras.Length, Is.EqualTo(0));
    }

    [Test]
    public async Task SearchCameras_WithoutNameParameter_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras/search");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(content, Does.Contain("Search name parameter is required"));
    }

    [Test]
    public async Task SearchCameras_WithEmptyName_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras/search?name=");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(content, Does.Contain("Search name parameter is required"));
    }

    [Test]
    public async Task SearchCameras_CaseInsensitive_ReturnsMatchingCameras()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras/search?name=neude");
        var content = await response.Content.ReadAsStringAsync();
        var cameras = JsonSerializer.Deserialize<Camera[]>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(cameras, Is.Not.Null);
        Assert.That(cameras.Length, Is.EqualTo(2));
    }

    #endregion

    #region GET /api/cameras/{number} Tests

    [Test]
    public async Task GetCameraByNumber_WithExistingCamera_ReturnsCamera()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras/501");
        var content = await response.Content.ReadAsStringAsync();
        var camera = JsonSerializer.Deserialize<Camera>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(camera, Is.Not.Null);
        Assert.That(camera.Number, Is.EqualTo(501));
        Assert.That(camera.Name, Is.EqualTo("UTR-CM-501 Neude rijbaan voor Postkantoor"));
    }

    [Test]
    public async Task GetCameraByNumber_WithNonExistentCamera_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras/9999");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(content, Does.Contain("Camera with number 9999 not found"));
    }

    [Test]
    public async Task GetCameraByNumber_WithInvalidNumber_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/cameras/0");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #endregion

    #region CORS Tests

    [Test]
    public async Task GetAllCameras_HasCorsHeaders()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Origin", "http://localhost:3000");

        // Act
        var response = await _client.GetAsync("/api/cameras");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True);
        
        var corsHeader = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
        Assert.That(corsHeader, Is.EqualTo("*"));
    }

    #endregion

    #region Response Format Tests

    [Test]
    public async Task AllEndpoints_ReturnValidJsonFormat()
    {
        // Test GetAll
        var getAllResponse = await _client.GetAsync("/api/cameras");
        Assert.That(getAllResponse.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
        
        var getAllContent = await getAllResponse.Content.ReadAsStringAsync();
        Assert.DoesNotThrow(() => JsonSerializer.Deserialize<Camera[]>(getAllContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }));

        // Test Search
        var searchResponse = await _client.GetAsync("/api/cameras/search?name=Neude");
        Assert.That(searchResponse.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
        
        var searchContent = await searchResponse.Content.ReadAsStringAsync();
        Assert.DoesNotThrow(() => JsonSerializer.Deserialize<Camera[]>(searchContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }));

        // Test GetByNumber
        var getByNumberResponse = await _client.GetAsync("/api/cameras/501");
        Assert.That(getByNumberResponse.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
        
        var getByNumberContent = await getByNumberResponse.Content.ReadAsStringAsync();
        Assert.DoesNotThrow(() => JsonSerializer.Deserialize<Camera>(getByNumberContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }));
    }

    #endregion

    private void CreateTestCsv(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Camera;Latitude;Longitude");
        writer.WriteLine("UTR-CM-501 Neude rijbaan voor Postkantoor;52.093421;5.118278");
        writer.WriteLine("UTR-CM-502 Potterstraat / Loeff Berchmakerstraat;52.093599;5.118325");
        writer.WriteLine("UTR-CM-503 Neude plein;52.093448;5.118536");
    }
}
