using MongoDB.Bson;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

public class RecomendacionRepository : IRecomendacionRepository
{
    private readonly IMongoCollection<RecomendacionColaborador> _colabCollection;
    private readonly IMongoCollection<RecomendacionVacante> _vacantesCollection;

    public RecomendacionRepository(IMongoDatabase db)
    {
        _colabCollection = db.GetCollection<RecomendacionColaborador>("recomendacion/colaboradores");
        _vacantesCollection = db.GetCollection<RecomendacionVacante>("recomendacion/vacantes");
    }

    public async Task<List<RecomendacionColaborador>> GetRecomendacionesColaboradores(string objectId)
    {
        return await _colabCollection
            .Find(r => r.Id == objectId)
            .ToListAsync();
    }


    public async Task<List<RecomendacionVacante>> GetRecomendacionesVacantes(string colaboradorId)
    {
        return await _vacantesCollection
            .Find(r => r.ColaboradorId == colaboradorId)
            .ToListAsync();
    }

}
