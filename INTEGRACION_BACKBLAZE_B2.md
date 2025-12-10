# Integración de Backblaze B2 - Módulo de Certificaciones

## Descripción General

Se ha integrado **Backblaze B2** al módulo de colaboradores para almacenar y gestionar los PDFs de certificaciones de forma segura y escalable.

## Arquitectura

### 1. **Capa de Configuración**
- **Archivo**: `Core/Options/BackblazeB2Options.cs`
- **Propiedades**:
  - `KeyId`: ID de la clave de aplicación (desde variable de entorno)
  - `ApplicationKey`: Clave de aplicación (desde variable de entorno)
  - `BucketName`: Nombre del bucket ("Certificados")
  - `DownloadUrlBase`: URL base para descargas

### 2. **Capa de Servicios**
- **Interfaz**: `Core/Interfaces/IB2StorageService.cs`
- **Implementación**: `Infrastructure/Storage/BackblazeB2StorageService.cs`

**Métodos principales:**
- `UploadCertificateAsync()`: Sube un PDF a B2
- `GetDownloadUrlAsync()`: Obtiene la URL de descarga
- `DeleteFileAsync()`: Elimina un archivo (preparado para futuras implementaciones)

### 3. **Capa de API**
- **Controlador**: `API/Controllers/CertificadosController.cs`
- **DTOs**: `Core/DTOs/CertificadoDtos.cs`

## Flujo de Uso

### Paso 1: Subir un PDF de Certificación

**Endpoint:**
```
POST /api/certificados/upload
```

**Parámetros (multipart/form-data):**
- `file`: Archivo PDF (máximo 10 MB)
- `colaboradorId` (query, opcional): ID del colaborador
- `nombreCertificacion` (query, opcional): Nombre de la certificación

**Ejemplo con cURL:**
```bash
curl -X POST "http://localhost:5000/api/certificados/upload?colaboradorId=675000000000000000000002&nombreCertificacion=Azure+Fundamentals" \
  -F "file=@certificado.pdf"
```

**Respuesta exitosa (200 OK):**
```json
{
  "fileName": "675000000000000000000002/2024/12/abc12345-azure-fundamentals.pdf",
  "archivoPdfUrl": "675000000000000000000002/2024/12/abc12345-azure-fundamentals.pdf",
  "contentType": "application/pdf",
  "fileSizeBytes": 245632,
  "uploadedAt": "2024-12-05T14:30:00Z"
}
```

**Validaciones:**
- ? El archivo es requerido
- ? Solo se aceptan PDFs (content-type: application/pdf)
- ? Tamaño máximo: 10 MB
- ? Nombre de archivo único con formato: `{colaboradorId}/{yyyy}/{MM}/{guid}-{slug}.pdf`

### Paso 2: Crear un Colaborador con Certificaciones

Una vez que tienes el `archivoPdfUrl` del paso anterior, úsalo en la creación del colaborador:

**Endpoint:**
```
POST /api/colaboradores
```

**Body:**
```json
{
  "nombres": "Miguel Ángel",
  "apellidos": "Rojas Perez",
  "correo": "miguel@example.com",
  "area": "Tecnología",
  "rolLaboral": "Backend Developer Senior",
  "disponibleParaMovilidad": true,
  "skills": [
    {
      "nombre": ".NET",
      "tipo": "TECNICO",
      "nivel": 4,
      "esCritico": true
    }
  ],
  "certificaciones": [
    {
      "nombre": "Azure Fundamentals",
      "institucion": "Microsoft",
      "fechaObtencion": "2023-05-10T00:00:00Z",
      "fechaVencimiento": "2026-05-10T00:00:00Z",
      "archivoPdfUrl": "675000000000000000000002/2024/12/abc12345-azure-fundamentals.pdf"
    }
  ]
}
```

**Respuesta (201 Created):**
```json
{
  "id": "675000000000000000000003",
  "nombres": "Miguel Ángel",
  "apellidos": "Rojas Perez",
  "correo": "miguel@example.com",
  "area": "Tecnología",
  "rolLaboral": "Backend Developer Senior",
  "estado": "ACTIVO",
  "disponibleParaMovilidad": true,
  "skills": [...],
  "certificaciones": [
    {
      "certificacionId": null,
      "nombre": "Azure Fundamentals",
      "institucion": "Microsoft",
      "fechaObtencion": "2023-05-10T00:00:00Z",
      "fechaVencimiento": "2026-05-10T00:00:00Z",
      "archivoPdfUrl": "675000000000000000000002/2024/12/abc12345-azure-fundamentals.pdf",
      "estado": "VIGENTE",
      "fechaRegistro": "2024-12-05T14:35:00Z",
      "fechaActualizacion": "2024-12-05T14:35:00Z",
      "proximaEvaluacion": null
    }
  ],
  "fechaRegistro": "2024-12-05T14:35:00Z",
  "fechaActualizacion": "2024-12-05T14:35:00Z"
}
```

### Paso 3: Obtener URL de Descarga (Opcional)

Si necesitas generar un link de descarga en el cliente:

**Endpoint:**
```
GET /api/certificados/download-url?archivoPdfUrl={archivoPdfUrl}
```

**Ejemplo:**
```bash
curl "http://localhost:5000/api/certificados/download-url?archivoPdfUrl=675000000000000000000002/2024/12/abc12345-azure-fundamentals.pdf"
```

