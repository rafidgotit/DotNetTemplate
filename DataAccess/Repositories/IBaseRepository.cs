namespace DataAccess.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity?> GetAll();
    Task<TEntity?> GetById(int id);
    void Add(TEntity? entity);
    void Update(TEntity? entity);
    void Delete(TEntity? entity);
}