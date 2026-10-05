using Loja_Api.Model;
using Loja_Api.Repository;

namespace Loja_Api.Manager
{
    public class AuthManager 
    {
        private readonly RepositoryBase _repository;
        public AuthManager( RepositoryBase repo)
        {
            this._repository = repo;
        }

        public async Task<IEnumerable<Usuario>> GetAll()
        {
            return await _repository.GetAsync<Usuario>();
        }
    }
}
