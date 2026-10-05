using Loja_Api.Model;
using Loja_Api.Model.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Loja_Api.Controllers
{
    [Controller]
    [Route("login")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public Usuario login( LoginUsuario usuario)
        {

        }
    }
}
