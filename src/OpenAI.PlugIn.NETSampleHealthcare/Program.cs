using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Dynamic;
using System.Net.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://chat.openai.com", "http://localhost:5178").AllowAnyHeader().AllowAnyMethod();
        policy.WithOrigins("http://localhost:3000", "http://localhost:5178").AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "El Bruno Doctors' Appointments API",
        Version = "v1",
        Description = "View El Bruno's Doctors' Appointments information.",
        Contact = new OpenApiContact
        {
            Name = "El Bruno",
            Email = "elbruno@elbruno.com",
            Url = new Uri("https://elbruno.com/")
        }
    });
    // add servers to swagger, this is needed if testing with the Semantic Kernel Console App
    options.AddServer(new OpenApiServer
    {
        Url = "http://localhost:5178"
    });
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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
    options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "ElBruno.doctors v1");
});

// necessary public files for ChatGPT to get plugin logo
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "pluginInfo")),
    RequestPath = "/pluginInfo"
});

// publish the plugin manifest information, update the host with the current one
app.MapGet("/.well-known/ai-plugin.json", (HttpRequest request) =>
{
    Console.WriteLine($"GET PLUGIN MANIFEST");
    // get current url from request headers (codespaces dev) or app Urls (local dev)
    var userAgent = request.Headers.UserAgent;
    var customHeader = request.Headers["x-custom-header"];
    var currentUrl = request.Headers["x-forwarded-proto"] + "://" + request.Headers["x-forwarded-host"];
    if (currentUrl == "://")
        currentUrl = app.Urls.First();

    // update current host in manifest
    string aiPlugInManifest = File.ReadAllText("pluginInfo/ai-plugin.json");
    aiPlugInManifest = aiPlugInManifest.Replace("$host", currentUrl);
    return Results.Text(aiPlugInManifest);
})
.ExcludeFromDescription(); // exclude from swagger description;

// return the list of doctors
app.MapGet("/GetAlldoctors", () =>
{
    Console.WriteLine("GET ALL DOCTORS");
    var doctorsFile = File.ReadAllText("data/doctors.json");
    return Results.Json(doctorsFile);
})
.WithName("GetAlldoctors")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Gets the list of doctors available in El Bruno's Doctors' Appointments catalog.";
    return generatedOperation;
});


app.Run();