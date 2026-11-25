namespace Loja_Api.Model.Auth
{
    public class LoginResposta
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
        public UsuarioConta Usuario { get; set; } = new();
    }
}
