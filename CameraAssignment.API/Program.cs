using CameraAssignment.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure camera services
var csvFilePath = builder.Configuration.GetValue<string>("CameraData:CsvFilePath") ?? "../data/cameras-defb.csv";
var fullCsvPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, csvFilePath));
builder.Services.ConfigureCameraServices(fullCsvPath);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowWebApp");

app.MapControllers();

app.Run();

// Make Program class accessible for integration testing
public partial class Program { }