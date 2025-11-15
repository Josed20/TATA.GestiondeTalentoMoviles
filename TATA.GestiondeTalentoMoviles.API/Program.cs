using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Conventions;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.Services;
using TATA.GestiondeTalentoMoviles.CORE.Core.Settings;
using TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using System.Reflection;
using System.Text;
using TATA.GestiondeTalentoMoviles.CORE.Services;
using TATA.GestiondeTalentoMoviles.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Register camelCase convention so BSON keys like 'nombre' map to C# properties 'Nombre'
var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", conventionPack, t => true);

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

// --- REGISTRAR SERVICIOS Y REPOSITORIOS ---

// Colaboradores
builder.Services.AddScoped<IColaboradorService, ColaboradorService>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();

// Evaluaciones
builder.Services.AddScoped<IEvaluacionService, EvaluacionService>();
builder.Services.AddScoped<IEvaluacionRepository, EvaluacionRepository>();

// Solicitudes
builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();

// Recomendaciones
builder.Services.AddScoped<IRecomendacionRepository, RecomendacionRepository>();
builder.Services.AddScoped<IRecomendacionService, RecomendacionService>();

// Skills
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();

// NivelSkills
builder.Services.AddScoped<INivelSkillService, NivelSkillService>();
builder.Services.AddScoped<INivelSkillRepository, NivelSkillRepository>();

// Roles
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Autenticación (Auth)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Vacantes
builder.Services.AddScoped<IVacanteService, VacanteService>();
builder.Services.AddScoped<IVacanteRepository, VacanteRepository>();

// Areas
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();

// --- FIN DE REGISTRO DE SERVICIOS ---

// Configurar Controllers con validación de modelos
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Personalizar respuesta de validación de modelos
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Errors = e.Value.Errors.Select(x => x.ErrorMessage).ToArray()
                }).ToArray();

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
            {
                Message = "Error de validación",
                Errors = errors
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS para aplicaciones móviles
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Construir la aplicación
WebApplication app = builder.Build();

// --- INICIO DE MANEJO GLOBAL DE EXCEPCIONES ---
// Usar el middleware personalizado de manejo de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();
// --- FIN DE MANEJO GLOBAL DE EXCEPCIONES ---

// Habilitar CORS
app.UseCors("AllowMobileApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// ¡IMPORTANTE! UseAuthentication debe ir ANTES de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// --- INICIO DEL BLOQUE DE DEBUG ---
// El error ReflectionTypeLoadException sucede aquí, cuando .NET
// intenta escanear todos tus 'Controllers'.
try
{
    app.MapControllers();
    Console.WriteLine("✅ Todos los controladores se mapearon correctamente.");
}
catch (ReflectionTypeLoadException ex)
{
    Console.WriteLine("!!! ERROR DE CARGA DE REFLEXIÓN !!!");
    foreach (var loaderEx in ex.LoaderExceptions)
    {
        // Esto te dirá qué DLL o paquete NuGet está causando el conflicto
        if (loaderEx != null)
        {
            Console.WriteLine($"❌ {loaderEx.Message}");
        }
    }
    // Lanza la excepción principal para detener la app
    throw;
}
// --- FIN DEL BLOQUE DE DEBUG ---

Console.WriteLine("🚀 Aplicación iniciada correctamente.");
app.Run();