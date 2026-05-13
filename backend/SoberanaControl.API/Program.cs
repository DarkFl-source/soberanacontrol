using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SoberanaControl.Infrastructure.Data.ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registros de Serviços e Casos de Uso
builder.Services.AddScoped<SoberanaControl.Application.Interfaces.IApplicationDbContext>(provider => provider.GetRequiredService<SoberanaControl.Infrastructure.Data.ApplicationDbContext>());
builder.Services.AddScoped<SoberanaControl.Application.Services.NFeParserService>();
builder.Services.AddScoped<SoberanaControl.Application.UseCases.ImportarNfeUseCase>();

var app = builder.Build();

// Auto-Apply Database Migrations (útil para nuvem como o Render)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<SoberanaControl.Infrastructure.Data.ApplicationDbContext>();
        
        // Retry logic for cloud environments where DB might still be booting up
        var retryCount = 5;
        while (retryCount > 0)
        {
            try
            {
                dbContext.Database.Migrate();
                break;
            }
            catch (Exception)
            {
                retryCount--;
                if (retryCount == 0) throw;
                System.Threading.Thread.Sleep(3000); // Wait 3 seconds before retry
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro durante a migração do banco de dados.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
