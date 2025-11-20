using System.Linq;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly ICatalogoRepository _repo;

        public CatalogoService(ICatalogoRepository repo)
        {
            _repo = repo;
        }

        public async Task<CatalogoReadDto?> GetAsync(string id)
        {
            var c = await _repo.GetAsync(id);
            if (c == null) return null;
            return Map(c);
        }

        public async Task<CatalogoReadDto?> GetFirstAsync()
        {
            var c = await _repo.GetFirstAsync();
            if (c == null) return null;
            return Map(c);
        }

        // helper: try find a native property on Catalogo that matches the provided section name
        private static PropertyInfo? FindNativeProperty(string seccion)
        {
            var key = seccion.ToLowerInvariant();
            var props = typeof(Catalogo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            // 1) exact match
            var exact = props.FirstOrDefault(p => string.Equals(p.Name, seccion, System.StringComparison.OrdinalIgnoreCase));
            if (exact != null) return exact;

            // 2) contains match (so 'roles' matches 'RolesLaborales', 'niveles' matches 'NivelesSkill')
            var contains = props.FirstOrDefault(p => p.Name.ToLowerInvariant().Contains(key));
            if (contains != null) return contains;

            // 3) reverse contains (seccion contains property name)
            var rev = props.FirstOrDefault(p => key.Contains(p.Name.ToLowerInvariant()));
            if (rev != null) return rev;

            return null;
        }

        public async Task<CatalogoReadDto> UpdateAsync(string seccion, object data)
        {
            // Obtener catalogo global
            var catalogo = await _repo.GetFirstAsync();
            if (catalogo == null)
            {
                // crear uno nuevo
                catalogo = new Catalogo { Id = "catalogos_globales" };
            }

            var key = seccion.ToLowerInvariant();

            // Intentar resolver propiedad nativa mediante reflexión
            var prop = FindNativeProperty(seccion);
            if (prop != null && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
            {
                // Deserializar directamente al tipo de la propiedad (p. ej. List<string> o List<NivelSkill>)
                var json = JsonConvert.SerializeObject(data);
                var deserialized = JsonConvert.DeserializeObject(json, prop.PropertyType);
                if (deserialized != null)
                {
                    prop.SetValue(catalogo, deserialized);
                }
                else
                {
                    // si la deserialización falla, aseguramos lista vacía compatible
                    var empty = Activator.CreateInstance(prop.PropertyType);
                    prop.SetValue(catalogo, empty);
                }
            }
            else
            {
                // Sección dinámica: almacenar JSON en dictionary
                var json = JsonConvert.SerializeObject(data);
                if (catalogo.AdditionalSections == null)
                    catalogo.AdditionalSections = new();
                catalogo.AdditionalSections[key] = json;
            }

            await _repo.CreateOrReplaceAsync(catalogo);
            return Map(catalogo);
        }

        public async Task<bool> DeleteIndexAsync(string seccion, int index)
        {
            var catalogo = await _repo.GetFirstAsync();
            if (catalogo == null) return false;
            var key = seccion.ToLowerInvariant();

            try
            {
                var prop = FindNativeProperty(seccion);
                if (prop != null && typeof(IList).IsAssignableFrom(prop.PropertyType))
                {
                    var list = prop.GetValue(catalogo) as IList;
                    if (list == null) return false;
                    if (index < 0 || index >= list.Count) return false;
                    list.RemoveAt(index);
                }
                else
                {
                    if (catalogo.AdditionalSections == null || !catalogo.AdditionalSections.ContainsKey(key)) return false;
                    var arr = JsonConvert.DeserializeObject<List<object>>(catalogo.AdditionalSections[key]) ?? new();
                    if (index < 0 || index >= arr.Count) return false;
                    arr.RemoveAt(index);
                    catalogo.AdditionalSections[key] = JsonConvert.SerializeObject(arr);
                }

                await _repo.CreateOrReplaceAsync(catalogo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CatalogoReadDto> CreateSectionAsync(CatalogoCreateSectionDto dto)
        {
            var catalogo = await _repo.GetFirstAsync();
            if (catalogo == null)
            {
                catalogo = new Catalogo { Id = "catalogos_globales" };
            }

            var key = dto.NombreSeccion.ToLowerInvariant();

            // Si existe una propiedad nativa que coincida, no permitir creación
            var prop = FindNativeProperty(dto.NombreSeccion);
            if (prop != null)
            {
                throw new InvalidOperationException("No se puede crear una sección que ya existe como sección nativa");
            }

            var json = JsonConvert.SerializeObject(dto.Data);
            if (catalogo.AdditionalSections == null)
                catalogo.AdditionalSections = new();
            catalogo.AdditionalSections[key] = json;

            await _repo.CreateOrReplaceAsync(catalogo);
            return Map(catalogo);
        }

        public async Task<CatalogoReadDto> AddItemToSectionAsync(string seccion, object item)
        {
            var catalogo = await _repo.GetFirstAsync();
            if (catalogo == null)
                catalogo = new Catalogo { Id = "catalogos_globales" };

            var key = seccion.ToLowerInvariant();
            var prop = FindNativeProperty(seccion);

            if (prop != null && typeof(IList).IsAssignableFrom(prop.PropertyType))
            {
                // obtener la lista existente (o crear una nueva si null)
                var list = prop.GetValue(catalogo) as IList;
                if (list == null)
                {
                    var instance = Activator.CreateInstance(prop.PropertyType) as IList;
                    if (instance == null)
                    {
                        throw new InvalidOperationException("La sección nativa no es una lista manipulable");
                    }
                    prop.SetValue(catalogo, instance);
                    list = instance;
                }

                // Deserializar el item al tipo de elemento de la lista
                var elementType = prop.PropertyType.IsGenericType ? prop.PropertyType.GetGenericArguments()[0] : typeof(object);
                var jsonItem = JsonConvert.SerializeObject(item);
                var deserializedItem = JsonConvert.DeserializeObject(jsonItem, elementType);
                if (deserializedItem != null)
                {
                    list.Add(deserializedItem);
                }
            }
            else
            {
                var arr = new List<object>();
                if (catalogo.AdditionalSections != null && catalogo.AdditionalSections.ContainsKey(key))
                {
                    arr = JsonConvert.DeserializeObject<List<object>>(catalogo.AdditionalSections[key]) ?? new();
                }
                arr.Add(item);
                if (catalogo.AdditionalSections == null)
                    catalogo.AdditionalSections = new();
                catalogo.AdditionalSections[key] = JsonConvert.SerializeObject(arr);
            }

            await _repo.CreateOrReplaceAsync(catalogo);
            return Map(catalogo);
        }

        private static CatalogoReadDto Map(Catalogo c)
        {
            return new CatalogoReadDto
            {
                Id = c.Id,
                Areas = c.Areas,
                RolesLaborales = c.RolesLaborales,
                NivelesSkill = c.NivelesSkill.Select(n => new NivelSkillDto { Codigo = n.Codigo, Descripcion = n.Descripcion }).ToList(),
                TiposSkill = c.TiposSkill,
                AdditionalSections = (c.AdditionalSections ?? new()).ToDictionary(k => k.Key, v => (object)JsonConvert.DeserializeObject<object>(v.Value)!)
            };
        }
    }
}