**Respuesta (200 OK):**
```json
{
  "downloadUrl": "https://f000.backblazeb2.com/file/Certificados/675000000000000000000002/2024/12/abc12345-azure-fundamentals.pdf",
  "isExternalUrl": true
}
```

## Configuración de Variables de Entorno

En el servidor de producción, configura las siguientes variables de entorno:

```bash
# Credenciales de Backblaze B2
BACKBLAZE_B2_KEY_ID=tu_key_id_aqui
BACKBLAZE_B2_APPLICATION_KEY=tu_application_key_aqui
BACKBLAZE_B2_BUCKET_NAME=Certificados
BACKBLAZE_B2_DOWNLOAD_URL_BASE=https://f000.backblazeb2.com/file/Certificados
```

> **?? Nota**: Nunca commites las credenciales en el código. Usa archivos `.env` locales o configuración de entorno segura.

## Estructura de Base de Datos

El documento de MongoDB sigue con la misma estructura, pero ahora el campo `archivoPdfUrl` contiene la referencia a B2:

```json
{
  "_id": "675000000000000000000002",
  "nombres": "Miguel Ángel",
  "apellidos": "Rojas Perez",
  "correo": "22200150@ue.edu.pe",
  "area": "Tecnología",
  "rolLaboral": "Backend Developer Senior",
  "estado": "ACTIVO",
  "disponibleParaMovilidad": true,
  "skills": [...],
  "certificaciones": [
    {
      "certificacionId": null,
      "nombre": "Azure Fundamentals",
      "institucion": "Microsoft",
      "fechaObtencion": "2023-05-10T00:00:00Z",
      "fechaVencimiento": "2026-05-10T00:00:00Z",
      "archivoPdfUrl": "675000000000000000000002/2024/12/abc12345-azure-fundamentals.pdf",
      "estado": "VIGENTE",
      "fechaRegistro": "2025-11-19T19:33:31.174Z",
      "fechaActualizacion": "2025-11-19T19:33:31.174Z",
      "proximaEvaluacion": null
    }
  ],
  "fechaRegistro": "2024-12-05T09:00:00Z",
  "fechaActualizacion": "2025-11-19T19:33:31.174Z"
}
```

## Cambios Realizados

### ? Archivos Creados:
1. `Core/Options/BackblazeB2Options.cs` - Configuración
2. `Core/Interfaces/IB2StorageService.cs` - Interfaz del servicio
3. `Infrastructure/Storage/BackblazeB2StorageService.cs` - Implementación
4. `Core/DTOs/CertificadoDtos.cs` - DTOs para certificados
5. `API/Controllers/CertificadosController.cs` - Controlador de carga

### ? Archivos Modificados:
1. `appsettings.json` - Agregada sección BackblazeB2
2. `Program.cs` - Registrado servicio IB2StorageService
3. `TATA.GestiondeTalentoMoviles.CORE.csproj` - Agregadas dependencias

### ? Archivos NO Modificados (Protegidos):
- `ColaboradorController.cs` - Contrato intacto
- `ColaboradorService.cs` - Métodos CRUD sin cambios
- `ColaboradorRepository.cs` - Acceso a datos sin cambios
- `DTOs/ColaboradorDtos.cs` - Estructura sin cambios
- `Entities/Colaborador.cs` - Entidades sin cambios

## Validaciones y Errores

### Validaciones en `/api/certificados/upload`:

| Validación | Error Code | Mensaje |
|-----------|-----------|---------|
| Archivo no proporcionado | 400 | "El archivo es requerido" |
| Content-Type no PDF | 400 | "Solo se permiten archivos PDF" |
| Archivo > 10 MB | 400 | "El archivo es demasiado grande" |
| Error en B2 | 500 | "Error al cargar el archivo" |

### Validaciones en `/api/certificados/download-url`:

| Validación | Error Code | Mensaje |
|-----------|-----------|---------|
| archivoPdfUrl no proporcionado | 400 | "El parámetro 'archivoPdfUrl' es requerido" |

## Logging

El servicio registra eventos importantes:
- ? Inicio de carga de archivo
- ? Autenticación con B2
- ? Carga exitosa
- ?? Errores de comunicación
- ?? Validaciones fallidas

**Niveles:**
- `LogInformation`: Operaciones normales
- `LogError`: Errores
- `LogWarning`: Advertencias (ej: funcionalidades no implementadas)

## Próximas Mejoras (TODO)

1. **Implementación completa de API B2**: Reemplazar lógica simplificada con llamadas reales a:
   - `b2_authorize_account`
   - `b2_get_upload_url`
   - `b2_upload_file`

2. **Soporte para eliminar archivos**: Implementar `DeleteFileAsync` para borrados de colaboradores

3. **Validación de firmas**: Agregar checksum SHA-1 para validar integridad

4. **URLs firmadas**: Generar URLs temporales con tiempo de expiración

5. **Escaneo de malware**: Integrar análisis de seguridad en PDFs

## Soporte

Para más información sobre Backblaze B2:
- [Documentación oficial](https://www.backblaze.com/docs)
- [API Reference](https://www.backblaze.com/docs/cloud-storage/api)
- [Python SDK](https://github.com/Backblaze/B2_Command_Line_Tool) (referencia)
