namespace Loja_Api.Interfaces
{
    public interface IRepositoryBase
    {
        Task<IEnumerable<T>> GetAsync<T>(IDictionary<string, string>? parametros = null) where T : class;
        Task<T> AddAsync<T>(T objeto) where T : class;
        Task<T> Putasync<T>(T objeto) where T : class;
        Task<bool> DeleteAsync<T>(object id) where T : class;
    }
}
