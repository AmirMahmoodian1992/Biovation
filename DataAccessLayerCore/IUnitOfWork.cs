namespace DataAccessLayerCore
{
    public interface IUnitOfWork
    {
        void Dispose();

        void SaveChanges();
    }
}
