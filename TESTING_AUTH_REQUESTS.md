# Colección de Requests para Testing - Módulo de Autenticación

## ?? Thunder Client / Postman Collection

### Variables de Entorno
```json
{
  "baseUrl": "https://localhost:7XXX",
  "token": "",
  "refreshToken": ""
}
```

---

## 1?? Register (Registro de Usuario)

### Request
```
POST {{baseUrl}}/api/auth/register
Content-Type: application/json
```

### Body
```json
{
  "nombre": "Mattias",
  "apellido": "Caballero",
  "email": "mattias.caballero@tata.com",
  "password": "Password123!"
}
```

### Script Post-Request (Thunder Client)
```javascript
// Guardar el token y refreshToken automáticamente
const response = tc.response.json;
if (response.success) {
  tc.setVar("token", response.data.token);
  tc.setVar("refreshToken", response.data.refreshToken);
}
```

### Expected Response (200 OK)
```json
{
  "success": true,
  "message": "Usuario registrado exitosamente",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2N2E...",
    "refreshToken": "Qx5Z3mK8pL2vN9tR4sT6uV8wX0yZ1aB...",
    "tokenExpires": "2025-01-15T15:30:00Z",
    "user": {
      "id": "67a1b2c3d4e5f6g7h8i9j0k1",
      "nombreCompleto": "Mattias Caballero",
      "email": "mattias.caballero@tata.com",
      "roles": []
    }
  }
}
```

---

## 2?? Login (Autenticación)

### Request
```
POST {{baseUrl}}/api/auth/login
Content-Type: application/json
```

### Body
```json
{
  "email": "mattias.caballero@tata.com",
  "password": "Password123!"
}
```

### Script Post-Request (Thunder Client)
```javascript
const response = tc.response.json;
if (response.success) {
  tc.setVar("token", response.data.token);
  tc.setVar("refreshToken", response.data.refreshToken);
}
```

### Expected Response (200 OK)
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "nEwT0k3nF0rR3fR3sh...",
    "tokenExpires": "2025-01-15T16:30:00Z",
    "user": {
      "id": "67a1b2c3d4e5f6g7h8i9j0k1",
      "nombreCompleto": "Mattias Caballero",
      "email": "mattias.caballero@tata.com",
      "roles": []
    }
  }
}
```

### Error Response (401 Unauthorized)
```json
{
  "success": false,
  "message": "Credenciales incorrectas"
}
```

---

## 3?? Refresh Token (Renovar Token)

### Request
```
POST {{baseUrl}}/api/auth/refresh
Content-Type: application/json
```

### Body
```json
{
  "refreshToken": "{{refreshToken}}"
}
```

### Script Post-Request (Thunder Client)
```javascript
const response = tc.response.json;
if (response.success) {
  tc.setVar("token", response.data.token);
  tc.setVar("refreshToken", response.data.refreshToken);
}
```

### Expected Response (200 OK)
```json
{
  "success": true,
  "message": "Token refrescado exitosamente",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "uPd4t3dR3fR3shT0k3n...",
    "tokenExpires": "2025-01-15T17:30:00Z",
    "user": {
      "id": "67a1b2c3d4e5f6g7h8i9j0k1",
      "nombreCompleto": "Mattias Caballero",
      "email": "mattias.caballero@tata.com",
      "roles": []
    }
  }
}
```

### Error Response (401 Unauthorized)
```json
{
  "success": false,
  "message": "Refresh token inválido o expirado"
}
```

---

## 4?? Protected Endpoint Example (Con Autenticación)

### Request
```
GET {{baseUrl}}/api/colaboradores
Authorization: Bearer {{token}}
```

### Expected Response (200 OK)
```json
{
  "success": true,
  "data": [...]
}
```

### Error Response (401 Unauthorized)
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401
}
```

---

## ?? Escenarios de Prueba

### ? Happy Path (Flujo Exitoso)
1. **Register** ? Guardar token
2. Usar el token en requests protegidos
3. Esperar 1 hora (o cambiar la expiración a 1 minuto para testing)
4. **Refresh** ? Obtener nuevo token
5. Continuar usando el nuevo token

### ? Error Scenarios (Casos de Error)

#### Registro con email duplicado
```json
POST /api/auth/register
{
  "nombre": "Otro",
  "apellido": "Usuario",
  "email": "mattias.caballero@tata.com", // Email ya existe
  "password": "Password123!"
}
```
**Response (409 Conflict):**
```json
{
  "success": false,
  "message": "El email ya está registrado"
}
```

