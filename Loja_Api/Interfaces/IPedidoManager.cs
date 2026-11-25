using Loja_Api.Model.Pedido;

namespace Loja_Api.Interfaces
{
    public interface IPedidoManager
    {
        Task<PedidoResposta> CriarAsync(CriarPedidoRequest pedido, Guid? usuarioId);
        Task<IEnumerable<PedidoResposta>> ListarDoUsuarioAsync(Guid usuarioId);
        Task<PedidoResposta?> BuscarDoUsuarioAsync(Guid usuarioId, Guid pedidoId);
    }
}
