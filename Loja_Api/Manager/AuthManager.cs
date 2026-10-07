using Loja_Api.Interfaces;
using Loja_Api.Model;
using Loja_Api.Repository;

namespace Loja_Api.Manager
{
    public class AuthManager : IAuthManager
    {
        private readonly IRepositoryBase _repository;
        public AuthManager( IRepositoryBase repo)
        {
            this._repository = repo;
        }

        public async Task<IEnumerable<Usuario>> GetAll()
        {
            return await _repository.GetAsync<Usuario>();
        }
    }
}
