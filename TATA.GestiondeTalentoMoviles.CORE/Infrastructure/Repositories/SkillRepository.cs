using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly IMongoCollection<Skill> _skills;

        public SkillRepository(IMongoDatabase database)
        {
            // nombre EXACTO de la colección en Mongo
            _skills = database.GetCollection<Skill>("skills");
        }

        public async Task<Skill> CreateAsync(Skill skill)
        {
            await _skills.InsertOneAsync(skill);
            return skill;
        }

        public async Task<IEnumerable<Skill>> GetAllAsync()
        {
            return await _skills.Find(_ => true).ToListAsync();
        }

        public async Task<Skill?> GetByIdAsync(string id)
        {
            return await _skills.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, Skill skill)
        {
            skill.Id = id;

            var result = await _skills.ReplaceOneAsync(
                s => s.Id == id,
                skill,
                new ReplaceOptions { IsUpsert = false }
            );

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _skills.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
