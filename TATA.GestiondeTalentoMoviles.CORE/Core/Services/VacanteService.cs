using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class VacanteService : IVacanteService
    {
        private readonly IVacanteRepository _repository;

        public VacanteService(IVacanteRepository repository)
        {
            _repository = repository;
        }

        public async Task<VacanteReadDto> CreateAsync(VacanteCreateDto createDto)
        {
            var vacante = new Vacante
            {
                PerfilName = createDto.PerfilName,
                IdArea = createDto.IdArea,
                StartDate = createDto.StartDate,
                Urgency = createDto.Urgency,
                State = createDto.State,
                IdRol = createDto.IdRol,
                Certifications = createDto.Certifications,
                Skills = createDto.Skills
            };

            var nuevaVacante = await _repository.CreateAsync(vacante);

            return new VacanteReadDto
            {
                Id = nuevaVacante.Id!,
                PerfilName = nuevaVacante.PerfilName,
                IdArea = nuevaVacante.IdArea,
                StartDate = nuevaVacante.StartDate,
                Urgency = nuevaVacante.Urgency,
                State = nuevaVacante.State,
                IdRol = nuevaVacante.IdRol,
                Certifications = nuevaVacante.Certifications,
                Skills = nuevaVacante.Skills
            };
        }

        public async Task<IEnumerable<VacanteReadDto>> GetAllAsync()
        {
            var vacantes = await _repository.GetAllAsync();

            return vacantes.Select(v => new VacanteReadDto
            {
                Id = v.Id!,
                PerfilName = v.PerfilName,
                IdArea = v.IdArea,
                StartDate = v.StartDate,
                Urgency = v.Urgency,
                State = v.State,
                IdRol = v.IdRol,
                Certifications = v.Certifications,
                Skills = v.Skills
            }).ToList();
        }

        public async Task<VacanteReadDto?> GetByIdAsync(string id)
        {
            var v = await _repository.GetByIdAsync(id);
            if (v == null) return null;

            return new VacanteReadDto
            {
                Id = v.Id!,
                PerfilName = v.PerfilName,
                IdArea = v.IdArea,
                StartDate = v.StartDate,
                Urgency = v.Urgency,
                State = v.State,
                IdRol = v.IdRol,
                Certifications = v.Certifications,
                Skills = v.Skills
            };
        }

        public async Task<VacanteReadDto?> UpdateAsync(string id, VacanteCreateDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.PerfilName = updateDto.PerfilName;
            existing.IdArea = updateDto.IdArea;
            existing.StartDate = updateDto.StartDate;
            existing.Urgency = updateDto.Urgency;
            existing.State = updateDto.State;
            existing.IdRol = updateDto.IdRol;
            existing.Certifications = updateDto.Certifications;
            existing.Skills = updateDto.Skills;

            var updated = await _repository.UpdateAsync(id, existing);
            if (updated == null) return null;

            return new VacanteReadDto
            {
                Id = updated.Id!,
                PerfilName = updated.PerfilName,
                IdArea = updated.IdArea,
                StartDate = updated.StartDate,
                Urgency = updated.Urgency,
                State = updated.State,
                IdRol = updated.IdRol,
                Certifications = updated.Certifications,
                Skills = updated.Skills
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
