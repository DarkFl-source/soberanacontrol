using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Serviços da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// CORS - permite chamadas do Angular local e do Render
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://localhost:4201",
                "https://soberana-frontend.onrender.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Banco de dados
builder.Services.AddDbContext<SoberanaControl.Infrastructure.Data.ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Serviços e Casos de Uso
builder.Services.AddScoped<SoberanaControl.Application.Interfaces.IApplicationDbContext>(
    provider => provider.GetRequiredService<SoberanaControl.Infrastructure.Data.ApplicationDbContext>());
builder.Services.AddScoped<SoberanaControl.Application.Services.NFeParserService>();
builder.Services.AddScoped<SoberanaControl.Application.UseCases.ImportarNfeUseCase>();
builder.Services.AddScoped<SoberanaControl.Application.UseCases.RegistrarMovimentacaoUseCase>();

var app = builder.Build();

// Auto-Apply Migrations + Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<SoberanaControl.Infrastructure.Data.ApplicationDbContext>();
        var retryCount = 5;
        while (retryCount > 0)
        {
            try
            {
                dbContext.Database.Migrate();
                SoberanaControl.Infrastructure.Data.DbSeeder.SeedAsync(dbContext).Wait();
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATAL ERROR] Falha na migração do banco: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                retryCount--;
                if (retryCount == 0) throw;
                System.Threading.Thread.Sleep(3000);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro durante a migração ou seed do banco de dados.");
    }
}

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilita Swagger e Erros detalhados em produção para depuração
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    Console.WriteLine("[DEBUG] Rodando em modo Produção");
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
