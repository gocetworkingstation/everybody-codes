using CameraAssignment.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CameraAssignment.Core.Configuration;


public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureCameraServices(this IServiceCollection services, string csvFilePath)
    {
        services.AddSingleton<ICameraRepository>(new CameraRepository(csvFilePath));
        services.AddSingleton<ICameraService, CameraService>();
        return services;
    }
}