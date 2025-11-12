using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities; // agregar Entities para usar Colaborador

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
            var colaborador = new Colaborador
            {
                Nombres = createDto.Nombres,
                Apellidos = createDto.Apellidos,
                Area = createDto.Area,
                RolActual = createDto.RolActual,
                SkillPrimario = createDto.SkillPrimario,
                SkillSecundario = createDto.SkillSecundario,
                NivelDominio = createDto.NivelDominio,
                Certificaciones = createDto.Certificaciones,
                Disponibilidad = createDto.Disponibilidad,
                DiasDisponibilidad = createDto.DiasDisponibilidad
            };

            var nuevoColaborador = await _repository.CreateAsync(colaborador);

            return new ColaboradorReadDto
            {
                Id = nuevoColaborador.Id!,
                Nombres = nuevoColaborador.Nombres,
                Apellidos = nuevoColaborador.Apellidos,
                Area = nuevoColaborador.Area,
                RolActual = nuevoColaborador.RolActual,
                SkillPrimario = nuevoColaborador.SkillPrimario,
                SkillSecundario = nuevoColaborador.SkillSecundario,
                NivelDominio = nuevoColaborador.NivelDominio,
                Certificaciones = nuevoColaborador.Certificaciones,
                Disponibilidad = nuevoColaborador.Disponibilidad,
                DiasDisponibilidad = nuevoColaborador.DiasDisponibilidad
            };
        }

        public async Task<IEnumerable<ColaboradorReadDto>> GetAllAsync()
        {
            var colaboradores = await _repository.GetAllAsync();

            var readDtos = colaboradores.Select(c => new ColaboradorReadDto
            {
                Id = c.Id!,
                Nombres = c.Nombres,
                Apellidos = c.Apellidos,
                Area = c.Area,
                RolActual = c.RolActual,
                SkillPrimario = c.SkillPrimario,
                SkillSecundario = c.SkillSecundario,
                NivelDominio = c.NivelDominio,
                Certificaciones = c.Certificaciones,
                Disponibilidad = c.Disponibilidad,
                DiasDisponibilidad = c.DiasDisponibilidad
            }).ToList();

            return readDtos;
        }

        public async Task<ColaboradorReadDto?> GetByIdAsync(string id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;

            return new ColaboradorReadDto
            {
                Id = c.Id!,
                Nombres = c.Nombres,
                Apellidos = c.Apellidos,
                Area = c.Area,
                RolActual = c.RolActual,
                SkillPrimario = c.SkillPrimario,
                SkillSecundario = c.SkillSecundario,
                NivelDominio = c.NivelDominio,
                Certificaciones = c.Certificaciones,
                Disponibilidad = c.Disponibilidad,
                DiasDisponibilidad = c.DiasDisponibilidad
            };
        }
    }
}
