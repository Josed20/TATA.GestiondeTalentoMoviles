# ?? Generador de Hash BCrypt para MongoDB

Este documento explica cómo generar manualmente un hash BCrypt para crear usuarios en MongoDB.

## Opción 1: Usar una Herramienta Online

Visita: [https://bcrypt-generator.com/](https://bcrypt-generator.com/)

1. Ingresa tu contraseña (ej: `Admin123456`)
2. Selecciona **rounds: 11** (coincide con nuestro código)
3. Haz clic en **"Generate Hash"**
4. Copia el hash generado

**Ejemplo:**
- **Password:** `Admin123456`
- **Hash:** `$2a$11$ZK9XGCvLHPXwFKZqjN2EHO.3F8gAj7cP0U7Q6iBxVx9K5QJw8T8O.`

## Opción 2: Usar C# (Crear un Console App temporal)

Si prefieres generar el hash usando código C#, crea un proyecto de consola temporal:

```bash
dotnet new console -n HashGenerator
cd HashGenerator
dotnet add package BCrypt.Net-Next
```

Luego edita `Program.cs`:

```csharp
using BCrypt.Net;

Console.WriteLine("?? Generador de Hash BCrypt");
Console.WriteLine("================================\n");

Console.Write("Ingresa la contraseña a hashear: ");
string password = Console.ReadLine() ?? "";

if (string.IsNullOrEmpty(password))
{
    Console.WriteLine("? La contraseña no puede estar vacía");
    return;
}

// Generar el hash con 11 rounds (coincide con la configuración del API)
string hash = BCrypt.Net.BCrypt.HashPassword(password, 11);

Console.WriteLine($"\n? Hash generado exitosamente:");
Console.WriteLine($"\n{hash}\n");

// Verificar que el hash funcione
bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
Console.WriteLine($"?? Verificación: {(isValid ? "? VÁLIDO" : "? INVÁLIDO")}");
```

Ejecuta el programa:

```bash
dotnet run
```

## Opción 3: Usar Node.js

Si tienes Node.js instalado:

```bash
npm install bcrypt
```

Crea un archivo `hash-generator.js`:

```javascript
const bcrypt = require('bcrypt');

const password = 'Admin123456'; // Cambia esto por tu contraseña
const rounds = 11; // Debe coincidir con tu configuración

bcrypt.hash(password, rounds, (err, hash) => {
  if (err) {
    console.error('Error:', err);
    return;
  }
  console.log('Password:', password);
  console.log('Hash:', hash);
  
  // Verificar
  bcrypt.compare(password, hash, (err, result) => {
    console.log('Verificación:', result ? '? VÁLIDO' : '? INVÁLIDO');
  });
});
```

Ejecuta:

```bash
node hash-generator.js
```

## ?? Usuarios Predefinidos

Aquí hay algunos hashes precalculados para usuarios comunes:

### Usuario: admin
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
**Password:** `Admin123456`

### Usuario: manager
```json
{
  "username": "manager",
  "email": "manager@empresa.com",
  "passwordHash": "$2a$11$XYZ123ABC456DEF789GHI.JKL012MNO345PQR678STU901VWX234YZ0",
  "rolSistema": "BUSINESS_MANAGER",
  "colaboradorId": null,
  "intentosFallidos": 0,
  "bloqueadoHasta": null,
  "ultimoAcceso": { "$date": "2024-01-01T00:00:00.000Z" },
  "fechaCreacion": { "$date": "2024-01-01T00:00:00.000Z" }
}
```
**Password:** `Manager123456`

### Usuario: user
```json
{
  "username": "user",
  "email": "user@empresa.com",
  "passwordHash": "$2a$11$ABC987ZYX654WVU321TSR.QPO098NML765KJI432HGF109EDC876BA5",
  "rolSistema": "COLABORADOR",
  "colaboradorId": "677e1234567890abcdef1234",
  "intentosFallidos": 0,
  "bloqueadoHasta": null,
  "ultimoAcceso": { "$date": "2024-01-01T00:00:00.000Z" },
  "fechaCreacion": { "$date": "2024-01-01T00:00:00.000Z" }
}
```
**Password:** `User123456`

## ?? Importante

- **NUNCA** guardes contraseñas en texto plano en la base de datos
- Los hashes son únicos cada vez que se generan (aunque sean de la misma contraseña)
- BCrypt incluye automáticamente un "salt" aleatorio
- El número de rounds (11) determina la complejidad del hash (más rounds = más seguro pero más lento)

## ?? Verificar un Hash

Para verificar si un hash corresponde a una contraseña específica, puedes usar la herramienta online o código:

### Online
[https://bcrypt-generator.com/](https://bcrypt-generator.com/)
- Pega el hash en el campo "Hash"
- Ingresa la contraseña en "Password"
- Haz clic en "Check"

### C#
```csharp
string password = "Admin123456";
string hash = "$2a$11$ZK9XGCvLHPXwFKZqjN2EHO.3F8gAj7cP0U7Q6iBxVx9K5QJw8T8O.";
bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
Console.WriteLine(isValid ? "? Válido" : "? Inválido");
```

---

**Nota:** Los hashes de `manager` y `user` son ejemplos ficticios. Debes generar tus propios hashes si necesitas crear esos usuarios manualmente en MongoDB.
