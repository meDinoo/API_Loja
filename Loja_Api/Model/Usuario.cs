using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja_Api.Model
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public string _id { get; set; }
        public string _nome { get; set; }
        public string _email { get; set; }

    }
}   
