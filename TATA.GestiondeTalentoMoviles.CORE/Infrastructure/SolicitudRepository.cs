using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.Infrastructure.Repositories
{
    public class SolicitudRepository
    {
        private readonly IMongoCollection<SolicitudActualizacion> _collection;

        public SolicitudRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<SolicitudActualizacion>("solicitudes");
        }

        public async Task<List<SolicitudActualizacion>> GetAll()
            => await _collection.Find(_ => true).ToListAsync();

        public async Task<SolicitudActualizacion> GetById(string id)
            => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task Create(SolicitudActualizacion solicitud)
            => await _collection.InsertOneAsync(solicitud);

        public async Task Update(string id, SolicitudActualizacion model)
            => await _collection.ReplaceOneAsync(x => x.Id == id, model);

        public async Task Delete(string id)
            => await _collection.DeleteOneAsync(x => x.Id == id);
    }
}
