using Loja_Api.Interfaces;
using Loja_Api.Model;

namespace Loja_Api.Manager
{
    public class ProdutoManager : IProdutoManager
    {
        private readonly IRepositoryBase _repository;

        public ProdutoManager(IRepositoryBase repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Produto>> ListarAsync(IDictionary<string, string>? filtros = null) =>
            _repository.GetAsync<Produto>(filtros);

        public async Task<Produto?> BuscarPorIdAsync(int id) =>
            (await _repository.GetAsync<Produto>(new Dictionary<string, string> { [nameof(Produto.Id)] = id.ToString() }))
            .SingleOrDefault();

        public Task<Produto> CriarAsync(Produto produto)
        {
            ArgumentNullException.ThrowIfNull(produto);
            return _repository.AddAsync(produto);
        }

        public Task<Produto> AtualizarAsync(Produto produto)
        {
            ArgumentNullException.ThrowIfNull(produto);
            return _repository.Putasync(produto);
        }

        public Task<bool> ExcluirAsync(int id) => _repository.DeleteAsync<Produto>(id);
    }
}
