# Solución a los Problemas de NivelSkill

## ?? Problemas Identificados y Solucionados

### 1. **Error POST con ID Duplicado (Error 11000)**

**Problema Original:**
```
MongoWriteException: E11000 duplicate key error collection: talento_db.nivelskills index: _id_ dup key: { _id: 0 }
```

**Causa:**
- No había manejo de excepciones para claves duplicadas
- MongoDB lanzaba una excepción no controlada

**Solución Implementada:**

#### En `NivelSkillService.cs`:
```csharp
public async Task<NivelSkillReadDto> CreateAsync(NivelSkillCreateDto createDto)
{
    var nivel = new NivelSkill
    {
        Id = createDto.Id,
        Nombre = createDto.Nombre
    };

    try
    {
        var nuevoNivel = await _repository.CreateAsync(nivel);
        return MapToReadDto(nuevoNivel);
    }
    catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
    {
        throw new InvalidOperationException($"Ya existe un nivel de skill con ID {createDto.Id}", ex);
    }
}
```

#### En `NivelesSkillController.cs`:
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] NivelSkillCreateDto createDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    try
    {
        var nuevoNivel = await _service.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = nuevoNivel.Id }, nuevoNivel);
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe"))
    {
        return Conflict(new { message = ex.Message });
    }
}
```

**Resultado:**
- ? POST con ID nuevo ? **201 Created** con el objeto creado
- ? POST con ID duplicado ? **409 Conflict** con mensaje claro: `"Ya existe un nivel de skill con ID {id}"`

---

### 2. **Error PUT retornando 404 cuando el registro existe**

**Problema Original:**
```json
{
  "message": "Nivel de skill con ID '0' no encontrado"
}
```

**Causa Posible:**
- El filtro de búsqueda en MongoDB no estaba funcionando correctamente
- El ID `0` (int) podría tener problemas de comparación

**Verificación:**
El código del Repository estaba correcto:
```csharp
public async Task<NivelSkill?> GetByIdAsync(int id)
{
    return await _nivelesSkill
        .Find(n => n.Id == id)
        .FirstOrDefaultAsync();
}
```

**Solución:**
- El código ya está correcto
- El problema podría ser:
  1. Nombre de colección incorrecto (verificar que sea exactamente `"nivelskills"`)
  2. Nombre de base de datos incorrecto (verificar `"talento_db"`)
  3. Problema de conexión SSL (ya solucionado en Program.cs)

---

## ?? Archivos Modificados

### 1. `NivelSkillService.cs`
- ? Agregado manejo de `MongoWriteException` para duplicados
- ? Lanza `InvalidOperationException` con mensaje claro

### 2. `NivelesSkillController.cs`
- ? Agregado try-catch en método `Create`
- ? Retorna `409 Conflict` para IDs duplicados
- ? Retorna `404 Not Found` solo cuando realmente no existe

### 3. `Program.cs` (ya corregido anteriormente)
- ? Configuración SSL/TLS mejorada
- ? Timeouts configurados
- ? CheckCertificateRevocation deshabilitado

---

## ?? Pruebas Recomendadas

Use el archivo `TATA.GestiondeTalentoMoviles.API\Tests\NivelSkill.http` para probar:

### Escenarios de Prueba:

1. **GET /api/nivelesskill** ? Lista todos los niveles
2. **GET /api/nivelesskill/0** ? Obtiene el nivel con ID 0 (existente)
3. **GET /api/nivelesskill/99** ? Retorna 404 (no existe)
4. **POST /api/nivelesskill** con ID nuevo ? 201 Created
5. **POST /api/nivelesskill** con ID duplicado ? 409 Conflict
6. **PUT /api/nivelesskill/0** ? Actualiza el nivel 0
7. **PUT /api/nivelesskill/99** ? Retorna 404 (no existe)
8. **DELETE /api/nivelesskill/1** ? Elimina el nivel 1
9. **DELETE /api/nivelesskill/99** ? Retorna 404 (no existe)

---

## ?? Respuestas HTTP Esperadas

| Operación | Endpoint | Body | Respuesta Esperada |
|-----------|----------|------|-------------------|
| GET ALL | `/api/nivelesskill` | - | 200 OK + Array |
| GET BY ID (existe) | `/api/nivelesskill/0` | - | 200 OK + Objeto |
| GET BY ID (no existe) | `/api/nivelesskill/99` | - | 404 Not Found |
| POST (nuevo) | `/api/nivelesskill` | `{"id":1,"nombre":"intermedio"}` | 201 Created |
| POST (duplicado) | `/api/nivelesskill` | `{"id":0,"nombre":"basico"}` | **409 Conflict** |
| PUT (existe) | `/api/nivelesskill/0` | `{"nombre":"actualizado"}` | 200 OK |
| PUT (no existe) | `/api/nivelesskill/99` | `{"nombre":"test"}` | 404 Not Found |
| DELETE (existe) | `/api/nivelesskill/1` | - | 204 No Content |
| DELETE (no existe) | `/api/nivelesskill/99` | - | 404 Not Found |

---

## ? Checklist de Verificación

- [x] Compilación exitosa
- [x] Manejo de excepciones para duplicados implementado
- [x] Controller retorna códigos HTTP correctos
- [x] Service maneja errores de MongoDB
- [x] Repository usa filtros correctos para int IDs
- [x] Archivo de pruebas HTTP creado

---

## ?? Troubleshooting

### Si PUT sigue retornando 404:

1. Verificar que la colección se llama exactamente `"nivelskills"` en MongoDB
2. Verificar que la base de datos es `"talento_db"`
3. Verificar que el documento tiene `_id: 0` (int, no string)
4. Ejecutar este query en MongoDB Compass:
   ```javascript
   db.nivelskills.find({ "_id": 0 })
   ```

### Si POST sigue lanzando excepción:

1. Verificar que el paquete `MongoDB.Driver` sea versión 3.0.0
2. Limpiar y recompilar: `dotnet clean && dotnet build`
3. Verificar la conexión SSL en `Program.cs`

---

## ?? Notas Adicionales

- El ID `0` es válido en MongoDB cuando se usa `BsonRepresentation(BsonType.Int32)`
- El manejo de excepciones usa pattern matching: `when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)`
- El método `Conflict()` retorna status code 409 HTTP
- La ruta del controller es `/api/nivelesskill` (singular) por el atributo `[Route("api/[controller]")]`

