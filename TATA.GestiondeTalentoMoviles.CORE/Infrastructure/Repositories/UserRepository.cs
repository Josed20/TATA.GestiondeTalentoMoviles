using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("usuarios");
        }

        public async Task<User> GetByUsernameAsync(string username) =>
            await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task<User> GetByIdAsync(string id) =>
            await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _users.Find(u => true).ToListAsync();

        public async Task CreateAsync(User user) =>
            await _users.InsertOneAsync(user);

        public async Task UpdateAsync(string id, User userIn) =>
            await _users.ReplaceOneAsync(u => u.Id == id, userIn);

        public async Task DeleteAsync(string id) =>
            await _users.DeleteOneAsync(u => u.Id == id);
    }
}
