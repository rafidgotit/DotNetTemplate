namespace Sugary.WepApi.Models
{
    public abstract class BaseDto<TEntity>
    {
        public int Id { get; protected set; }

        public abstract void Map(TEntity entity);
    }
}
