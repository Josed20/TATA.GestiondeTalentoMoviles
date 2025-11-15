# ?? CONFIGURACIÓN DE SEGURIDAD PARA PRODUCCIÓN

## ?? IMPORTANTE: ANTES DE DESPLEGAR A PRODUCCIÓN

Este archivo contiene recomendaciones críticas de seguridad que DEBES implementar antes de desplegar tu aplicación a producción.

---

## ?? 1. CLAVE JWT SEGURA

### ? NO HACER (Desarrollo)
```json
{
  "Jwt": {
    "Key": "TuClaveSecretaSuperSeguraDeAlMenos32CaracteresParaJWT123456"
  }
}
```

### ? HACER (Producción)

#### Opción A: Variables de Entorno
```bash
# Linux/Mac
export JWT_KEY="tu_clave_super_segura_generada_aleatoriamente_de_al_menos_256_bits"

# Windows PowerShell
$env:JWT_KEY="tu_clave_super_segura_generada_aleatoriamente_de_al_menos_256_bits"

# Docker
docker run -e JWT_KEY="..." myapp
```

En `appsettings.Production.json`:
```json
{
  "Jwt": {
    "Key": ""  // ?? Vacío, se lee desde variable de entorno
  }
}
```

En `Program.cs`:
```csharp
var jwtKey = builder.Configuration["JWT_KEY"] 
    ?? builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key no configurada");
```

#### Opción B: Azure Key Vault (Recomendado para Azure)
```bash
# Instalar paquete
dotnet add package Azure.Identity
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
```

En `Program.cs`:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Agregar Azure Key Vault
var keyVaultEndpoint = builder.Configuration["AzureKeyVault:Endpoint"];
if (!string.IsNullOrEmpty(keyVaultEndpoint))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint),
        new DefaultAzureCredential()
    );
}
```

En Azure Key Vault:
```
Secret Name: JwtKey
Secret Value: tu_clave_super_segura_256_bits
```

#### Generar Clave Segura
```bash
# Linux/Mac
openssl rand -base64 64

# PowerShell
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})

# Python
python -c "import secrets; print(secrets.token_urlsafe(64))"
```

---

## ?? 2. CORS (Cross-Origin Resource Sharing)

### ? NO HACER (Permite TODOS los orígenes)
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

### ? HACER (Orígenes específicos)
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", builder =>
    {
        builder.WithOrigins(
                "https://tu-frontend.com",
                "https://www.tu-frontend.com"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Si usas cookies
    });
});

app.UseCors("Production");
```

### Para múltiples entornos:
```csharp
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

`appsettings.Production.json`:
```json
{
  "AllowedOrigins": [
    "https://tu-frontend.com",
    "https://www.tu-frontend.com"
  ]
}
```

---

## ?? 3. RATE LIMITING (Prevenir ataques de fuerza bruta)

### Instalar paquete
```bash
dotnet add package AspNetCoreRateLimit
```

### Configuración en `Program.cs`
```csharp
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Configurar Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ...resto de servicios...

var app = builder.Build();

// Middleware de Rate Limiting (ANTES de UseAuthentication)
app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();
```

### `appsettings.json`
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "POST:/api/auth/login",
        "Period": "1m",
        "Limit": 5
      },
      {
        "Endpoint": "POST:/api/auth/register",
        "Period": "1h",
        "Limit": 3
      },
      {
        "Endpoint": "*",
        "Period": "1s",
        "Limit": 10
      }
    ]
  }
}
```

---

## ?? 4. HTTPS OBLIGATORIO

### Configuración en `Program.cs`
```csharp
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // Forzar HTTPS
    app.UseHttpsRedirection();
    
    // HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}
```

### Agregar HSTS en `Program.cs` (antes de Build)
```csharp
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});
```

---

## ?? 5. LOGGING Y AUDITORÍA

### Instalar Serilog (Recomendado)
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
```

### Configuración en `Program.cs`
```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/auth-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

### Logging en AuthService
```csharp
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository, 
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        _logger.LogInformation("Intento de login para email: {Email}", dto.Email);
        
        var user = await _userRepository.GetByEmailAsync(dto.Email.ToLower());
        if (user == null)
        {
            _logger.LogWarning("Login fallido: Usuario no encontrado - {Email}", dto.Email);
            throw new UnauthorizedAccessException("Credenciales incorrectas");
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            _logger.LogWarning("Login fallido: Contraseña incorrecta - {Email}", dto.Email);
            throw new UnauthorizedAccessException("Credenciales incorrectas");
        }

        _logger.LogInformation("Login exitoso para usuario: {UserId}", user.Id);
        
        // ...resto del código...
    }
}
```

---

## ??? 6. VALIDACIÓN DE INPUT ADICIONAL

### Instalar FluentValidation
```bash
dotnet add package FluentValidation.AspNetCore
```

### Crear validador personalizado
```csharp
using FluentValidation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(email => !email.Contains("+")) // Prevenir email aliases
            .WithMessage("Formato de email no válido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Debe contener al menos una mayúscula")
            .Matches(@"[a-z]").WithMessage("Debe contener al menos una minúscula")
            .Matches(@"[0-9]").WithMessage("Debe contener al menos un número")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Debe contener al menos un carácter especial");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("Nombre solo puede contener letras");

        RuleFor(x => x.Apellido)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("Apellido solo puede contener letras");
    }
}
```

