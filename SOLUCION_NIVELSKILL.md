# Implementación Completa de NivelSkill con ObjectId

## ?? Cambios Implementados

### **Esquema de MongoDB Actualizado**

La colección `nivelskills` ahora usa el siguiente esquema:

```javascript
{
  $jsonSchema: {
    bsonType: 'object',
    required: ['codigo', 'nombre'],
    properties: {
      _id: {
        bsonType: 'objectId',
        description: 'ID autogenerado por MongoDB'
      },
      codigo: {
        bsonType: 'int',
        description: 'Código del nivel (0=no iniciado, 1=básico, 2=intermedio, 3=avanzado)'
      },
      nombre: {
        bsonType: 'string',
        description: 'Nombre del nivel'
      }
    }
  }
}
```

---

## ?? Archivos Modificados

### **1. Entidad `NivelSkill.cs`** ? Ya estaba correcto

```csharp
public class NivelSkill
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }  // ObjectId como string

    [BsonElement("codigo")]
    public int Codigo { get; set; }  // 0, 1, 2, 3...

    [BsonElement("nombre")]
    public string Nombre { get; set; } = null!;
}
```

---

### **2. DTOs `NivelSkillDtos.cs`** ? Ya estaban correctos

```csharp
// Para crear (POST) - Solo envías codigo y nombre
public class NivelSkillCreateDto
{
    public int Codigo { get; set; }
    public string Nombre { get; set; } = null!;
}

// Para actualizar (PUT) - Solo envías codigo y nombre
public class NivelSkillUpdateDto
{
    public int Codigo { get; set; }
    public string Nombre { get; set; } = null!;
}

// Para leer (respuestas GET/POST/PUT) - Incluye el Id generado
public class NivelSkillReadDto
{
    public string Id { get; set; } = null!;  // ObjectId como string
    public int Codigo { get; set; }
    public string Nombre { get; set; } = null!;
}
```

---

### **3. Interfaz `INivelSkillService.cs`** ? ACTUALIZADO

**Cambios:**
- `GetByIdAsync(int id)` ? `GetByIdAsync(string id)`
- `UpdateAsync(int id, ...)` ? `UpdateAsync(string id, ...)`
- `DeleteAsync(int id)` ? `DeleteAsync(string id)`

```csharp
public interface INivelSkillService
{
    Task<NivelSkillReadDto> CreateAsync(NivelSkillCreateDto createDto);
    Task<IEnumerable<NivelSkillReadDto>> GetAllAsync();
    Task<NivelSkillReadDto?> GetByIdAsync(string id);        // ? Cambiado a string
    Task<NivelSkillReadDto?> UpdateAsync(string id, NivelSkillUpdateDto updateDto);  // ? Cambiado
    Task<bool> DeleteAsync(string id);                        // ? Cambiado a string
}
```

---

### **4. Servicio `NivelSkillService.cs`** ? ACTUALIZADO

**Cambios principales:**

#### **CreateAsync** - No establece Id, MongoDB lo genera:
```csharp
public async Task<NivelSkillReadDto> CreateAsync(NivelSkillCreateDto createDto)
{
    var nivel = new NivelSkill
    {
        Codigo = createDto.Codigo,
        Nombre = createDto.Nombre
        // ? Id se deja null, MongoDB lo generará automáticamente
    };

    try
    {
        var nuevoNivel = await _repository.CreateAsync(nivel);
        return MapToReadDto(nuevoNivel);  // Devuelve con Id generado
    }
    catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
    {
        throw new InvalidOperationException($"Ya existe un nivel de skill con código {createDto.Codigo}", ex);
    }
}
```

#### **UpdateAsync** - Usa string id:
```csharp
public async Task<NivelSkillReadDto?> UpdateAsync(string id, NivelSkillUpdateDto updateDto)
{
    var existente = await _repository.GetByIdAsync(id);
    if (existente == null) return null;

    var nivelActualizado = new NivelSkill
    {
        Id = id,                      // ? Usa el ObjectId de la URL
        Codigo = updateDto.Codigo,    // ? Actualiza código
        Nombre = updateDto.Nombre     // ? Actualiza nombre
    };

    var ok = await _repository.UpdateAsync(id, nivelActualizado);
    if (!ok) return null;

    return MapToReadDto(nivelActualizado);
}
```

