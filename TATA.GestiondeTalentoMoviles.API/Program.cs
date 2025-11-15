using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces; // usar el namespace correcto de Interfaces
using TATA.GestiondeTalentoMoviles.CORE.Services;   // usar el namespace correcto de Services
using TATA.GestiondeTalentoMoviles.CORE.Core.Settings; // Este ya estaba bien
using TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// --- INICIO DE CONFIGURACIÓN DE MONGODB ---

// 1. Cargar la configuración de appsettings.json
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

// 2. Registrar el cliente de MongoDB como Singleton
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString"))
);

// 3. Registrar la base de datos (IMongoDatabase)
builder.Services.AddScoped<IMongoDatabase>(s =>
{
    var settings = s.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = s.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// --- FIN DE CONFIGURACIÓN DE MONGODB ---

// 4. Registrar tus servicios y repositorios
builder.Services.AddScoped<IColaboradorService, ColaboradorService>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
// Registro del servicio y repositorio de Evaluacion
builder.Services.AddScoped<IEvaluacionService, EvaluacionService>();
builder.Services.AddScoped<IEvaluacionRepository, EvaluacionRepository>();

// Servicios existentes de la plantilla
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();app.Run();