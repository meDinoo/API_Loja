namespace Loja_Api.Model.Pedido
{
    public class CriarPedidoRequest
    {
        public ContatoPedido Contato { get; set; } = new();
        public EnderecoEntrega EnderecoEntrega { get; set; } = new();
        public string FormaPagamento { get; set; } = string.Empty;
        public List<ItemPedidoRequest> Itens { get; set; } = [];
    }
}
