using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IMongoCollection<Role> _rolesCollection;

        public RoleRepository(IMongoDatabase database)
        {
            _rolesCollection = database.GetCollection<Role>("roles");
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _rolesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(string id)
        {
            return await _rolesCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Role role)
        {
            await _rolesCollection.InsertOneAsync(role);
        }

        public async Task<bool> UpdateAsync(string id, Role role)
        {
            var result = await _rolesCollection.ReplaceOneAsync(r => r.Id == id, role);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _rolesCollection.DeleteOneAsync(r => r.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
