using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class SolicitudRepository : ISolicitudRepository
    {
        private readonly IMongoCollection<SolicitudActualizacion> _collection;

        public SolicitudRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<SolicitudActualizacion>("solicitudes");
        }

        public async Task<SolicitudActualizacion> Create(SolicitudActualizacion solicitud)
        {
            await _collection.InsertOneAsync(solicitud);
            return solicitud;
        }

        public async Task<List<SolicitudActualizacion>> GetByColaborador(string colaboradorId)
        {
            return await _collection
                .Find(s => s.ColaboradorId == colaboradorId)
                .ToListAsync();
        }

        public async Task<SolicitudActualizacion> UpdateEstado(string id, string estado, string comentarios)
        {
            var update = Builders<SolicitudActualizacion>.Update
                .Set(s => s.Estado, estado)
                .Set(s => s.ComentariosRRHH, comentarios)
                .Set(s => s.FechaRevision, DateTime.UtcNow);

            return await _collection.FindOneAndUpdateAsync(
                Builders<SolicitudActualizacion>.Filter.Eq(s => s.Id, id),
                update,
                new FindOneAndUpdateOptions<SolicitudActualizacion>
                {
                    ReturnDocument = ReturnDocument.After
                }
            );
        }
    }
}
