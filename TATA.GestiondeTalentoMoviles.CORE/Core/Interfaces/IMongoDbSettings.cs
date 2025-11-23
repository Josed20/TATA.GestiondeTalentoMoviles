namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IMongoDbSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}