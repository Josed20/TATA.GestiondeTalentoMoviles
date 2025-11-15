using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities; // Asume que aquí está la clase Evaluacion
using TATA.GestiondeTalentoMoviles.CORE.Interfaces; // Asume que aquí está la interfaz IEvaluacionRepository

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Repositories
{
    // Asegúrate de que esta clase implemente la interfaz correspondiente
    public class EvaluacionRepository : IEvaluacionRepository
    {
        // Colección privada que interactúa con MongoDB
        private readonly IMongoCollection<Evaluacion> _evaluaciones;

        // --- Constructor ---
        public EvaluacionRepository(IMongoDatabase database)
        {
            // ✅ Nombre EXACTO de la colección en MongoDB, que es "evaluaciones"
            _evaluaciones = database.GetCollection<Evaluacion>("plantillas_evaluacion");
        }

        // --- Operaciones CRUD ---

        public async Task<Evaluacion> CreateAsync(Evaluacion evaluacion)
        {
            // ✅ No establecer Id, dejar que MongoDB lo genere automáticamente
            await _evaluaciones.InsertOneAsync(evaluacion);
            // MongoDB habrá asignado el Id automáticamente al objeto 'evaluacion'
            return evaluacion;
        }

        public async Task<IEnumerable<Evaluacion>> GetAllAsync()
        {
            // ✅ Obtiene todos los documentos de la colección
            return await _evaluaciones.Find(_ => true).ToListAsync();
        }

        public async Task<Evaluacion?> GetByIdAsync(string id)
        {
            // ✅ Busca un documento por su campo Id (ObjectId)
            return await _evaluaciones
                .Find(e => e.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, Evaluacion evaluacion)
        {
            // ✅ Asegurar que el Id del documento coincida con el que se está actualizando
            evaluacion.Id = id;

            var result = await _evaluaciones.ReplaceOneAsync(
                // Filtro: busca el documento por Id
                e => e.Id == id,
                // Documento de reemplazo completo
                evaluacion,
                // Opciones: no insertar si no existe (IsUpsert = false)
                new ReplaceOptions { IsUpsert = false }
            );

            // Retorna true si se modificó al menos un documento
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            // ✅ En lugar de un borrado lógico como en el ejemplo de Colaborador,
            //    usaremos el borrado físico (DeleteOne) que es más común para registros de historial/auditoría.
            var result = await _evaluaciones.DeleteOneAsync(e => e.Id == id);

            // Retorna true si se borró al menos un documento
            return result.DeletedCount > 0;
        }
    }
}