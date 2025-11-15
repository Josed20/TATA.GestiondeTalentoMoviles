using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly IMongoCollection<Rol> _roles;

        public RolRepository(IMongoDatabase database)
        {
            _roles = database.GetCollection<Rol>("roles");
        }

        public async Task<Rol> CreateAsync(Rol rol)
        {
            await _roles.InsertOneAsync(rol);
            return rol;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var res = await _roles.DeleteOneAsync(r => r.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _roles.Find(_ => true).ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(string id)
        {
            return await _roles.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Rol?> GetByNombreAsync(string nombre)
        {
            return await _roles.Find(r => r.Nombre == nombre).FirstOrDefaultAsync();
        }

        public async Task<Rol?> UpdateAsync(string id, Rol rol)
        {
            var res = await _roles.ReplaceOneAsync(r => r.Id == id, rol);
            if (res.ModifiedCount > 0) return rol;
            return null;
        }
    }
}