# ?? GUÍA RÁPIDA DE INICIO - Módulo de Autenticación

## ? Inicio Rápido en 5 Pasos

### 1?? Verificar Configuración
Abre `appsettings.json` y verifica:
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb+srv://...",  // ? Tu conexión a MongoDB
    "DatabaseName": "talento_db"              // ? Nombre de tu BD
  },
  "Jwt": {
    "Key": "TuClaveSecretaSuperSeguraDeAlMenos32CaracteresParaJWT123456",
    "Issuer": "TATA.GestiondeTalentoMoviles.API",
    "Audience": "TATA.GestiondeTalentoMoviles.Client"
  }
}
```

### 2?? Ejecutar la API
```bash
cd TATA.GestiondeTalentoMoviles.API
dotnet run
```

La API estará disponible en: `https://localhost:7XXX` (revisa el puerto en la consola)

### 3?? Primer Registro (con Thunder Client, Postman o cURL)
```http
POST https://localhost:7XXX/api/auth/register
Content-Type: application/json

{
  "nombre": "Tu Nombre",
  "apellido": "Tu Apellido",
  "email": "tu.email@example.com",
  "password": "Password123!"
}
```

**Respuesta Exitosa:**
```json
{
  "success": true,
  "message": "Usuario registrado exitosamente",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",  // ?? COPIA ESTE TOKEN
    "refreshToken": "Qx5Z3mK8pL...",
    "tokenExpires": "2025-01-15T15:30:00Z",
    "user": {
      "id": "67a1b2c3d4e5f6g7h8i9j0k1",
      "nombreCompleto": "Tu Nombre Tu Apellido",
      "email": "tu.email@example.com",
      "roles": []
    }
  }
}
```

### 4?? Hacer Login
```http
POST https://localhost:7XXX/api/auth/login
Content-Type: application/json

{
  "email": "tu.email@example.com",
  "password": "Password123!"
}
```

### 5?? Usar el Token en Endpoints Protegidos
```http
GET https://localhost:7XXX/api/colaboradores
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## ?? Proteger un Endpoint (Para Desarrolladores)

### Opción 1: Requiere autenticación (cualquier usuario)
```csharp
[Authorize]  // ?? Solo agregar esto
[HttpGet("mi-endpoint")]
public IActionResult MiEndpoint()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var email = User.FindFirst(ClaimTypes.Email)?.Value;
    
    return Ok(new { userId, email });
}
```

### Opción 2: Requiere rol específico
```csharp
[Authorize(Roles = "Admin")]  // ?? Solo admins
[HttpDelete("usuarios/{id}")]
public IActionResult EliminarUsuario(string id)
{
    return Ok(new { message = "Usuario eliminado" });
}
```

### Opción 3: Endpoint público (sin autenticación)
```csharp
[AllowAnonymous]  // ?? Permite acceso sin token
[HttpGet("info-publica")]
public IActionResult InfoPublica()
{
    return Ok(new { message = "Esta información es pública" });
}
```

---

## ?? Debugging - Verificar Token

### Método 1: Usar jwt.io
1. Ir a https://jwt.io
2. Pegar tu token
3. Ver el contenido decodificado:
   ```json
   {
     "sub": "67a1b2c3d4e5f6g7h8i9j0k1",
     "email": "tu.email@example.com",
     "nombre": "Tu Nombre",
     "apellido": "Tu Apellido",
     "exp": 1736958600,
     "iss": "TATA.GestiondeTalentoMoviles.API",
     "aud": "TATA.GestiondeTalentoMoviles.Client"
   }
   ```

### Método 2: En tu código
```csharp
[Authorize]
[HttpGet("debug-token")]
public IActionResult DebugToken()
{
    var claims = User.Claims.Select(c => new { c.Type, c.Value });
    return Ok(claims);
}
```

---

## ? Solución de Problemas Comunes

### Problema 1: "401 Unauthorized"
**Causa:** Token inválido, expirado o faltante

**Solución:**
1. Verifica que incluyas el header `Authorization: Bearer {token}`
2. Verifica que el token no haya expirado (válido por 1 hora)
3. Haz login nuevamente para obtener un nuevo token

### Problema 2: "409 Conflict - Email ya registrado"
**Causa:** El email ya existe en la base de datos

**Solución:**
1. Usa un email diferente
2. O haz login con el email existente

### Problema 3: "400 Bad Request - Validation Error"
**Causa:** Datos inválidos en el request

**Solución:**
Verifica que:
- `nombre` no esté vacío
- `apellido` no esté vacío
- `email` tenga formato válido (ej. `usuario@dominio.com`)
- `password` tenga al menos 6 caracteres

### Problema 4: "CS0234: JwtBearer no existe"
**Causa:** Paquetes NuGet no instalados

**Solución:**
```bash
cd TATA.GestiondeTalentoMoviles.API
dotnet restore
dotnet build
```

### Problema 5: MongoDB Connection Error
**Causa:** MongoDB no está corriendo o ConnectionString es inválido

**Solución:**
1. Verifica que MongoDB esté activo
2. Verifica el ConnectionString en `appsettings.json`
3. Prueba la conexión con MongoDB Compass

---

## ?? Testing con Thunder Client (VS Code)

### Instalar Thunder Client
1. En VS Code: `Ctrl+Shift+X`
2. Buscar "Thunder Client"
3. Instalar

### Crear Colección
1. Abrir Thunder Client (ícono de rayo ?)
2. New Request ? POST
3. URL: `https://localhost:7XXX/api/auth/register`
4. Headers:
   ```
   Content-Type: application/json
   ```
