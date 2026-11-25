using Loja_Api.Interfaces;
using Loja_Api.Model.Pedido;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Loja_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoManager _manager;

        public PedidosController(IPedidoManager manager)
        {
            _manager = manager;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<PedidoResposta>> Criar([FromBody] CriarPedidoRequest pedido)
        {
            Guid? usuarioId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var id = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(id, out var parsedId))
                    return Unauthorized();
                usuarioId = parsedId;
            }

            try
            {
                return StatusCode(StatusCodes.Status201Created, await _manager.CriarAsync(pedido, usuarioId));
            }
            catch (ArgumentException erro)
            {
                return BadRequest(new { mensagem = erro.Message });
            }
            catch (KeyNotFoundException erro)
            {
                return NotFound(new { mensagem = erro.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [HttpGet("meus")]
        public async Task<ActionResult<IEnumerable<PedidoResposta>>> ListarMeus()
        {
            var id = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(id, out var usuarioId))
                return Unauthorized();

            return Ok(await _manager.ListarDoUsuarioAsync(usuarioId));
        }

        [Authorize]
        [HttpGet("{pedidoId:guid}")]
        public async Task<ActionResult<PedidoResposta>> BuscarMeuPedido(Guid pedidoId)
        {
            var id = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(id, out var usuarioId))
                return Unauthorized();

            var pedido = await _manager.BuscarDoUsuarioAsync(usuarioId, pedidoId);
            return pedido is null ? NotFound() : Ok(pedido);
        }
    }
}
