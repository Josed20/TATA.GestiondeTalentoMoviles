using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces; // usar el namespace correcto de Interfaces
using TATA.GestiondeTalentoMoviles.CORE.Services;   // usar el namespace correcto de Services
using TATA.GestiondeTalentoMoviles.CORE.Core.Settings; // Este ya estaba bien
using TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories;
using System.Reflection; // Este ya estaba bien

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


// Servicios existentes de la plantilla
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build(); // El error debería desaparecer de aquí

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

try
{
    app.MapControllers();
}
catch (ReflectionTypeLoadException ex)
{
    foreach (var le in ex.LoaderExceptions) Console.WriteLine(le);
    throw;
}

app.Run();