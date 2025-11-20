using System.Linq;
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

            // áreas y roles (arrays de strings)
            if (key == "areas")
            {
                catalogo.Areas = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data)) ?? new();
            }
            else if (key == "roles" || key == "roleslaborales")
            {
                catalogo.RolesLaborales = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data)) ?? new();
            }
            else if (key == "niveles" || key == "nivelesskill" || key == "nivelesskills")
            {
                catalogo.NivelesSkill = JsonConvert.DeserializeObject<List<NivelSkill>>(JsonConvert.SerializeObject(data)) ?? new();
            }
            else if (key == "tipos" || key == "tiposskill" || key == "tiposs")
            {
                catalogo.TiposSkill = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data)) ?? new();
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
                if (key == "areas")
                {
                    if (index < 0 || index >= catalogo.Areas.Count) return false;
                    catalogo.Areas.RemoveAt(index);
                }
                else if (key == "roles" || key == "roleslaborales")
                {
                    if (index < 0 || index >= catalogo.RolesLaborales.Count) return false;
                    catalogo.RolesLaborales.RemoveAt(index);
                }
                else if (key == "niveles" || key == "nivelesskill" || key == "nivelesskills")
                {
                    if (index < 0 || index >= catalogo.NivelesSkill.Count) return false;
                    catalogo.NivelesSkill.RemoveAt(index);
                }
                else if (key == "tipos" || key == "tiposskill" || key == "tiposs")
                {
                    if (index < 0 || index >= catalogo.TiposSkill.Count) return false;
                    catalogo.TiposSkill.RemoveAt(index);
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
            if (key == "areas" || key == "roleslaborales" || key == "roles" || key == "niveles" || key == "nivelesskill" || key == "nivelesskills" || key == "tipos" || key == "tiposskill" || key == "tiposs")
            {
                // no permitir crear sobre secciones fijas
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

            if (key == "areas")
            {
                var val = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(item));
                if (val != null) catalogo.Areas.Add(val);
            }
            else if (key == "roles" || key == "roleslaborales")
            {
                var val = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(item));
                if (val != null) catalogo.RolesLaborales.Add(val);
            }
            else if (key == "niveles" || key == "nivelesskill" || key == "nivelesskills")
            {
                var val = JsonConvert.DeserializeObject<NivelSkill>(JsonConvert.SerializeObject(item));
                if (val != null) catalogo.NivelesSkill.Add(val);
            }
            else if (key == "tipos" || key == "tiposskill" || key == "tiposs")
            {
                var val = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(item));
                if (val != null) catalogo.TiposSkill.Add(val);
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