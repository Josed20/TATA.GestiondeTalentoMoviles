using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class AlertaRepository : IAlertaRepository
    {
        private readonly IMongoCollection<Alerta> _alertas;

        public AlertaRepository(IMongoDatabase database)
        {
            _alertas = database.GetCollection<Alerta>("alertas");
        }

        public async Task<Alerta> GetByIdAsync(string id) =>
            await _alertas.Find(a => a.Id == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Alerta>> GetAllAsync() =>
            await _alertas.Find(a => true).ToListAsync();

        public async Task CreateAsync(Alerta alerta) =>
            await _alertas.InsertOneAsync(alerta);

        public async Task UpdateAsync(string id, Alerta alertaIn) =>
            await _alertas.ReplaceOneAsync(a => a.Id == id, alertaIn);

        public async Task DeleteAsync(string id) =>
            await _alertas.DeleteOneAsync(a => a.Id == id);
    }
}
