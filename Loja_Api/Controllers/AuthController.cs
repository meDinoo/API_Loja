using Loja_Api.Manager;
using Loja_Api.Model;
using Loja_Api.Model.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Loja_Api.Controllers
{
    [Controller]
    [Route("login")]
    public class AuthController : ControllerBase
    {
        private readonly AuthManager _manager;
        public AuthController(AuthManager manager)
        {
            this._manager = manager;
        }

        [HttpPost]
        public Usuario login( LoginUsuario usuario)
        {
            return new Usuario();
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> Get()
        {

          return Ok(await _manager.GetAll());

        }
    }
}
