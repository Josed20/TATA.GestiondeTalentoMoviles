# Módulo de Autenticación JWT + Refresh Token

## ?? Descripción

Módulo completo de autenticación para **TATA Gestión de Talento Móviles** que implementa:
- ? Registro de usuarios con contraseñas hasheadas (BCrypt)
- ? Login con JWT (JSON Web Tokens)
- ? Refresh Tokens para renovar la sesión
- ? RBAC (Role-Based Access Control) preparado
- ? Validación de tokens en cada request
- ? Épico S1 completado: JWT + Refresh + RBAC

---

## ??? Arquitectura Implementada

### Entidades Creadas
1. **`User.cs`** - Entidad de usuario con campos de autenticación
2. **`Role.cs`** - Entidad de roles para RBAC

### DTOs Creados
3. **`AuthDtos.cs`** contiene:
   - `RegisterRequestDto` - Para registro
   - `LoginRequestDto` - Para login
   - `RefreshTokenRequestDto` - Para refresh token
   - `AuthResponseDto` - Respuesta de autenticación
   - `UserResponseDto` - Datos del usuario autenticado

### Repositorios
4. **`IUserRepository.cs`** - Interfaz del repositorio
5. **`UserRepository.cs`** - Implementación con MongoDB

### Servicios
6. **`IAuthService.cs`** - Interfaz del servicio
7. **`AuthService.cs`** - Lógica de negocio completa

### Controladores
8. **`AuthController.cs`** - Endpoints REST

### Configuración
9. **`Program.cs`** - Configuración de JWT Bearer
10. **`appsettings.json`** - Configuración de JWT

---

## ?? Seguridad Implementada

### Hashing de Contraseñas
- ? **BCrypt.Net-Next** para hashear contraseñas
- ? Las contraseñas NUNCA se guardan en texto plano
- ? Verificación segura en el login

### JWT Tokens
- ? **Access Token**: Válido por 1 hora
- ? **Refresh Token**: Válido por 7 días
- ? Token firmado con clave secreta (HMAC SHA256)
- ? Claims incluidos: ID, Email, Nombre, Apellido, Roles

### Validación
- ? Validación de Issuer, Audience y Lifetime
- ? ClockSkew = 0 (sin tiempo de gracia)

---

## ?? Endpoints Disponibles

### 1. Registro de Usuario
```http
POST /api/auth/register
Content-Type: application/json

{
  "nombre": "Juan",
  "apellido": "Pérez",
  "email": "juan.perez@example.com",
  "password": "MiContraseñaSegura123"
}
```

**Respuesta Exitosa (200 OK):**
```json
{
  "success": true,
  "message": "Usuario registrado exitosamente",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "Qx5Z3mK8pL2vN9tR...",
    "tokenExpires": "2025-01-15T15:30:00Z",
    "user": {
      "id": "67a1b2c3d4e5f6g7h8i9j0k1",
      "nombreCompleto": "Juan Pérez",
      "email": "juan.perez@example.com",
      "roles": []
    }
  }
}
```

**Error: Email ya existe (409 Conflict):**
```json
{
  "success": false,
  "message": "El email ya está registrado"
}
```

---

### 2. Login de Usuario
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "juan.perez@example.com",
  "password": "MiContraseñaSegura123"
}
```

**Respuesta Exitosa (200 OK):**
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "Qx5Z3mK8pL2vN9tR...",
    "tokenExpires": "2025-01-15T15:30:00Z",
    "user": {
      "id": "67a1b2c3d4e5f6g7h8i9j0k1",
      "nombreCompleto": "Juan Pérez",
      "email": "juan.perez@example.com",
      "roles": []
    }
  }
}
```

**Error: Credenciales incorrectas (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Credenciales incorrectas"
}
```

---

### 3. Refrescar Token
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "Qx5Z3mK8pL2vN9tR..."
}
```

**Respuesta Exitosa (200 OK):**
```json
{
  "success": true,
  "message": "Token refrescado exitosamente",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "nEwT0k3nF0rR3fR3sh...",
    "tokenExpires": "2025-01-15T16:30:00Z",
    "user": {
      "id": "67a1b2c3d4e5f6g7h8i9j0k1",
      "nombreCompleto": "Juan Pérez",
      "email": "juan.perez@example.com",
      "roles": []
    }
  }
}
```

---

## ?? Configuración JWT

