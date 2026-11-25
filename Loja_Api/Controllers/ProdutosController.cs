using Loja_Api.Interfaces;
using Loja_Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loja_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoManager _manager;

        public ProdutosController(IProdutoManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Listar(
            [FromQuery] string? busca,
            [FromQuery] string? categoria)
        {
            var filtros = string.IsNullOrWhiteSpace(categoria)
                ? null
                : new Dictionary<string, string> { [nameof(Produto.Categoria)] = categoria.Trim() };
            var produtos = await _manager.ListarAsync(filtros);
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.Trim();
                produtos = produtos.Where(produto =>
                    produto.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                    produto.Artista.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                    produto.Categoria.Contains(termo, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(produtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Produto>> BuscarPorId(int id)
        {
            var produto = await _manager.BuscarPorIdAsync(id);
            return produto is null ? NotFound() : Ok(produto);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Produto>> Criar([FromBody] Produto produto)
        {
            produto.Id = 0;
            var criado = await _manager.CriarAsync(produto);
            return CreatedAtAction(nameof(BuscarPorId), new { id = criado.Id }, criado);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Produto>> Atualizar(int id, [FromBody] Produto produto)
        {
            produto.Id = id;
            try
            {
                return Ok(await _manager.AtualizarAsync(produto));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Excluir(int id)
        {
            return await _manager.ExcluirAsync(id) ? NoContent() : NotFound();
        }
    }
}
