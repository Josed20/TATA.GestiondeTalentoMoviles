using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class ColaboradorService : IColaboradorService
    {
        private readonly IColaboradorRepository _repository;

        public ColaboradorService(IColaboradorRepository repository)
        {
            _repository = repository;
        }

        public async Task<ColaboradorReadDto> CreateAsync(ColaboradorCreateDto createDto)
        {
            // Mapear CreateDto -> Entidad
            var colaborador = MapCreateDtoToEntity(createDto);

            // Crear en repositorio
            var nuevoColaborador = await _repository.CreateAsync(colaborador);

            // Entidad -> ReadDto
            return MapEntityToReadDto(nuevoColaborador);
        }

        public async Task<IEnumerable<ColaboradorReadDto>> GetAllAsync()
        {
            var colaboradores = await _repository.GetAllAsync();
            return colaboradores.Select(MapEntityToReadDto).ToList();
        }

        public async Task<ColaboradorReadDto?> GetByIdAsync(string id)
        {
            var colaborador = await _repository.GetByIdAsync(id);
            if (colaborador == null) return null;

            return MapEntityToReadDto(colaborador);
        }

        public async Task<ColaboradorReadDto?> UpdateAsync(string id, ColaboradorUpdateDto updateDto)
        {
            // Verificar si el colaborador existe
            var colaboradorExistente = await _repository.GetByIdAsync(id);
            if (colaboradorExistente == null) return null;

            // Mapear UpdateDto -> Entidad
            var colaboradorActualizado = MapUpdateDtoToEntity(id, updateDto);

            // Actualizar en repositorio
            var actualizado = await _repository.UpdateAsync(id, colaboradorActualizado);
            
            if (!actualizado) return null;

            // Entidad -> ReadDto
            return MapEntityToReadDto(colaboradorActualizado);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            // Verificar si el colaborador existe
            var colaboradorExistente = await _repository.GetByIdAsync(id);
            if (colaboradorExistente == null) return false;

            // Borrado lógico: marcar como Inactivo
            return await _repository.DeleteAsync(id);
        }

        // ====================================
        // Métodos privados de mapeo
        // ====================================

        private static Colaborador MapCreateDtoToEntity(ColaboradorCreateDto dto)
        {
            return new Colaborador
            {
                // Id se deja null, MongoDB lo generará automáticamente
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Area = dto.Area,
                RolActual = dto.RolActual,
                Skills = dto.Skills,
                NivelCodigo = dto.NivelCodigo,
                Certificaciones = dto.Certificaciones.Select(c => new CertificacionColaborador
                {
                    Nombre = c.Nombre,
                    ImagenUrl = c.ImagenUrl,
                    FechaObtencion = c.FechaObtencion,
                    Estado = "vigente" // ✅ Seteado automáticamente en backend
                }).ToList(),
                Disponibilidad = new DisponibilidadColaborador
                {
                    Estado = dto.Disponibilidad.Estado,
                    Dias = dto.Disponibilidad.Dias
                }
            };
        }

        private static Colaborador MapUpdateDtoToEntity(string id, ColaboradorUpdateDto dto)
        {
            return new Colaborador
            {
                Id = id, // ✅ Mantener el mismo ID
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Area = dto.Area,
                RolActual = dto.RolActual,
                Skills = dto.Skills,
                NivelCodigo = dto.NivelCodigo,
                Certificaciones = dto.Certificaciones.Select(c => new CertificacionColaborador
                {
                    Nombre = c.Nombre,
                    ImagenUrl = c.ImagenUrl,
                    FechaObtencion = c.FechaObtencion,
                    Estado = "vigente" // ✅ Seteado automáticamente en backend
                }).ToList(),
                Disponibilidad = new DisponibilidadColaborador
                {
                    Estado = dto.Disponibilidad.Estado,
                    Dias = dto.Disponibilidad.Dias
                }
            };
        }

        private static ColaboradorReadDto MapEntityToReadDto(Colaborador c)
        {
            return new ColaboradorReadDto
            {
                Id = c.Id!,
                Nombres = c.Nombres,
                Apellidos = c.Apellidos,
                Area = c.Area,
                RolActual = c.RolActual,
                Skills = c.Skills,
                NivelCodigo = c.NivelCodigo,
                Certificaciones = c.Certificaciones.Select(cert => new CertificacionReadDto
                {
                    Nombre = cert.Nombre,
                    ImagenUrl = cert.ImagenUrl,
                    FechaObtencion = cert.FechaObtencion,
                    Estado = cert.Estado
                }).ToList(),
                Disponibilidad = new DisponibilidadDto
                {
                    Estado = c.Disponibilidad.Estado,
                    Dias = c.Disponibilidad.Dias
                }
            };
        }
    }
}
