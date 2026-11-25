using Loja_Api.Interfaces;
using Loja_Api.Model.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Loja_Api.Controllers
{
    [ApiController]
    [Route("login")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _manager;

        public AuthController(IAuthManager manager)
        {
            _manager = manager;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginResposta>> Login([FromBody] LoginUsuario credenciais)
        {
            var resposta = await _manager.Login(credenciais);
            return resposta is null ? Unauthorized(new { mensagem = "E-mail ou senha inválidos." }) : Ok(resposta);
        }

        [AllowAnonymous]
        [HttpPost("cadastro")]
        public async Task<ActionResult<UsuarioConta>> Cadastrar([FromBody] CadastroUsuario cadastro)
        {
            try
            {
                var conta = await _manager.Cadastrar(cadastro);
                return Created("login/atual", conta);
            }
            catch (InvalidOperationException erro)
            {
                return Conflict(new { mensagem = erro.Message });
            }
            catch (ArgumentException erro)
            {
                return BadRequest(new { mensagem = erro.Message });
            }
        }

        [Authorize]
        [HttpGet("atual")]
        public ActionResult<UsuarioConta> Atual()
        {
            var nome = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
            return string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email)
                ? Unauthorized()
                : Ok(new UsuarioConta { Nome = nome, Email = email });
        }
    }
}
