using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja_Api.Model
{
    [Table("Produto")]
    public class Produto
    {
        [Key]
        [Column("ID_PRODUTO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Artista { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Imagem { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public bool Destaque { get; set; } = false;
    }
}
