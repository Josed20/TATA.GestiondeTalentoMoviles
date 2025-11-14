using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("users");
        }

        public async Task<User> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var res = await _users.DeleteOneAsync(u => u.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByNombreApellidoAsync(string nombre, string apellido)
        {
            return await _users.Find(u => u.Nombre == nombre && u.Apellido == apellido).FirstOrDefaultAsync();
        }

        public async Task<User?> UpdateAsync(string id, User user)
        {
            var res = await _users.ReplaceOneAsync(u => u.Id == id, user);
            if (res.ModifiedCount > 0) return user;
            return null;
        }
    }
}