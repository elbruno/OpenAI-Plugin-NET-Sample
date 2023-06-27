using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Dynamic;
using System.Net.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalHost", policy =>
    {
        policy.WithOrigins("https://chat.openai.com", "http://localhost:5100").AllowAnyHeader().AllowAnyMethod();
    });
    options.AddPolicy("AllowCodespaces", policy =>
    {
        policy.WithOrigins("https://chat.openai.com", "http://localhost:5100").AllowAnyHeader().AllowAnyMethod();
    });
});

// log the current url
Console.WriteLine("Current Url: " + builder.Environment.WebRootPath);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "El Bruno Pet Search",
        Version = "v1",
        Description = "Search through El Bruno's wide range of pets."
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

// need to serialize this as V2 to work as ChatGPT API plugin
app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "ElBruno.Pets v1");
});

// necessary public files for ChatGPT to get plugin information
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), ".well-known")),
    RequestPath = "/.well-known"
});

app.MapGet("/pets", () =>
{
    using var httpClient = new HttpClient();
    var petsFile = File.ReadAllText("data/pets.json");
    return Results.Json(petsFile);
})
.WithName("Pets")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Gets the list of pets available in El Bruno's Pet catalog.";
    return generatedOperation;
});

app.Run();