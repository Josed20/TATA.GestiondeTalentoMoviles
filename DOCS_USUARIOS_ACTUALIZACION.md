# Resumen de Cambios - Sistema de Usuarios

## Cambios Realizados

Se ha actualizado completamente el sistema de usuarios para adaptarlo al nuevo esquema de MongoDB.

### 1. Entidad User (User.cs)

**Campos Anteriores:**
- Nombre, Apellido, Email, Password, Roles[], Estado, RefreshToken, CreatedAt, UpdatedAt

**Campos Nuevos:**
```csharp
- Username (string) - Nombre de usuario único
- Email (string) - Email único
- PasswordHash (string) - Contraseña hasheada con BCrypt
- RolSistema (string) - "ADMIN", "RRHH", "COLABORADOR"
- ColaboradorId (string?) - Referencia al colaborador (opcional)
- IntentosFallidos (int) - Contador de intentos de login fallidos
- BloqueadoHasta (DateTime?) - Fecha hasta cuando el usuario está bloqueado
- UltimoAcceso (DateTime?) - Fecha del último acceso exitoso
- FechaCreacion (DateTime) - Fecha de creación del usuario
- RefreshToken (string?) - Token para refrescar JWT
- RefreshTokenExpiryTime (DateTime?) - Expiración del refresh token
```

### 2. DTOs Actualizados

#### UserCreateDto
- Username (requerido)
- Email (requerido, validado)
- Password (requerido, mínimo 6 caracteres)
- RolSistema (requerido)
- ColaboradorId (opcional)

#### UserUpdateDto
- Username (requerido)
- Email (requerido, validado)
- Password (opcional)
- RolSistema (requerido)
- ColaboradorId (opcional)

#### UserReadDto
- Id, Username, Email, RolSistema, ColaboradorId
- IntentosFallidos, BloqueadoHasta, UltimoAcceso, FechaCreacion

#### UserChangePasswordDto
- CurrentPassword (requerido)
- NewPassword (requerido, mínimo 6 caracteres)

#### UserUnblockDto
- UserId (requerido)

#### RegisterRequestDto (Auth)
- Username (requerido)
- Email (requerido)
- Password (requerido, mínimo 6 caracteres)
- RolSistema (opcional, default: "COLABORADOR")
- ColaboradorId (opcional)

#### LoginRequestDto (Auth)
- Email (opcional) - Se puede usar email O username
- Username (opcional) - Se puede usar email O username
- Password (requerido)

### 3. Repositorio (UserRepository.cs)

**Nuevos Métodos:**
- `GetByUsernameAsync(string username)` - Buscar por username
- `GetByColaboradorIdAsync(string colaboradorId)` - Buscar por colaboradorId
- `IncrementarIntentosFallidosAsync(string id)` - Incrementar intentos fallidos
- `BloquearUsuarioAsync(string id, DateTime bloqueadoHasta)` - Bloquear usuario
- `DesbloquearUsuarioAsync(string id)` - Desbloquear usuario y resetear intentos
- `ActualizarUltimoAccesoAsync(string id)` - Actualizar último acceso

### 4. Servicio (UserService.cs)

**Nuevos Métodos:**
- `GetByUsernameAsync(string username)`
- `GetByEmailAsync(string email)`
- `GetByColaboradorIdAsync(string colaboradorId)`
- `UpdateAsync(string id, UserUpdateDto dto)` - Actualizado para nuevo esquema
- `ChangePasswordAsync(string id, UserChangePasswordDto dto)` - Cambiar contraseña
- `UnblockUserAsync(string userId)` - Desbloquear usuario
- `GetBlockedUsersAsync()` - Obtener usuarios bloqueados

**Características:**
- Validación de username y email únicos
- Hash de contraseñas con BCrypt
- Verificación de contraseña actual al cambiar

### 5. AuthService (AuthService.cs)

**Mejoras:**
- Soporte para login con username O email
- Sistema de bloqueo automático tras 5 intentos fallidos
- Bloqueo temporal de 30 minutos
- Contador de intentos fallidos
- Desbloqueo automático al expirar el tiempo
- Actualización de último acceso
- Mensajes informativos de intentos restantes
- Generación de JWT con claims actualizados (username, rolSistema, colaboradorId)

### 6. UsersController (UsersController.cs)

**Endpoints Disponibles:**

