using Loja_Api.Model;

namespace Loja_Api.Interfaces
{
    public interface IProdutoManager
    {
        Task<IEnumerable<Produto>> ListarAsync(IDictionary<string, string>? filtros = null);
        Task<Produto?> BuscarPorIdAsync(int id);
        Task<Produto> CriarAsync(Produto produto);
        Task<Produto> AtualizarAsync(Produto produto);
        Task<bool> ExcluirAsync(int id);
    }
}
