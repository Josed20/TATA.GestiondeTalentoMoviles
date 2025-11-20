using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class EvaluacionService : IEvaluacionService
    {
        private readonly IEvaluacionRepository _repository;

        public EvaluacionService(IEvaluacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Evaluacion>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Evaluacion?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Evaluacion>> GetByColaboradorAsync(string colaboradorId)
        {
            return await _repository.GetByColaboradorAsync(colaboradorId);
        }

        public async Task AddAsync(Evaluacion evaluacion)
        {
            evaluacion.FechaCreacion = DateTime.Now;
            evaluacion.FechaActualizacion = DateTime.Now;

            await _repository.AddAsync(evaluacion);
        }

        public async Task UpdateAsync(Evaluacion evaluacion)
        {
            evaluacion.FechaActualizacion = DateTime.Now;
            await _repository.UpdateAsync(evaluacion);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
