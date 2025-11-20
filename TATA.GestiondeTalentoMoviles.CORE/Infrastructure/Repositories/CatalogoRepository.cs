using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using Newtonsoft.Json;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class CatalogoRepository : ICatalogoRepository
    {
        private readonly IMongoCollection<Catalogo> _collection;

        public CatalogoRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Catalogo>("catalogo");
        }

        public async Task<Catalogo?> GetAsync(string id) =>
            await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task<Catalogo?> GetFirstAsync() =>
            await _collection.Find(_ => true).FirstOrDefaultAsync();

        public async Task CreateOrReplaceAsync(Catalogo catalogo)
        {
            await _collection.ReplaceOneAsync(c => c.Id == catalogo.Id, catalogo, new ReplaceOptions { IsUpsert = true });
        }

        // helper para guardar sección dinámica como JSON string
        public static string SerializeSection(object? data) => JsonConvert.SerializeObject(data ?? new object());
        public static T? DeserializeSection<T>(string json) => JsonConvert.DeserializeObject<T?>(json);
    }
}