#### Públicos:
- Ninguno (todos requieren autenticación)

#### Requieren Autenticación:
- `GET /api/users/{id}` - Obtener usuario por ID
- `GET /api/users/colaborador/{colaboradorId}` - Obtener por colaboradorId
- `POST /api/users/{id}/change-password` - Cambiar contraseña

#### Requieren Rol ADMIN:
- `GET /api/users` - Obtener todos los usuarios
- `GET /api/users/username/{username}` - Obtener por username
- `GET /api/users/email/{email}` - Obtener por email
- `POST /api/users` - Crear nuevo usuario
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario
- `GET /api/users/blocked` - Obtener usuarios bloqueados
- `POST /api/users/unblock` - Desbloquear usuario

### 7. Paquetes NuGet Agregados

- **BCrypt.Net-Next (v4.0.3)** - Para hash seguro de contraseñas

### 8. Archivo de Pruebas

Se creó `TATA.GestiondeTalentoMoviles.API\Tests\Users.http` con 18 pruebas para todos los endpoints:

1. Registro de usuario
2. Login de usuario
3. Obtener todos los usuarios
4. Obtener usuario por ID
5. Obtener usuario por username
6. Obtener usuario por email
7. Obtener usuario por colaborador ID
8. Crear nuevo usuario
9. Actualizar usuario
10. Cambiar contraseña
11. Obtener usuarios bloqueados
12. Desbloquear usuario
13. Eliminar usuario
14. Refresh token
15. Crear usuario colaborador
16. Login con email
17. Login con username
18. Intentar login con contraseña incorrecta (para probar bloqueo)

## Seguridad Implementada

1. **Hash de Contraseñas**: BCrypt para hash seguro (no reversible)
2. **Bloqueo Temporal**: Tras 5 intentos fallidos, bloqueo de 30 minutos
3. **Contador de Intentos**: Seguimiento de intentos fallidos
4. **Desbloqueo Automático**: Al expirar el tiempo de bloqueo
5. **Validación de Datos**: Data annotations en todos los DTOs
6. **Autorización por Roles**: Endpoints protegidos por roles
7. **JWT Tokens**: Autenticación basada en tokens con expiración
8. **Refresh Tokens**: Para renovar tokens sin volver a autenticar

## Roles del Sistema

- **ADMIN**: Acceso total al sistema, puede gestionar usuarios
- **RRHH**: Recursos Humanos, puede gestionar colaboradores y vacantes
- **COLABORADOR**: Usuario estándar, acceso limitado a sus propios datos

## Ejemplo de Usuario en MongoDB

```json
{
  "_id": {
    "$oid": "675000000000000000001001"
  },
  "username": "admin",
  "email": "admin@empresa.com",
  "passwordHash": "$2a$11$...", // Hash BCrypt
  "rolSistema": "ADMIN",
  "colaboradorId": null,
  "intentosFallidos": 0,
  "bloqueadoHasta": null,
  "ultimoAcceso": {
    "$date": "2025-01-14T15:00:00.000Z"
  },
  "fechaCreacion": {
    "$date": "2025-01-01T10:00:00.000Z"
  },
  "refreshToken": "base64_token_here",
  "refreshTokenExpiryTime": {
    "$date": "2025-01-21T15:00:00.000Z"
  }
}
```

## Notas Importantes

1. La contraseña NUNCA se almacena en texto plano, solo el hash BCrypt
2. El login acepta username O email (no ambos requeridos)
3. Los usuarios se bloquean automáticamente tras 5 intentos fallidos
4. El bloqueo dura 30 minutos y se desbloquea automáticamente
5. Los ADMIN pueden desbloquear usuarios manualmente
6. El cambio de contraseña requiere verificar la contraseña actual
7. Los JWT tokens expiran en 1 hora
8. Los refresh tokens expiran en 7 días

## Compatibilidad

- ? .NET 9
- ? C# 13.0
- ? MongoDB
- ? JWT Bearer Authentication
- ? BCrypt para contraseñas

## Estado del Proyecto

? Compilación exitosa
? Todos los endpoints implementados
? DTOs actualizados
? Repositorio actualizado
? Servicio actualizado
? Autenticación actualizada
? Autorización por roles
? Sistema de bloqueo implementado
? Archivo de pruebas creado
? Documentación completa
