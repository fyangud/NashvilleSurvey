using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using TransitSurveyAzure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Retrieve Key Vault name from configuration (e.g., appsettings.json)
var keyVaultName = builder.Configuration["KeyVault:Name"];
var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
var client = new SecretClient(new Uri($"https://{keyVaultName}vault.azure.net/"), new DefaultAzureCredential());

// Authenticate to Azure Key Vault using DefaultAzureCredential
var secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());

//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//builder.WebHost.UseUrls($"http://*:{port}");

// Fetch the connection string from Key Vault
KeyVaultSecret secret = await secretClient.GetSecretAsync("SurveyDbConnection");
string connectionString = secret.Value;
//Console.WriteLine(connectionString);

// Register the database context with the fetched connection string
builder.Services.AddDbContext<SurveyDbContext>(options =>
    options.UseSqlServer(connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,      // Number of retries
            maxRetryDelay: TimeSpan.FromSeconds(2),
            errorNumbersToAdd: null) // Specific SQL error codes (optional)
    ));

// Add services to the container.
builder.Services.AddRazorPages();

// Add swagger
builder.Services.AddControllers();
//builder.Services.AddControllersWithViews(); // if using MVC
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapRazorPages();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Create a content type provider and map the .geojson extension
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".geojson"] = "application/geo+json";

// Configure static file options with the content type provider
var staticFileOptions = new StaticFileOptions
{
    ContentTypeProvider = provider,
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
};

// Use the configured static file middleware from the wwwroot folder
app.UseStaticFiles(staticFileOptions);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// Add swagger only in development
if (app.Environment.IsDevelopment()) // Add Swagger only in development
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = "swagger"; // Swagger at root (localhost:5000/)
    });
}

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