#### Login con contraseña incorrecta
```json
POST /api/auth/login
{
  "email": "mattias.caballero@tata.com",
  "password": "WrongPassword"
}
```
**Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Credenciales incorrectas"
}
```

#### Validación de campos
```json
POST /api/auth/register
{
  "nombre": "",
  "apellido": "Caballero",
  "email": "invalid-email",
  "password": "123" // Menos de 6 caracteres
}
```
**Response (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nombre": ["El nombre es requerido"],
    "Email": ["El formato del email no es válido"],
    "Password": ["La contraseña debe tener al menos 6 caracteres"]
  }
}
```

#### Token expirado
```json
GET /api/colaboradores
Authorization: Bearer expired_token_here
```
**Response (401 Unauthorized):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401
}
```

---

## ?? Thunder Client Collection (JSON)

Puedes importar esta colección en Thunder Client:

```json
{
  "clientName": "Thunder Client",
  "collectionName": "TATA Auth Module",
  "collectionId": "tata-auth-module",
  "dateExported": "2025-01-15",
  "version": "1.0",
  "folders": [],
  "requests": [
    {
      "name": "Register User",
      "method": "POST",
      "url": "{{baseUrl}}/api/auth/register",
      "headers": [
        {
          "name": "Content-Type",
          "value": "application/json"
        }
      ],
      "body": {
        "type": "json",
        "raw": "{\n  \"nombre\": \"Mattias\",\n  \"apellido\": \"Caballero\",\n  \"email\": \"mattias.caballero@tata.com\",\n  \"password\": \"Password123!\"\n}"
      },
      "tests": [
        {
          "type": "set-env-var",
          "custom": "const response = tc.response.json; if (response.success) { tc.setVar('token', response.data.token); tc.setVar('refreshToken', response.data.refreshToken); }"
        }
      ]
    },
    {
      "name": "Login",
      "method": "POST",
      "url": "{{baseUrl}}/api/auth/login",
      "headers": [
        {
          "name": "Content-Type",
          "value": "application/json"
        }
      ],
      "body": {
        "type": "json",
        "raw": "{\n  \"email\": \"mattias.caballero@tata.com\",\n  \"password\": \"Password123!\"\n}"
      },
      "tests": [
        {
          "type": "set-env-var",
          "custom": "const response = tc.response.json; if (response.success) { tc.setVar('token', response.data.token); tc.setVar('refreshToken', response.data.refreshToken); }"
        }
      ]
    },
    {
      "name": "Refresh Token",
      "method": "POST",
      "url": "{{baseUrl}}/api/auth/refresh",
      "headers": [
        {
          "name": "Content-Type",
          "value": "application/json"
        }
      ],
      "body": {
        "type": "json",
        "raw": "{\n  \"refreshToken\": \"{{refreshToken}}\"\n}"
      },
      "tests": [
        {
          "type": "set-env-var",
          "custom": "const response = tc.response.json; if (response.success) { tc.setVar('token', response.data.token); tc.setVar('refreshToken', response.data.refreshToken); }"
        }
      ]
    }
  ]
}
```

---

## ?? Verificar Token JWT

Puedes decodificar el token en: https://jwt.io

### Ejemplo de Payload Decodificado:
```json
{
  "sub": "67a1b2c3d4e5f6g7h8i9j0k1",
  "email": "mattias.caballero@tata.com",
  "jti": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "nombre": "Mattias",
  "apellido": "Caballero",
  "exp": 1736958600,
  "iss": "TATA.GestiondeTalentoMoviles.API",
  "aud": "TATA.GestiondeTalentoMoviles.Client"
}
```

---

## ?? Cómo Usar

1. **Cambiar el puerto** en `baseUrl` según tu `launchSettings.json`
2. **Ejecutar Register** ? Token guardado automáticamente
3. **Copiar el token** para usar en otros endpoints
4. **Agregar header** `Authorization: Bearer {token}` en requests protegidos

---

## ?? Testing de Expiración (Opcional)

Para probar la expiración rápidamente, cambia en `appsettings.json`:

```json
{
  "Jwt": {
    "ExpirationInHours": 0.0166667  // 1 minuto
  }
}
```

Luego:
1. **Register/Login** ? Obtener token
2. **Esperar 1 minuto**
3. **Usar el token** ? Debería fallar (401)
4. **Refresh** ? Obtener nuevo token válido

---

**¡Colección de Testing Completa! ??**
