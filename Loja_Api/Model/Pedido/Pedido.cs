using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja_Api.Model.Pedido
{
    [Table("Pedido")]
    public class Pedido
    {
        [Key]
        [Column("ID_PEDIDO")]
        public Guid Id { get; set; }
        public Guid? UsuarioId { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Status { get; set; } = "confirmado";
        public string NomeContato { get; set; } = string.Empty;
        public string EmailContato { get; set; } = string.Empty;
        public string TelefoneContato { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string FormaPagamento { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Frete { get; set; }
        public decimal Total { get; set; }
    }
}
