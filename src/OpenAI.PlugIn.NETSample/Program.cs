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
        policy.WithOrigins("https://chat.openai.com", "http://localhost:5100").AllowAnyHeader().AllowAnyMethod();
        policy.WithOrigins("http://localhost:3000", "http://localhost:5100").AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "El Bruno Pet Store",
        Version = "v1",
        Description = "View El Bruno's Pets store information.",
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
        Url = "http://localhost:5100"
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
    options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "ElBruno.Pets v1");
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

// return the list of pets
app.MapGet("/GetAllPets", () =>
{
    Console.WriteLine("GET ALL PETS");
    var petsFile = File.ReadAllText("data/pets.json");
    return Results.Json(petsFile);
})
.WithName("GetAllPets")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Gets the list of pets available in El Bruno's Pet catalog.";
    return generatedOperation;
});

// add a new pet
app.MapPost("/AddPet", async (Root newPet) =>
{
    Console.WriteLine($"ADDPET / PET info: {newPet.name}");
    var petsFile = File.ReadAllText("data/pets.json");
    var pets = JsonSerializer.Deserialize<List<Root>>(petsFile);
    pets.Add(newPet);
    petsFile = JsonSerializer.Serialize(pets);
    File.WriteAllText("data/pets.json", petsFile);
    return Results.Ok();
})
.WithName("AddPet")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Add a new pet to the pet catalog available in El Bruno's Pet catalog. Requires pet and pet's owner information.";
    return generatedOperation;
});

app.Run();


/// <summary>
/// Pet Owner information
/// </summary>
public class Owner
{
    /// <summary>
    /// Owner name
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// Owner email
    /// </summary>
    public string email { get; set; }
    /// <summary>
    /// Owner phone
    /// </summary>
    public string phone { get; set; }
}

/// <summary>
/// Pet information
/// </summary>
public class Root
{
    /// <summary>
    /// Pet name
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// Pet type
    /// </summary>
    public string type { get; set; }
    /// <summary>
    /// Pet breed
    /// </summary>
    public string breed { get; set; }
    /// <summary>
    /// Pet age
    /// </summary>
    public int age { get; set; }
    /// <summary>
    /// Pet color
    /// </summary>
    public string color { get; set; }
    /// <summary>
    /// Pet weight
    /// </summary>
    public int weight { get; set; }
    /// <summary>
    /// Pet owner
    /// </summary>
    public Owner owner { get; set; }
}