5. Body (JSON):
   ```json
   {
     "nombre": "Test",
     "apellido": "User",
     "email": "test@example.com",
     "password": "Password123!"
   }
   ```
6. Send

### Guardar Token Automáticamente
En Thunder Client, pestaña **Tests**:
```javascript
const response = tc.response.json;
if (response.success) {
  tc.setVar("token", response.data.token);
  tc.setVar("refreshToken", response.data.refreshToken);
}
```

Luego en otros requests:
```
Authorization: Bearer {{token}}
```

---

## ?? Flujo Completo de Autenticación

```
???????????????????????????????????????????????????????????????
? 1. REGISTRO                                                 ?
?    POST /api/auth/register                                  ?
?    ? Hashea contraseña con BCrypt                           ?
?    ? Guarda usuario en MongoDB                              ?
?    ? Devuelve token + refreshToken                          ?
???????????????????????????????????????????????????????????????
                            ?
???????????????????????????????????????????????????????????????
? 2. USAR TOKEN                                               ?
?    GET /api/cualquier-endpoint                              ?
?    Header: Authorization: Bearer {token}                    ?
?    ? Middleware valida el token                             ?
?    ? Si válido: continúa                                    ?
?    ? Si inválido/expirado: 401 Unauthorized                 ?
???????????????????????????????????????????????????????????????
                            ?
???????????????????????????????????????????????????????????????
? 3. TOKEN EXPIRA (después de 1 hora)                        ?
?    GET /api/cualquier-endpoint                              ?
?    ? 401 Unauthorized                                       ?
???????????????????????????????????????????????????????????????
                            ?
???????????????????????????????????????????????????????????????
? 4. REFRESH TOKEN                                            ?
?    POST /api/auth/refresh                                   ?
?    Body: { "refreshToken": "..." }                          ?
?    ? Valida refreshToken                                    ?
?    ? Genera nuevo token + nuevo refreshToken                ?
?    ? Devuelve ambos                                         ?
???????????????????????????????????????????????????????????????
                            ?
???????????????????????????????????????????????????????????????
? 5. CONTINUAR USANDO LA APLICACIÓN                          ?
?    (Repetir desde paso 2)                                   ?
???????????????????????????????????????????????????????????????
```

---

## ?? Ejemplo Completo en JavaScript (Frontend)

```javascript
// 1. Registro
async function register(nombre, apellido, email, password) {
  const response = await fetch('https://localhost:7XXX/api/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ nombre, apellido, email, password })
  });
  
  const data = await response.json();
  
  if (data.success) {
    localStorage.setItem('token', data.data.token);
    localStorage.setItem('refreshToken', data.data.refreshToken);
    return data.data.user;
  } else {
    throw new Error(data.message);
  }
}

// 2. Login
async function login(email, password) {
  const response = await fetch('https://localhost:7XXX/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  
  const data = await response.json();
  
  if (data.success) {
    localStorage.setItem('token', data.data.token);
    localStorage.setItem('refreshToken', data.data.refreshToken);
    return data.data.user;
  } else {
    throw new Error(data.message);
  }
}

// 3. Hacer request autenticado
async function fetchProtectedData() {
  const token = localStorage.getItem('token');
  
  const response = await fetch('https://localhost:7XXX/api/colaboradores', {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  
  if (response.status === 401) {
    // Token expirado, intentar refresh
    await refreshToken();
    return fetchProtectedData(); // Reintentar
  }
  
  return await response.json();
}

// 4. Refresh token
async function refreshToken() {
  const refreshToken = localStorage.getItem('refreshToken');
  
  const response = await fetch('https://localhost:7XXX/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken })
  });
  
  const data = await response.json();
  
  if (data.success) {
    localStorage.setItem('token', data.data.token);
    localStorage.setItem('refreshToken', data.data.refreshToken);
  } else {
    // Refresh token también expiró, redirigir a login
    window.location.href = '/login';
  }
}

// 5. Logout
function logout() {
  localStorage.removeItem('token');
  localStorage.removeItem('refreshToken');
  window.location.href = '/login';
}
```

---

## ?? Más Información

- **Documentación completa:** `DOCS_AUTH_MODULE.md`
- **Testing:** `TESTING_AUTH_REQUESTS.md`
- **Resumen técnico:** `RESUMEN_IMPLEMENTACION.md`

---

## ? Checklist de Verificación

Antes de empezar a usar el módulo:

- [ ] MongoDB está corriendo
- [ ] `appsettings.json` tiene ConnectionString válido
- [ ] `appsettings.json` tiene configuración JWT
- [ ] API está corriendo (`dotnet run`)
- [ ] Puerto en URL coincide con `launchSettings.json`
- [ ] Puedes hacer un POST a `/api/auth/register`

---

**¡Listo para Comenzar! ??**

Empieza con el paso 3 de la Guía Rápida: hacer tu primer registro.
