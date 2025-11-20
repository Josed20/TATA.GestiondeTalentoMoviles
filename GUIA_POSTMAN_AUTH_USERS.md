# ?? Guía de Uso - Colección de Postman para Auth & Users

## ?? Índice
1. [Paso 0: Crear el Usuario Admin Inicial](#paso-0-crear-el-usuario-admin-inicial)
2. [Importar la Colección en Postman](#importar-la-colección-en-postman)
3. [Configurar Variables de Entorno](#configurar-variables-de-entorno)
4. [Probar los Endpoints](#probar-los-endpoints)
5. [Notas Importantes](#notas-importantes)

---

## ??? Paso 0: Crear el Usuario Admin Inicial

Antes de probar los endpoints, necesitas crear manualmente un usuario administrador en MongoDB. Este será tu primer usuario y te permitirá acceder al sistema.

### Opción 1: Usando MongoDB Compass

1. Abre **MongoDB Compass** y conéctate a tu base de datos
2. Navega a la colección **`usuarios`**
3. Haz clic en **"ADD DATA"** ? **"Insert Document"**
4. Copia y pega el siguiente JSON:

```json
{
  "username": "admin",
  "email": "admin@empresa.com",
  "passwordHash": "$2a$11$ZK9XGCvLHPXwFKZqjN2EHO.3F8gAj7cP0U7Q6iBxVx9K5QJw8T8O.",
  "rolSistema": "ADMIN",
  "colaboradorId": null,
  "intentosFallidos": 0,
  "bloqueadoHasta": null,
  "ultimoAcceso": { "$date": "2024-01-01T00:00:00.000Z" },
  "fechaCreacion": { "$date": "2024-01-01T00:00:00.000Z" }
}
```

5. Haz clic en **"Insert"**

### Opción 2: Usando MongoDB Shell

Ejecuta el siguiente comando en MongoDB Shell:

```javascript
db.usuarios.insertOne({
  username: "admin",
  email: "admin@empresa.com",
  passwordHash: "$2a$11$ZK9XGCvLHPXwFKZqjN2EHO.3F8gAj7cP0U7Q6iBxVx9K5QJw8T8O.",
  rolSistema: "ADMIN",
  colaboradorId: null,
  intentosFallidos: 0,
  bloqueadoHasta: null,
  ultimoAcceso: new Date("2024-01-01T00:00:00.000Z"),
  fechaCreacion: new Date("2024-01-01T00:00:00.000Z")
})
```

### ?? Credenciales del Admin

- **Username:** `admin`
- **Password:** `Admin123456`

> ?? **IMPORTANTE**: El `passwordHash` proporcionado es el resultado de hashear "Admin123456" con BCrypt. NO intentes guardar contraseñas en texto plano.

---

## ?? Importar la Colección en Postman

1. Abre **Postman**
2. Haz clic en **"Import"** (esquina superior izquierda)
3. Selecciona el archivo **`TATA.GestiondeTalentoMoviles_Postman_Collection.json`**
4. Haz clic en **"Import"**

La colección aparecerá en tu barra lateral con el nombre **"TATA Gestión de Talento - Auth & Users"**.

---

## ?? Configurar Variables de Entorno

La colección utiliza variables para facilitar las pruebas. Las variables se guardan automáticamente cuando ejecutas ciertos requests.

### Variables incluidas:

| Variable | Descripción | Valor Inicial |
|----------|-------------|---------------|
| `base_url` | URL base del API | `http://localhost:5000` |
| `jwt_token` | Token JWT (se guarda automáticamente después del login) | (vacío) |
| `user_id` | ID del último usuario creado (se guarda automáticamente) | (vacío) |

### Cómo ver/editar variables:

1. Haz clic en el **ícono del ojo** (???) en la esquina superior derecha
2. O haz clic en **"Environments"** en la barra lateral
3. Edita el `base_url` si tu API corre en otro puerto

---

## ?? Probar los Endpoints

### 1?? **Login (Autenticación)**

?? **Endpoint:** `POST /api/auth/login`

**Descripción:** Autentica al usuario y devuelve un token JWT.

**Cómo probarlo:**
1. Abre la carpeta **"Auth"** en la colección
2. Selecciona **"Login"**
3. El body ya viene con las credenciales del admin:
   ```json
   {
     "username": "admin",
     "password": "Admin123456"
   }
   ```
4. Haz clic en **"Send"**

**? Respuesta Exitosa (200):**
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "username": "admin",
    "rolSistema": "ADMIN",
    "colaboradorId": null
  }
}
```

> ?? **Nota:** El token se guarda automáticamente en la variable `jwt_token` gracias al script de Postman incluido.

---

### 2?? **Get All Users (Listar Usuarios)**

?? **Endpoint:** `GET /api/users`

**Descripción:** Obtiene la lista completa de usuarios (Solo ADMIN).

**Cómo probarlo:**
1. Primero debes hacer login (paso anterior)
2. Abre la carpeta **"Users"**
3. Selecciona **"Get All Users"**
4. Haz clic en **"Send"**

**? Respuesta Exitosa (200):**
```json
{
  "success": true,
  "data": [
    {
      "id": "677e1234567890abcdef1234",
      "username": "admin",
      "email": "admin@empresa.com",
      "rolSistema": "ADMIN",
      "colaboradorId": null,
      "intentosFallidos": 0,
      "bloqueadoHasta": null,
      "ultimoAcceso": "2024-01-10T15:30:00.000Z",
      "fechaCreacion": "2024-01-01T00:00:00.000Z"
    }
  ]
}
```

---

### 3?? **Get User By ID (Obtener Usuario por ID)**

?? **Endpoint:** `GET /api/users/{id}`

**Descripción:** Obtiene un usuario específico por su ID (Permite acceso anónimo).

**Cómo probarlo:**
1. Reemplaza `{{user_id}}` en la URL con un ID real de usuario
2. O usa la variable automática después de crear un usuario
3. Haz clic en **"Send"**

**? Respuesta Exitosa (200):**
```json
{
  "success": true,
  "data": {
    "id": "677e1234567890abcdef1234",
    "username": "jperez",
    "email": "jperez@empresa.com",
    "rolSistema": "COLABORADOR",
    "colaboradorId": "677e1234567890abcdef5678",
    "intentosFallidos": 0,
    "bloqueadoHasta": null,
    "ultimoAcceso": "2024-01-10T15:30:00.000Z",
    "fechaCreacion": "2024-01-10T10:00:00.000Z"
  }
}
```

---

### 4?? **Create User (Crear Usuario)**

?? **Endpoint:** `POST /api/users`

**Descripción:** Crea un nuevo usuario (Solo ADMIN).

**Cómo probarlo:**
1. Asegúrate de estar autenticado (haber hecho login)
2. Selecciona **"Create User"**
3. El body ya viene con un ejemplo:
   ```json
   {
     "username": "jperez",
     "email": "jperez@empresa.com",
     "password": "Password123",
     "rolSistema": "COLABORADOR",
     "colaboradorId": "677e1234567890abcdef1234"
   }
   ```
4. Modifica los datos según necesites
5. Haz clic en **"Send"**

**? Respuesta Exitosa (201):**
```json
{
  "success": true,
  "data": {
    "id": "677e9876543210fedcba9876",
    "username": "jperez",
    "email": "jperez@empresa.com",
    "rolSistema": "COLABORADOR",
    "colaboradorId": "677e1234567890abcdef1234",
    "intentosFallidos": 0,
    "bloqueadoHasta": null,
    "ultimoAcceso": "2024-01-10T16:00:00.000Z",
    "fechaCreacion": "2024-01-10T16:00:00.000Z"
  }
}
```

> ?? **Nota:** El ID del usuario creado se guarda automáticamente en la variable `user_id`.

**Roles Válidos:**
- `ADMIN`
- `BUSINESS_MANAGER`
- `COLABORADOR`

---

### 5?? **Update User (Actualizar Usuario)**

?? **Endpoint:** `PUT /api/users/{id}`

**Descripción:** Actualiza el email y rol de un usuario (Solo ADMIN).

**Cómo probarlo:**
1. Asegúrate de tener un `user_id` válido (creado en el paso anterior)
2. Selecciona **"Update User"**
3. El body incluye:
   ```json
   {
     "email": "jperez.updated@empresa.com",
     "rolSistema": "BUSINESS_MANAGER",
     "colaboradorId": "677e1234567890abcdef1234"
   }
   ```
4. Modifica los datos según necesites
5. Haz clic en **"Send"**

**? Respuesta Exitosa (200):**
```json
{
  "success": true,
  "message": "Usuario actualizado exitosamente",
  "data": {
    "id": "677e9876543210fedcba9876",
    "username": "jperez",
    "email": "jperez.updated@empresa.com",
    "rolSistema": "BUSINESS_MANAGER",
    "colaboradorId": "677e1234567890abcdef1234",
    "intentosFallidos": 0,
    "bloqueadoHasta": null,
    "ultimoAcceso": "2024-01-10T16:00:00.000Z",
    "fechaCreacion": "2024-01-10T16:00:00.000Z"
  }
}
```

---

### 6?? **Change Password (Cambiar Contraseña)**

?? **Endpoint:** `POST /api/auth/change-password`

**Descripción:** Permite al usuario autenticado cambiar su propia contraseña.

**Cómo probarlo:**
1. Asegúrate de estar autenticado
2. Selecciona **"Change Password"**
3. El body incluye:
   ```json
   {
     "currentPassword": "Admin123456",
     "newPassword": "NewPassword123"
   }
   ```
4. Modifica las contraseñas según corresponda
5. Haz clic en **"Send"**

**? Respuesta Exitosa (200):**
```json
{
  "success": true,
  "message": "Contraseña actualizada exitosamente"
}
```

---

### 7?? **Reset Password (Resetear Contraseña)**

?? **Endpoint:** `POST /api/users/{id}/reset-password`

**Descripción:** El ADMIN puede forzar el cambio de contraseña de cualquier usuario.

**Cómo probarlo:**
1. Asegúrate de estar autenticado como ADMIN
2. Selecciona **"Reset Password"**
3. El body incluye:
   ```json
   {
     "newPassword": "NewTempPassword123"
   }
   ```
4. Haz clic en **"Send"**

**? Respuesta Exitosa (200):**
```json
{
  "success": true,
  "message": "Contraseña reseteada exitosamente"
}
```

---

### 8?? **Unblock User (Desbloquear Usuario)**

?? **Endpoint:** `POST /api/users/{id}/unblock`

**Descripción:** Desbloquea un usuario que fue bloqueado por intentos fallidos de login (Solo ADMIN).

**Cómo probarlo:**
1. Asegúrate de estar autenticado como ADMIN
2. Selecciona **"Unblock User"**
3. Haz clic en **"Send"**

**? Respuesta Exitosa (200):**
```json
{
  "success": true,
  "message": "Usuario desbloqueado exitosamente"
}
```

---

### 9?? **Delete User (Eliminar Usuario)**

?? **Endpoint:** `DELETE /api/users/{id}`

**Descripción:** Elimina un usuario permanentemente (Solo ADMIN).

**Cómo probarlo:**
1. Asegúrate de estar autenticado como ADMIN
2. Selecciona **"Delete User"**
3. Haz clic en **"Send"**

**? Respuesta Exitosa (200):**
```json
{
  "success": true,
  "message": "Usuario eliminado exitosamente"
}
```

---

## ?? Notas Importantes

### ?? Autenticación y Autorización

- **Login:** No requiere autenticación (es el punto de entrada)
- **Get User By ID:** Permite acceso anónimo (cualquiera puede ver un perfil)
- **Todos los demás endpoints de Users:** Requieren rol `ADMIN`
- **Change Password:** Requiere estar autenticado (cualquier rol)

### ?? Tokens JWT

- Los tokens JWT expiran después de **8 horas**
- Si recibes un error `401 Unauthorized`, necesitas hacer login nuevamente
- El token se guarda automáticamente en la variable `jwt_token` después del login

### ?? Sistema de Bloqueo

- Después de **5 intentos fallidos** de login, el usuario se bloquea por **15 minutos**
- Durante el bloqueo, el usuario verá el mensaje: _"Cuenta bloqueada temporalmente. Intente de nuevo en 15 minutos."_
- El ADMIN puede desbloquear manualmente usando el endpoint `Unblock User`

### ?? Errores Comunes

| Error | Descripción | Solución |
|-------|-------------|----------|
| `401 Unauthorized` | Token inválido o expirado | Haz login nuevamente |
| `403 Forbidden` | No tienes permisos (no eres ADMIN) | Usa credenciales de ADMIN |
| `404 Not Found` | Usuario no encontrado | Verifica el ID del usuario |
| `409 Conflict` | Usuario ya existe | Usa otro username |

---

## ?? Flujo Recomendado para Pruebas

1. **Login** con el admin
2. **Get All Users** para ver la lista actual
3. **Create User** para crear un usuario de prueba
4. **Get User By ID** para ver el usuario recién creado
5. **Update User** para cambiar el rol o email
6. **Reset Password** para cambiar la contraseña del usuario
7. **Unblock User** (si está bloqueado)
8. **Delete User** para limpiar datos de prueba

---

## ?? Soporte

Si encuentras algún problema:

1. Verifica que el API esté corriendo en `http://localhost:5000`
2. Revisa que MongoDB esté activo y conectado
3. Asegúrate de que el usuario admin exista en la base de datos
4. Verifica que el token JWT sea válido y no esté expirado

---

**¡Listo!** ?? Ahora puedes probar todos los endpoints de Auth y Users con Postman.