#### **MapToReadDto** - Mapea todos los campos:
```csharp
private static NivelSkillReadDto MapToReadDto(NivelSkill n)
{
    return new NivelSkillReadDto
    {
        Id = n.Id!,           // ? ObjectId
        Codigo = n.Codigo,    // ? Código del nivel
        Nombre = n.Nombre     // ? Nombre del nivel
    };
}
```

---

### **5. Repositorio `NivelSkillRepository.cs`** ? Ya estaba correcto

```csharp
public class NivelSkillRepository : INivelSkillRepository
{
    private readonly IMongoCollection<NivelSkill> _nivelesSkill;

    public NivelSkillRepository(IMongoDatabase database)
    {
        _nivelesSkill = database.GetCollection<NivelSkill>("nivelskills");
    }

    public async Task<NivelSkill> CreateAsync(NivelSkill nivelSkill)
    {
        // ? No establecer Id, dejar que MongoDB lo genere automáticamente
        await _nivelesSkill.InsertOneAsync(nivelSkill);
        return nivelSkill; // MongoDB habrá asignado el Id automáticamente
    }

    public async Task<NivelSkill?> GetByIdAsync(string id)
    {
        return await _nivelesSkill
            .Find(n => n.Id == id)  // ? Busca por ObjectId (string)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAsync(string id, NivelSkill nivelSkill)
    {
        nivelSkill.Id = id;  // ? Asegura que el Id coincida

        var result = await _nivelesSkill.ReplaceOneAsync(
            n => n.Id == id,
            nivelSkill,
            new ReplaceOptions { IsUpsert = false }
        );

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _nivelesSkill.DeleteOneAsync(n => n.Id == id);
        return result.DeletedCount > 0;
    }
}
```

---

### **6. Controller `NivelesSkillController.cs`** ? ACTUALIZADO

**Cambios:**
- `{id:int}` ? `{id}` (ahora acepta strings)
- `int id` ? `string id` en todos los métodos

```csharp
[Route("api/[controller]")]
[ApiController]
public class NivelesSkillController : ControllerBase
{
    // GET: api/NivelesSkill
    [HttpGet]
    public async Task<IActionResult> GetAll() { ... }

    // GET: api/NivelesSkill/6736c2b4f2a0455da3041942
    [HttpGet("{id}")]  // ? Sin restricción :int
    public async Task<IActionResult> GetById(string id) { ... }  // ? string id

    // POST: api/NivelesSkill
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NivelSkillCreateDto createDto)
    {
        var nuevoNivel = await _service.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = nuevoNivel.Id }, nuevoNivel);
        // ? Devuelve 201 Created con Location header y el objeto con Id
    }

    // PUT: api/NivelesSkill/6736c2b4f2a0455da3041942
    [HttpPut("{id}")]  // ? Sin restricción :int
    public async Task<IActionResult> Update(string id, [FromBody] NivelSkillUpdateDto updateDto)
    {
        var nivelActualizado = await _service.UpdateAsync(id, updateDto);
        if (nivelActualizado == null)
        {
            return NotFound(new { message = $"Nivel de skill con ID '{id}' no encontrado" });
        }
        return Ok(nivelActualizado);  // ? Devuelve 200 OK con objeto actualizado
    }

    // DELETE: api/NivelesSkill/6736c2b4f2a0455da3041942
    [HttpDelete("{id}")]  // ? Sin restricción :int
    public async Task<IActionResult> Delete(string id)
    {
        var eliminado = await _service.DeleteAsync(id);
        if (!eliminado)
        {
            return NotFound(new { message = $"Nivel de skill con ID '{id}' no encontrado" });
        }
        return NoContent();  // ? Devuelve 204 No Content
    }
}
```

---

## ?? Ejemplos de Uso

### **1. POST - Crear un nuevo nivel**

