using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Services;
using TATA.GestiondeTalentoMoviles.CORE.Core.Settings;
using TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.Services;

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
builder.Services.AddTransient<IMongoDatabase>(s =>
{
    var settings = s.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = s.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// --- FIN DE CONFIGURACIÓN DE MONGODB ---

// --- INICIO DE CONFIGURACIÓN DE JWT ---

// Configurar autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtAudience = builder.Configuration["Jwt:Audience"];

    if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
    {
        throw new InvalidOperationException("La configuración de JWT no está completa en appsettings.json");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Elimina el tiempo de gracia por defecto de 5 minutos
    };
});

builder.Services.AddAuthorization();

// --- FIN DE CONFIGURACIÓN DE JWT ---

// 4. Registrar servicios y repositorios
// Colaboradores
builder.Services.AddTransient<IColaboradorService, ColaboradorService>();
builder.Services.AddTransient<IColaboradorRepository, ColaboradorRepository>();

// Roles
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();

// Autenticación (Auth)
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

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

// ¡IMPORTANTE! UseAuthentication debe ir ANTES de UseAuthorization
app.UseAuthentication();
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