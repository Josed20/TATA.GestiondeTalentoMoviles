# ?? GUÍA RÁPIDA - Endpoints de Autenticación y Usuarios

## Base URL
```
http://localhost:5064/api
```

---

## ?? AUTENTICACIÓN

### 1. Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "admin",
    "rolSistema": "ADMIN",
    "colaboradorId": null
  }
}
```
**?? Guardar el `token` para usarlo en los siguientes requests**

---

### 2. Cambiar Contraseña (Usuario autenticado)
```http
POST /api/auth/change-password
Content-Type: application/json
Authorization: Bearer {token}

{
  "currentPassword": "admin123",
  "newPassword": "admin456"
}
```

---

## ?? USUARIOS (Requieren Token de ADMIN)

### 3. Listar Todos los Usuarios
```http
GET /api/users
Authorization: Bearer {token}
```

---

### 4. Obtener Usuario por ID
```http
GET /api/users/{userId}
```

---

### 5. Crear Usuario Colaborador
```http
POST /api/users
Content-Type: application/json
Authorization: Bearer {token}

{
  "username": "juanperez",
  "email": "juan.perez@example.com",
  "password": "password123",
  "rolSistema": "COLABORADOR",
  "colaboradorId": "60d5ec49f1b2c8b1f8e4e1a1"
}
```

**Roles válidos:**
- `ADMIN`
- `BUSINESS_MANAGER`
- `COLABORADOR`

---

### 6. Actualizar Usuario
```http
PUT /api/users/{userId}
Content-Type: application/json
Authorization: Bearer {token}

{
  "email": "juan.actualizado@example.com",
  "rolSistema": "BUSINESS_MANAGER",
  "colaboradorId": "60d5ec49f1b2c8b1f8e4e1a3"
}
```

---

### 7. Resetear Contraseña (Admin resetea la contraseña de otro usuario)
```http
POST /api/users/{userId}/reset-password
Content-Type: application/json
Authorization: Bearer {token}

{
  "newPassword": "nuevaPassword123"
}
```

---

### 8. Desbloquear Usuario (Después de 5 intentos fallidos)
```http
POST /api/users/{userId}/unblock
Authorization: Bearer {token}
```

---

### 9. Eliminar Usuario
```http
DELETE /api/users/{userId}
Authorization: Bearer {token}
```

---

## ?? ERRORES COMUNES

### Error 401 - No autorizado
```json
{
  "success": false,
  "message": "Usuario o contraseña incorrecta."
}
```
**Solución:** Verificar credenciales

---

### Error 401 - Cuenta bloqueada
```json
{
  "success": false,
  "message": "Cuenta bloqueada temporalmente. Intente de nuevo en 15 minutos."
}
```
**Solución:** Esperar 15 minutos o usar el endpoint de desbloqueo (como admin)

---

### Error 400 - Rol inválido
```json
{
  "success": false,
  "message": "El rol 'ROL_INVALIDO' no es válido. Roles válidos: ADMIN, BUSINESS_MANAGER, COLABORADOR"
}
```
**Solución:** Usar uno de los roles válidos

---

### Error 409 - Username duplicado
```json
{
  "success": false,
  "message": "El nombre de usuario ya existe."
}
```
**Solución:** Usar un username diferente

---

### Error 400 - Validación
```json
{
  "success": false,
  "message": "Error de validación",
  "errors": [
    {
      "Field": "Email",
      "Errors": ["El email no tiene un formato válido"]
    }
  ]
}
```
**Solución:** Corregir los campos según los mensajes de error

---

## ?? REGLAS DE SEGURIDAD

- **Token JWT:** Expira en 8 horas
- **Intentos de login:** 5 intentos fallidos = bloqueo de 15 minutos
- **Contraseña mínima:** 6 caracteres
- **Roles requeridos:** Solo ADMIN puede gestionar usuarios

---

## ?? FLUJO DE PRUEBA

1. **Login** ? Obtener token
2. **Listar usuarios** ? Ver usuarios existentes
3. **Crear usuario** ? Crear nuevo colaborador/manager
4. **Login con nuevo usuario** ? Probar credenciales
5. **Actualizar/Eliminar** ? Gestionar usuarios

---

## ?? NOTAS IMPORTANTES

1. **Para crear el primer usuario ADMIN**, debes insertarlo directamente en MongoDB:
   ```javascript
   db.usuarios.insertOne({
     username: "admin",
     email: "admin@example.com",
     passwordHash: "$2a$11$..." // Hash de "admin123" generado con BCrypt
     rolSistema: "ADMIN",
     colaboradorId: null,
     intentosFallidos: 0,
     bloqueadoHasta: null,
     ultimoAcceso: new Date(),
     fechaCreacion: new Date()
   })
   ```

2. **El token debe incluirse en el header:**
   ```
   Authorization: Bearer {token}
   ```

3. **Todos los endpoints de `/api/users` (excepto GET por ID) requieren rol ADMIN**

---

**Versión:** 1.0  
**Fecha:** Enero 2024
