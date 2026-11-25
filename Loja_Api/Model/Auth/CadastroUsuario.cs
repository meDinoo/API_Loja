namespace Loja_Api.Model.Auth
{
    /// <summary>Dados enviados pelo front para criar uma conta.</summary>
    public class CadastroUsuario
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        // O front coleta esses campos e os envia no cadastro.
        public string Cpf { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
    }
}
