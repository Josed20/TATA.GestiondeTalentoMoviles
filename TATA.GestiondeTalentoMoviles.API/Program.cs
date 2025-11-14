using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Services;
using TATA.GestiondeTalentoMoviles.CORE.Core.Settings;
using TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using System.Reflection;
using MongoDB.Bson.Serialization.Conventions;

var builder = WebApplication.CreateBuilder(args);

// Register camelCase convention so BSON keys like 'nombre' map to C# properties 'Nombre'
var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", conventionPack, t => true);

// --- INICIO DE CONFIGURACIÓN DE MONGODB ---

// 1. Cargar la configuración de appsettings.json
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

// 2. Registrar el cliente de MongoDB como Singleton (¡Esto es correcto!)
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString"))
);

// 3. Registrar la base de datos (IMongoDatabase)
// CAMBIADO A AddTransient como solicitaste
builder.Services.AddTransient<IMongoDatabase>(s =>
{
    var settings = s.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = s.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// --- FIN DE CONFIGURACIÓN DE MONGODB ---

// 4. Registrar tus servicios y repositorios
// CAMBIADO A AddTransient como solicitaste
builder.Services.AddTransient<IColaboradorService, ColaboradorService>();
builder.Services.AddTransient<IColaboradorRepository, ColaboradorRepository>();

// Registrar nuevos servicios/repositorios
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddTransient<IRolService, RolService>();
builder.Services.AddTransient<IRolRepository, RolRepository>();

builder.Services.AddTransient<IAreaService, AreaService>();
builder.Services.AddTransient<IAreaRepository, AreaRepository>();

// Servicios existentes de la plantilla
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CORRECCIÓN DEL TRY-CATCH ---
// Declaramos 'app' aquí para que sea accesible en todo el archivo
WebApplication app;

// 'builder.Build()' rara vez falla.
app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// --- INICIO DEL BLOQUE DE DEBUG ---
// El error ReflectionTypeLoadException sucede aquí, cuando .NET
// intenta escanear todos tus 'Controllers'.
try
{
    app.MapControllers();
}
catch (ReflectionTypeLoadException ex)
{
    // Esto AHORA SÍ debería imprimir el error real
    Console.WriteLine("!!! ERROR DE CARGA DE REFLEXIÓN !!!");
    foreach (var loaderEx in ex.LoaderExceptions)
    {
        // Esto te dirá qué DLL o paquete NuGet está causando el conflicto
        if (loaderEx != null)
        {
            Console.WriteLine(loaderEx.Message);
        }
    }
    // Lanza la excepción principal para detener la app
    throw;
}
// --- FIN DEL BLOQUE DE DEBUG ---

app.Run();