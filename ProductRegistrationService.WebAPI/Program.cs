using ProductRegistrationService.Infra.IoC;
using Swashbuckle.AspNetCore.SwaggerUI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInfrastructureJWT(builder.Configuration);
builder.Services.AddInfrastructureSwagger(builder.Configuration);
builder.Services.AddInfrastructureSerilog(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog();

// It needs to be the last one. Just use it after database update.
if (Convert.ToBoolean(builder.Configuration["Seed:Apply"]))
{
    builder.Services.AddSeed(builder.Configuration);
}

var app = builder.Build();

string version = builder.Configuration["Swagger:Version"];
string definition = builder.Configuration["Swagger:Title"];

if (app.Environment.IsDevelopment())
{
    if (Convert.ToBoolean(builder.Configuration["Swagger:Debugging"]))
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            x => x.ConfigObject.Urls = new[] { new UrlDescriptor { Name = $"{definition} {version}", Url = $"{version}/swagger.json" } }
        );
    }
}
else
{
    if (Convert.ToBoolean(builder.Configuration["Swagger:Production"]))
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            x => x.ConfigObject.Urls = new[] { new UrlDescriptor { Name = $"{definition} {version}", Url = $"{version}/swagger.json" } }
        );
    }
}

app.UseHttpsRedirection();

app.UseStatusCodePages();

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();