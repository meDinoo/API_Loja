namespace Loja_Api.Interfaces
{
    public interface IRepositoryBase
    {
        Task<IEnumerable<T>> GetAsync<T>(IDictionary<string, string> parametros = null) where T : class;
        Task<T> AddAsync<T>(T objeto);
        Task<T> Putasync<T>(T objeto);
    }
}
