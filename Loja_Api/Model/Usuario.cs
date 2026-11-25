using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja_Api.Model
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public Guid _id { get; set; } = new Guid();
        public string _nome { get; set; }
        public string _email { get; set; }

        public string _senha { get; set; }

        public string _cpf { get; set; } = string.Empty;

        public string _telefone { get; set; } = string.Empty;

        public Usuario() {}

        public Usuario(string nome, string email, string senha, string cpf, string telefone)
        {
            _id =  Guid.NewGuid();
            _nome = nome;
            _email = email;
            _senha = senha;
            _cpf = cpf;
            _telefone = telefone;
        }
    }
}   
