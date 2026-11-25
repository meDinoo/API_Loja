namespace Loja_Api.Model.Pedido
{
    public class PedidoResposta
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ItemPedidoResposta> Itens { get; set; } = [];
        public decimal Subtotal { get; set; }
        public decimal Frete { get; set; }
        public decimal Total { get; set; }
    }
}
