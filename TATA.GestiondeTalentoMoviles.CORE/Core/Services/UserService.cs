using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<UserReadDto> CreateAsync(UserCreateDto dto)
        {
            var now = DateTime.UtcNow;
            var user = new User
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                CreatedAt = now,
                UpdatedAt = now,
                Estado = dto.Estado
            };

            var created = await _repo.CreateAsync(user);

            return new UserReadDto
            {
                Id = created.Id!,
                Nombre = created.Nombre,
                Apellido = created.Apellido,
                Email = created.Email,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt,
                Estado = created.Estado
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(u => new UserReadDto
            {
                Id = u.Id!,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Estado = u.Estado
            }).ToList();
        }

        public async Task<UserReadDto?> GetByIdAsync(string id)
        {
            var u = await _repo.GetByIdAsync(id);
            if (u == null) return null;
            return new UserReadDto
            {
                Id = u.Id!,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Estado = u.Estado
            };
        }

        public async Task<UserReadDto?> GetByNombreApellidoAsync(string nombre, string apellido)
        {
            var u = await _repo.GetByNombreApellidoAsync(nombre, apellido);
            if (u == null) return null;
            return new UserReadDto
            {
                Id = u.Id!,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Estado = u.Estado
            };
        }

        public async Task<UserReadDto?> UpdateAsync(string id, UserCreateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            existing.Nombre = dto.Nombre;
            existing.Apellido = dto.Apellido;
            existing.Email = dto.Email;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.Estado = dto.Estado;

            var updated = await _repo.UpdateAsync(id, existing);
            if (updated == null) return null;

            return new UserReadDto
            {
                Id = updated.Id!,
                Nombre = updated.Nombre,
                Apellido = updated.Apellido,
                Email = updated.Email,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt,
                Estado = updated.Estado
            };
        }
    }
}