### Registrar en `Program.cs`
```csharp
using FluentValidation;

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
```

---

## ?? 7. CONFIGURACIÓN DE MONGODB SEGURA

### ? NO HACER
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:password123@localhost:27017"
  }
}
```

### ? HACER

#### Opción A: Variable de Entorno
```bash
export MONGODB_CONNECTION_STRING="mongodb+srv://..."
```

`Program.cs`:
```csharp
var mongoConnectionString = builder.Configuration["MONGODB_CONNECTION_STRING"]
    ?? builder.Configuration["MongoDbSettings:ConnectionString"]
    ?? throw new InvalidOperationException("MongoDB Connection String no configurada");
```

#### Opción B: Azure Key Vault
```
Secret Name: MongoDbConnectionString
Secret Value: mongodb+srv://...
```

### Configuración de MongoDB Atlas
1. **IP Whitelist**: Solo IPs específicas
2. **Autenticación**: Usar certificados o usuarios con permisos mínimos
3. **Encriptación**: Habilitar encriptación en tránsito (TLS/SSL)

---

## ?? 8. MONITOREO Y ALERTAS

### Application Insights (Azure)
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

`Program.cs`:
```csharp
builder.Services.AddApplicationInsightsTelemetry(
    builder.Configuration["ApplicationInsights:ConnectionString"]
);
```

### Alertas a configurar:
- ?? Intentos de login fallidos > 10 en 5 minutos
- ?? Errores 500 > 5 en 1 minuto
- ?? Latencia de API > 2 segundos
- ?? Uso de CPU > 80%
- ?? Uso de memoria > 90%

---

## ?? 9. CHECKLIST DE SEGURIDAD FINAL

Antes de desplegar:

### Configuración
- [ ] Clave JWT generada aleatoriamente (256+ bits)
- [ ] Clave JWT almacenada en Azure Key Vault o variable de entorno
- [ ] ConnectionString de MongoDB en variable de entorno
- [ ] CORS configurado con orígenes específicos
- [ ] HTTPS forzado en producción
- [ ] HSTS habilitado

### Rate Limiting
- [ ] Rate limiting en `/api/auth/login` (máx. 5 por minuto)
- [ ] Rate limiting en `/api/auth/register` (máx. 3 por hora)
- [ ] Rate limiting general en todos los endpoints

### Validación
- [ ] Contraseñas: mínimo 8 caracteres con complejidad
- [ ] Emails validados con formato correcto
- [ ] Inputs sanitizados para prevenir SQL/NoSQL injection

### Logging
- [ ] Logs de intentos de login (exitosos y fallidos)
- [ ] Logs de errores con stack traces
- [ ] Logs rotan diariamente
- [ ] Logs no contienen información sensible (contraseñas, tokens)

### Monitoreo
- [ ] Application Insights configurado
- [ ] Alertas de seguridad configuradas
- [ ] Health checks implementados
- [ ] Dashboard de métricas configurado

### Base de Datos
- [ ] MongoDB con autenticación habilitada
- [ ] IP Whitelist configurada
- [ ] Backup automático configurado
- [ ] Índices creados en campos frecuentes (email)

### Testing
- [ ] Pruebas de seguridad realizadas
- [ ] Pruebas de carga realizadas
- [ ] Penetration testing realizado
- [ ] Revisión de código de seguridad completada

---

## ?? 10. DESPLIEGUE A AZURE (Ejemplo)

### Crear App Service
```bash
az webapp create \
  --resource-group mi-grupo \
  --plan mi-plan \
  --name tata-talento-api \
  --runtime "DOTNETCORE:9.0"
```

### Configurar Variables de Entorno
```bash
az webapp config appsettings set \
  --resource-group mi-grupo \
  --name tata-talento-api \
  --settings \
    JWT_KEY="..." \
    MONGODB_CONNECTION_STRING="..." \
    ASPNETCORE_ENVIRONMENT="Production"
```

### Habilitar HTTPS Only
```bash
az webapp update \
  --resource-group mi-grupo \
  --name tata-talento-api \
  --https-only true
```

---

## ?? CONTACTO EN CASO DE INCIDENTE DE SEGURIDAD

1. **Cambiar inmediatamente**:
   - Clave JWT
   - Contraseñas de MongoDB
   - Todos los tokens de acceso

2. **Revocar tokens**:
   - Implementar blacklist de tokens
   - Forzar logout de todos los usuarios

3. **Investigar**:
   - Revisar logs de acceso
   - Identificar IPs sospechosas
   - Verificar integridad de datos

4. **Notificar**:
   - Equipo de seguridad
   - Usuarios afectados (si aplica)
   - Autoridades (si es requerido)

---

## ? RESUMEN

```
? Clave JWT segura (256+ bits, en Key Vault)
? HTTPS forzado con HSTS
? CORS configurado con orígenes específicos
? Rate limiting en endpoints de autenticación
? Logging completo con Serilog
? Validación robusta de inputs
? MongoDB con autenticación y encriptación
? Monitoreo con Application Insights
? Alertas de seguridad configuradas
? Backup automático configurado
```

---

**¡No olvides implementar estas medidas antes de producción! ??**