**Request:**
```http
POST /api/NivelesSkill
Content-Type: application/json

{
  "codigo": 0,
  "nombre": "no iniciado"
}
```

**Response:** `201 Created`
```json
{
  "id": "6736c2b4f2a0455da3041942",
  "codigo": 0,
  "nombre": "no iniciado"
}
```

**Headers:**
```
Location: https://localhost:5001/api/NivelesSkill/6736c2b4f2a0455da3041942
```

---

### **2. GET - Obtener todos los niveles**

**Request:**
```http
GET /api/NivelesSkill
```

**Response:** `200 OK`
```json
[
  {
    "id": "6736c2b4f2a0455da3041942",
    "codigo": 0,
    "nombre": "no iniciado"
  },
  {
    "id": "6736c2b5f2a0455da3041943",
    "codigo": 1,
    "nombre": "basico"
  }
]
```

---

### **3. GET BY ID - Obtener un nivel específico**

**Request:**
```http
GET /api/NivelesSkill/6736c2b4f2a0455da3041942
```

**Response:** `200 OK`
```json
{
  "id": "6736c2b4f2a0455da3041942",
  "codigo": 0,
  "nombre": "no iniciado"
}
```

---

### **4. PUT - Actualizar un nivel existente**

**Request:**
```http
PUT /api/NivelesSkill/6736c2b4f2a0455da3041942
Content-Type: application/json

{
  "codigo": 1,
  "nombre": "básico"
}
```

**Response:** `200 OK`
```json
{
  "id": "6736c2b4f2a0455da3041942",
  "codigo": 1,
  "nombre": "básico"
}
```

---

### **5. DELETE - Eliminar un nivel**

**Request:**
```http
DELETE /api/NivelesSkill/6736c2b4f2a0455da3041942
```

**Response:** `204 No Content`

---

## ?? Tabla de Respuestas HTTP

| Operación | Endpoint | Body | Respuesta Exitosa | Respuesta Error |
|-----------|----------|------|-------------------|-----------------|
| GET ALL | `/api/NivelesSkill` | - | `200 OK` + Array | - |
| GET BY ID | `/api/NivelesSkill/{objectId}` | - | `200 OK` + Objeto | `404 Not Found` |
| POST | `/api/NivelesSkill` | `{"codigo":0,"nombre":"..."}` | `201 Created` + Objeto con Id | `409 Conflict` (duplicado) |
| PUT | `/api/NivelesSkill/{objectId}` | `{"codigo":1,"nombre":"..."}` | `200 OK` + Objeto actualizado | `404 Not Found` |
| DELETE | `/api/NivelesSkill/{objectId}` | - | `204 No Content` | `404 Not Found` |

---

## ? Verificación

- [x] Entidad usa `ObjectId` como string
- [x] DTOs no requieren `Id` en Create/Update
- [x] Repository maneja `string id` correctamente
- [x] Service mapea `Codigo` y `Nombre` correctamente
- [x] Controller acepta `string id` en rutas
- [x] POST devuelve el `Id` generado por MongoDB
- [x] PUT actualiza ambos campos: `codigo` y `nombre`
- [x] DELETE verifica existencia antes de eliminar
- [x] Compilación exitosa
- [x] Archivo de pruebas HTTP actualizado

---

## ?? Troubleshooting

### Si PUT/GET devuelve 404:
1. Verifica que estás usando el **ObjectId correcto** (24 caracteres hexadecimales)
2. Copia el `id` exacto de la respuesta del POST
3. Verifica la conexión a MongoDB Atlas (revisar `appsettings.json`)

### Si POST falla:
1. Verifica que los campos `codigo` y `nombre` estén en el body
2. Verifica que `codigo` sea un número entero (int)
3. Verifica la conexión SSL en `Program.cs`

---

## ?? Notas Importantes

- ? El `Id` es generado **automáticamente** por MongoDB
- ? El POST **no requiere** enviar `id` en el body
- ? El `codigo` puede repetirse (no hay índice único en ese campo)
- ? El `_id` es único por defecto en MongoDB
- ? Las rutas usan `/api/NivelesSkill` (plural, con mayúscula)

