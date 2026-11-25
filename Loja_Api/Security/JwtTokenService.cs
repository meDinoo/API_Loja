using Loja_Api.Model;
using Loja_Api.Model.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Loja_Api.Security
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LoginResposta CriarToken(Usuario usuario)
        {
            var chave = ObterChave();
            var emissor = _configuration["Jwt:Issuer"] ?? "Loja_Api";
            var audiencia = _configuration["Jwt:Audience"] ?? "Loja_Frontend";
            var minutos = int.TryParse(_configuration["Jwt:DuracaoMinutos"], out var configurado) && configurado > 0
                ? Math.Min(configurado, 1440)
                : 60;
            var expiraEm = DateTime.UtcNow.AddMinutes(minutos);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario._id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario._email),
                new Claim(ClaimTypes.Name, usuario._nome),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var credenciais = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(emissor, audiencia, claims, expires: expiraEm, signingCredentials: credenciais);

            return new LoginResposta
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiraEm = expiraEm,
                Usuario = new UsuarioConta { Nome = usuario._nome, Email = usuario._email }
            };
        }

        public byte[] ObterChave()
        {
            var valor = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(valor) || Encoding.UTF8.GetByteCount(valor) < 32)
                throw new InvalidOperationException("Configure Jwt__Key com uma chave secreta de pelo menos 32 bytes.");
            return Encoding.UTF8.GetBytes(valor);
        }
    }
}
