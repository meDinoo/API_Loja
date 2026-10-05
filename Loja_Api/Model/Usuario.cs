using System.ComponentModel.DataAnnotations.Schema;

namespace Loja_Api.Model
{
    [Table("Usuario")]
    public class Usuario
    {
        private string _id { get; set; }
        private string _nome { get; set; }
        private string _email { get; set; }

    }
}
