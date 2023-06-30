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
    });
});

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

// necessary public files for ChatGPT to get plugin logo
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "pluginInfo")),
    RequestPath = "/pluginInfo"
});

// publish the plugin manifest information, update the host with the current one
app.MapGet("/.well-known/ai-plugin.json", (HttpRequest request) =>
{
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
app.MapGet("/pets", () =>
{
    var petsFile = File.ReadAllText("data/pets.json");
    return Results.Json(petsFile);
})
.WithName("Pets")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Gets the list of pets available in El Bruno's Pet catalog.";
    return generatedOperation;
});

// add a new pet
app.MapPut("/petadd", (string name, string type, string breed, int age, string color, int weight, string owner_name, string owner_email, string owner_phone) =>
{
    var petsFile = File.ReadAllText("data/pets.json");

    // deserialize the json file using root class
    var pets = JsonSerializer.Deserialize<List<Root>>(petsFile);

    // create a new pet object
    var newPet = new Root
    {
        name = name,
        type = type,
        breed = breed,
        age = age,
        color = color,
        weight = weight,
        owner = new Owner
        {
            name = owner_name,
            email = owner_email,
            phone = owner_phone
        }
    };

    // add the pet to the list
    pets.Add(newPet);

    // save the list back to the json file
    petsFile = JsonSerializer.Serialize(pets);
    File.WriteAllText("data/pets.json", petsFile);

    // return sucessful operation
    return Results.Ok();
})
.WithName("PetAdd")
.WithOpenApi(generatedOperation =>
{

    // generate operation parameters based on (string name, string type, string breed, int age, string color, int weight, string owner_name, string owner_email, string owner_phone)

    var parameter = generatedOperation.Parameters[0];
    parameter.Description = "The name of the Pet";
    parameter = generatedOperation.Parameters[1];
    parameter.Description = "The type of the Pet";
    parameter = generatedOperation.Parameters[2];
    parameter.Description = "The breed of the Pet";
    parameter = generatedOperation.Parameters[3];
    parameter.Description = "The age of the Pet";
    parameter = generatedOperation.Parameters[4];
    parameter.Description = "The color of the Pet";
    parameter = generatedOperation.Parameters[5];
    parameter.Description = "The weight of the Pet";
    parameter = generatedOperation.Parameters[6];
    parameter.Description = "The Pet's owner name";
    parameter = generatedOperation.Parameters[7];
    parameter.Description = "The Pet's owner email";
    parameter = generatedOperation.Parameters[8];
    parameter.Description = "The Pet's owner phone";

    generatedOperation.Description = "Add a new pet to the pet catalog available in El Bruno's Pet catalog.";
    return generatedOperation;
});

app.Run();

public class Owner
{
    public string name { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
}

public class Root
{
    public string name { get; set; }
    public string type { get; set; }
    public string breed { get; set; }
    public int age { get; set; }
    public string color { get; set; }
    public int weight { get; set; }
    public Owner owner { get; set; }
}