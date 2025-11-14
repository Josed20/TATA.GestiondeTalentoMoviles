using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class NivelSkillRepository : INivelSkillRepository
    {
        private readonly IMongoCollection<NivelSkill> _nivelesSkill;

        public NivelSkillRepository(IMongoDatabase database)
        {
            _nivelesSkill = database.GetCollection<NivelSkill>("nivelskills");
        }

        public async Task<NivelSkill> CreateAsync(NivelSkill nivelSkill)
        {
            // No establecer Id, dejar que MongoDB lo genere automáticamente
            await _nivelesSkill.InsertOneAsync(nivelSkill);
            return nivelSkill; // MongoDB habrá asignado el Id automáticamente
        }

        public async Task<IEnumerable<NivelSkill>> GetAllAsync()
        {
            return await _nivelesSkill.Find(_ => true).ToListAsync();
        }

        public async Task<NivelSkill?> GetByIdAsync(string id)
        {
            return await _nivelesSkill
                .Find(n => n.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<NivelSkill?> GetByCodigoAsync(int codigo)
        {
            return await _nivelesSkill
                .Find(n => n.Codigo == codigo)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, NivelSkill nivelSkill)
        {
            // Asegurar que el Id coincida con el parámetro
            nivelSkill.Id = id;

            var result = await _nivelesSkill.ReplaceOneAsync(
                n => n.Id == id,
                nivelSkill,
                new ReplaceOptions { IsUpsert = false }
            );

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _nivelesSkill.DeleteOneAsync(n => n.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
