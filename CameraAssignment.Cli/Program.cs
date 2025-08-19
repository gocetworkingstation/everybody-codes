using System.CommandLine;
using CameraAssignment.Core.Configuration;
using CameraAssignment.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

// Create the root command with description
var rootCommand = new RootCommand("Camera Assignment CLI - Search through camera data");

// Create the search command
var searchCommand = new Command("Search", "Search for cameras by name");

// Create the --name option
var nameOption = new Option<string>(
    name: "--name",
    description: "Search for cameras containing this name")
{
    IsRequired = true
};

searchCommand.AddOption(nameOption);

// Set the handler for the search command
searchCommand.SetHandler((string name) =>
{
    try
    {
        // Get the CSV file path (relative to the solution root)
        var solutionRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName
                          ?? throw new DirectoryNotFoundException("Could not find solution root directory");
        var csvFilePath = Path.Combine(solutionRoot, "data", "cameras-defb.csv");

        // Set up dependency injection
        var services = new ServiceCollection();
        services.ConfigureCameraServices(csvFilePath);
        var serviceProvider = services.BuildServiceProvider();

        // Get the camera service
        var cameraService = serviceProvider.GetRequiredService<ICameraService>();

        // Search for cameras
        var cameras = cameraService.Search(name);

        // Display results in the required format
        foreach (var camera in cameras)
        {
            Console.WriteLine($"{camera.Number} | {camera.Name} | {camera.Latitude} | {camera.Longitude}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Environment.Exit(1);
    }
}, nameOption);

// Add the search command to root command
rootCommand.AddCommand(searchCommand);

// Execute the command
return rootCommand.Invoke(args);