### En `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "TuClaveSecretaSuperSeguraDeAlMenos32CaracteresParaJWT123456",
    "Issuer": "TATA.GestiondeTalentoMoviles.API",
    "Audience": "TATA.GestiondeTalentoMoviles.Client",
    "ExpirationInHours": 1,
    "RefreshTokenExpirationInDays": 7
  }
}
```

?? **IMPORTANTE**: En producción, usa una clave secreta **diferente y más segura**. Guárdala en Azure Key Vault o variables de entorno.

---

## ??? Cómo Proteger Endpoints

Para proteger un endpoint y requerir autenticación:

```csharp
[Authorize] // Requiere token válido
[HttpGet("protected-resource")]
public IActionResult GetProtectedResource()
{
    // Solo usuarios autenticados pueden acceder
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Ok(new { message = "Acceso autorizado", userId });
}
```

Para requerir un rol específico:

```csharp
[Authorize(Roles = "Admin")] // Solo admins
[HttpDelete("delete-user/{id}")]
public IActionResult DeleteUser(string id)
{
    return Ok(new { message = "Usuario eliminado" });
}
```

---

## ?? Pruebas con Postman

### 1. Registrar un usuario
1. Crear request POST a `https://localhost:7XXX/api/auth/register`
2. Body ? raw ? JSON
3. Copiar el `token` de la respuesta

### 2. Usar el token en otros endpoints
1. En Headers, agregar:
   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

### 3. Refrescar el token (cuando expire)
1. POST a `/api/auth/refresh`
2. Enviar el `refreshToken` en el body

---

## ?? Paquetes NuGet Instalados

### En `TATA.GestiondeTalentoMoviles.CORE`:
- ? `BCrypt.Net-Next` v4.0.3
- ? `System.IdentityModel.Tokens.Jwt` v8.14.0
- ? `Microsoft.Extensions.Configuration.Abstractions` v10.0.0

### En `TATA.GestiondeTalentoMoviles.API`:
- ? `Microsoft.AspNetCore.Authentication.JwtBearer` v9.0.0
- ? `Microsoft.IdentityModel.Tokens` v8.14.0

---

## ??? Estructura de MongoDB

### Colección: `users`
```json
{
  "_id": "67a1b2c3d4e5f6g7h8i9j0k1",
  "nombre": "Juan",
  "apellido": "Pérez",
  "email": "juan.perez@example.com",
  "password": "$2a$11$K2hO3mL7pQ9sR8tU5vW6xY...",
  "roles": [],
  "estado": 1,
  "refreshToken": "Qx5Z3mK8pL2vN9tR...",
  "refreshTokenExpiryTime": "2025-01-22T14:30:00Z",
  "createdAt": "2025-01-15T14:30:00Z",
  "updatedAt": "2025-01-15T14:30:00Z"
}
```

---

## ?? Próximos Pasos (RBAC)

### 1. Crear Roles en MongoDB
```json
// Colección: roles
{
  "_id": "role_admin_id",
  "nombre": "Admin",
  "createdAt": "2025-01-15T14:30:00Z"
}
```

### 2. Asignar Roles a Usuarios
```json
{
  "_id": "user_id",
  "roles": ["role_admin_id", "role_manager_id"]
}
```

### 3. Crear un Servicio de Roles
- `IRoleService.cs`
- `RoleService.cs`
- `RoleRepository.cs`

### 4. Endpoint para asignar roles
```csharp
[Authorize(Roles = "Admin")]
[HttpPost("users/{userId}/assign-role")]
public async Task<IActionResult> AssignRole(string userId, string roleId)
{
    // Lógica para asignar rol
}
```

---

## ? Checklist de Implementación

- [x] Entidad `User` con campos de autenticación
- [x] Entidad `Role` para RBAC
- [x] DTOs de autenticación
- [x] Repositorio `UserRepository`
- [x] Servicio `AuthService` con lógica completa
- [x] Controlador `AuthController` con 3 endpoints
- [x] Hashing de contraseñas con BCrypt
- [x] Generación de JWT tokens
- [x] Generación de Refresh tokens
- [x] Configuración JWT en `Program.cs`
- [x] Configuración en `appsettings.json`
- [x] Validación de tokens en cada request
- [x] Manejo de errores (Unauthorized, Conflict, BadRequest)

---

## ?? Notas de Seguridad

1. **Cambiar la clave JWT en producción**
   - Usa una clave de al menos 256 bits
   - Guárdala en Azure Key Vault o variables de entorno

2. **HTTPS obligatorio**
   - Ya está configurado con `app.UseHttpsRedirection()`

3. **Validar entrada de usuario**
   - Los DTOs tienen `[Required]`, `[EmailAddress]`, `[MinLength]`

4. **Rate Limiting** (Próxima mejora)
   - Limitar intentos de login fallidos
   - Prevenir ataques de fuerza bruta

5. **Refresh Token Rotation**
   - Ya implementado: cada refresh genera un nuevo token

---

## ?? Soporte

Si tienes dudas sobre la implementación:
1. Revisa este README
2. Verifica la configuración en `appsettings.json`
3. Revisa los logs en la consola

---

**¡Módulo de Autenticación Completado! ??**

Épico S1: **JWT + Refresh Token + RBAC** ?
