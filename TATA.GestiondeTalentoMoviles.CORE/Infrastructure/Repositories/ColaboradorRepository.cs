using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities; // corregido namespace de Entities
using TATA.GestiondeTalentoMoviles.CORE.Interfaces; // corregido namespace de Interfaces

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly IMongoCollection<Colaborador> _colaboradores;

        public ColaboradorRepository(IMongoDatabase database)
        {
            _colaboradores = database.GetCollection<Colaborador>("Colaboradores");
        }

        public async Task<Colaborador> CreateAsync(Colaborador colaborador)
        {
            await _colaboradores.InsertOneAsync(colaborador);
            return colaborador;
        }

        public async Task<IEnumerable<Colaborador>> GetAllAsync()
        {
            return await _colaboradores.Find(_ => true).ToListAsync();
        }

        public async Task<Colaborador?> GetByIdAsync(string id)
        {
            return await _colaboradores.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}
