using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Reflection;
using System.Text;
using TATA.GestiondeTalentoMoviles.API.Middleware;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories;
using TATA.GestiondeTalentoMoviles.CORE.Core.Services;
using TATA.GestiondeTalentoMoviles.CORE.Core.Settings;
using TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories;
using TATA.GestiondeTalentoMoviles.CORE.Services;
using Microsoft.OpenApi.Models;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using TATA.GestiondeTalentoMoviles.INFRASTRUCTURE.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register camelCase convention
var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", conventionPack, t => true);

// --- CONFIGURACIÓN DE MONGODB ---

// 1. Registrar la clase 'MongoDbSettings' para que se enlace con "MongoDbSettings" de appsettings.json
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

// 2. Registrar la INTERFAZ 'IMongoDbSettings' para que use la clase registrada arriba
builder.Services.AddSingleton<IMongoDbSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

// 3. Registrar el cliente de MongoDB como Singleton (una sola conexión)
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(s.GetRequiredService<IMongoDbSettings>().ConnectionString)
);

// 4. Registrar la base de datos (IMongoDatabase) como Scoped (una por petición HTTP)
builder.Services.AddScoped<IMongoDatabase>(s =>
{
    var settings = s.GetRequiredService<IMongoDbSettings>();
    var client = s.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// --- CONFIGURACIÓN DE JWT ---
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
        throw new InvalidOperationException("La configuración de JWT (Key, Issuer, Audience) no está completa en appsettings.json");
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
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// --- REGISTRAR SERVICIOS Y REPOSITORIOS ---

// Autenticación
builder.Services.AddTransient<IAuthService, AuthService>();

// Usuarios
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

// Colaboradores
builder.Services.AddScoped<IColaboradorService, ColaboradorService>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();

// Solicitudes
builder.Services.AddScoped<ISolicitudesRepository, SolicitudesRepository>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();


// Alertas
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IAlertaService, AlertaService>();

// Procesos Matching
builder.Services.AddTransient<IProcesosMatchingService, ProcesosMatchingService>();
builder.Services.AddTransient<IProcesosMatchingRepository, ProcesosMatchingRepository>();

// Catalogo (nuevo)
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
// Vacantes 
builder.Services.AddScoped<IVacanteRepository, VacanteRepository>();
builder.Services.AddScoped<IVacanteService, VacanteService>();

// Evaluaciones
builder.Services.AddScoped<IEvaluacionRepository, EvaluacionesRepository>();
builder.Services.AddScoped<IEvaluacionService, EvaluacionService>();

// Evaluaciones II (Plantillas)
builder.Services.AddScoped<IEvaluacionesIIRepository, EvaluacionesIIRepository>();
builder.Services.AddScoped<IEvaluacionesIIService, EvaluacionesIIService>();

// Plantilla Evaluación
builder.Services.AddScoped<IPlantillaEvaluacionService, PlantillaEvaluacionService>();

// --- FIN DE REGISTRO DE SERVICIOS ---

// Configurar Controllers con validación de modelos
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
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
                success = false,
                message = "Error de validación",
                errors = errors
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to support JWT Bearer authentication (HTTP bearer)
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese el token JWT con el prefijo 'Bearer '. Ejemplo: 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- MANEJO GLOBAL DE EXCEPCIONES ---
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Habilitar CORS
app.UseCors("AllowMobileApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// ¡IMPORTANTE! El orden es crucial
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.WriteLine("🚀 Aplicación iniciada correctamente.");
app.Run();