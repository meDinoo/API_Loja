using Loja_Api.Model;

namespace Loja_Api.Interfaces
{
    public interface IAuthManager
    {
        Task<IEnumerable<Usuario>> GetAll();
    }
}
