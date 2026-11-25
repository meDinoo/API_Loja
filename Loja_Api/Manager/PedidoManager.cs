using Dapper;
using Loja_Api.Interfaces;
using Loja_Api.Model;
using Loja_Api.Model.Pedido;
using System.Data;

namespace Loja_Api.Manager
{
    public class PedidoManager : IPedidoManager
    {
        private const decimal FretePadrao = 12.90m;
        private readonly IDbConnection _connection;
        private readonly IRepositoryBase _repository;

        public PedidoManager(IDbConnection connection, IRepositoryBase repository)
        {
            _connection = connection;
            _repository = repository;
        }

        public async Task<PedidoResposta> CriarAsync(CriarPedidoRequest pedido, Guid? usuarioId)
        {
            ArgumentNullException.ThrowIfNull(pedido);
            ValidarPedido(pedido);

            var itensAgrupados = pedido.Itens
                .GroupBy(item => item.ProdutoId)
                .Select(grupo => new ItemPedidoRequest { ProdutoId = grupo.Key, Quantidade = grupo.Sum(item => item.Quantidade) })
                .ToList();
            var produtos = (await _repository.GetAsync<Produto>()).ToDictionary(produto => produto.Id);
            var itensResposta = new List<ItemPedidoResposta>();
            foreach (var item in itensAgrupados)
            {
                if (!produtos.TryGetValue(item.ProdutoId, out var produto))
                    throw new KeyNotFoundException($"Produto {item.ProdutoId} não encontrado.");

                itensResposta.Add(new ItemPedidoResposta
                {
                    ProdutoId = produto.Id,
                    NomeProduto = produto.Nome,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = produto.Preco,
                    Total = produto.Preco * item.Quantidade
                });
            }

            var subtotal = itensResposta.Sum(item => item.Total);
            var idPedido = Guid.NewGuid();
            var dataCriacao = DateTime.UtcNow;
            var formaPagamento = pedido.FormaPagamento.Trim().ToLowerInvariant();
            var endereco = pedido.EnderecoEntrega;
            var contato = pedido.Contato;

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transacao = _connection.BeginTransaction();
            try
            {
                const string sqlPedido = """
                    INSERT INTO [Pedido]
                    ([ID_PEDIDO], [UsuarioId], [DataCriacao], [Status], [NomeContato], [EmailContato], [TelefoneContato],
                     [Cep], [Endereco], [Numero], [Complemento], [Bairro], [Cidade], [Estado], [FormaPagamento], [Subtotal], [Frete], [Total])
                    VALUES
                    (@Id, @UsuarioId, @DataCriacao, @Status, @NomeContato, @EmailContato, @TelefoneContato,
                     @Cep, @Endereco, @Numero, @Complemento, @Bairro, @Cidade, @Estado, @FormaPagamento, @Subtotal, @Frete, @Total);
                    """;

                await _connection.ExecuteAsync(sqlPedido, new
                {
                    Id = idPedido,
                    UsuarioId = usuarioId,
                    DataCriacao = dataCriacao,
                    Status = "recebido",
                    NomeContato = contato.Nome.Trim(),
                    EmailContato = contato.Email.Trim().ToLowerInvariant(),
                    TelefoneContato = SomenteDigitos(contato.Telefone),
                    Cep = SomenteDigitos(endereco.Cep),
                    Endereco = endereco.Endereco.Trim(),
                    Numero = endereco.Numero.Trim(),
                    Complemento = endereco.Complemento.Trim(),
                    Bairro = endereco.Bairro.Trim(),
                    Cidade = endereco.Cidade.Trim(),
                    Estado = endereco.Estado.Trim().ToUpperInvariant(),
                    FormaPagamento = formaPagamento,
                    Subtotal = subtotal,
                    Frete = FretePadrao,
                    Total = subtotal + FretePadrao
                }, transacao);

                const string sqlItem = """
                    INSERT INTO [ItemPedido]
                    ([PedidoId], [ProdutoId], [NomeProduto], [PrecoUnitario], [Quantidade])
                    VALUES (@PedidoId, @ProdutoId, @NomeProduto, @PrecoUnitario, @Quantidade);
                    """;
                foreach (var item in itensResposta)
                {
                    await _connection.ExecuteAsync(sqlItem, new
                    {
                        PedidoId = idPedido,
                        item.ProdutoId,
                        item.NomeProduto,
                        item.PrecoUnitario,
                        item.Quantidade
                    }, transacao);
                }

                transacao.Commit();
            }
            catch
            {
                transacao.Rollback();
                throw;
            }

            return new PedidoResposta
            {
                Id = idPedido.ToString(),
                Data = dataCriacao,
                Status = "recebido",
                Itens = itensResposta,
                Subtotal = subtotal,
                Frete = FretePadrao,
                Total = subtotal + FretePadrao
            };
        }

        public async Task<IEnumerable<PedidoResposta>> ListarDoUsuarioAsync(Guid usuarioId)
        {
            var pedidos = await _repository.GetAsync<Pedido>(new Dictionary<string, string>
            {
                [nameof(Pedido.UsuarioId)] = usuarioId.ToString()
            });

            var respostas = new List<PedidoResposta>();
            foreach (var pedido in pedidos.OrderByDescending(item => item.DataCriacao))
            {
                var itens = await _repository.GetAsync<PedidoItem>(new Dictionary<string, string>
                {
                    [nameof(PedidoItem.PedidoId)] = pedido.Id.ToString()
                });
                respostas.Add(MapearResposta(pedido, itens));
            }

            return respostas;
        }

        public async Task<PedidoResposta?> BuscarDoUsuarioAsync(Guid usuarioId, Guid pedidoId)
        {
            var pedido = (await _repository.GetAsync<Pedido>(new Dictionary<string, string>
            {
                [nameof(Pedido.Id)] = pedidoId.ToString(),
                [nameof(Pedido.UsuarioId)] = usuarioId.ToString()
            })).SingleOrDefault();

            if (pedido is null)
                return null;

            var itens = await _repository.GetAsync<PedidoItem>(new Dictionary<string, string>
            {
                [nameof(PedidoItem.PedidoId)] = pedido.Id.ToString()
            });
            return MapearResposta(pedido, itens);
        }

        private static PedidoResposta MapearResposta(Pedido pedido, IEnumerable<PedidoItem> itens) => new()
        {
            Id = pedido.Id.ToString(),
            Data = pedido.DataCriacao,
            Status = pedido.Status,
            Itens = itens.Select(item => new ItemPedidoResposta
            {
                ProdutoId = item.ProdutoId,
                NomeProduto = item.NomeProduto,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                Total = item.PrecoUnitario * item.Quantidade
            }).ToList(),
            Subtotal = pedido.Subtotal,
            Frete = pedido.Frete,
            Total = pedido.Total
        };

        private static void ValidarPedido(CriarPedidoRequest pedido)
        {
            if (pedido.Contato is null || pedido.EnderecoEntrega is null || pedido.Itens is null || pedido.Itens.Count == 0)
                throw new ArgumentException("Informe contato, endereço de entrega e pelo menos um item.");
            if (string.IsNullOrWhiteSpace(pedido.Contato.Nome) || string.IsNullOrWhiteSpace(pedido.Contato.Email) ||
                string.IsNullOrWhiteSpace(pedido.Contato.Telefone))
                throw new ArgumentException("Informe nome, e-mail e telefone para contato.");
            if (string.IsNullOrWhiteSpace(pedido.EnderecoEntrega.Cep) || string.IsNullOrWhiteSpace(pedido.EnderecoEntrega.Endereco) ||
                string.IsNullOrWhiteSpace(pedido.EnderecoEntrega.Numero) || string.IsNullOrWhiteSpace(pedido.EnderecoEntrega.Cidade) ||
                string.IsNullOrWhiteSpace(pedido.EnderecoEntrega.Estado))
                throw new ArgumentException("Informe os dados necessários do endereço de entrega.");
            if (pedido.Itens.Any(item => item.ProdutoId <= 0 || item.Quantidade <= 0))
                throw new ArgumentException("Os produtos e quantidades do pedido são inválidos.");
            if (!new[] { FormaPagamento.Pix, FormaPagamento.Cartao, FormaPagamento.Boleto }
                .Contains(pedido.FormaPagamento?.Trim().ToLowerInvariant()))
                throw new ArgumentException("Forma de pagamento inválida.");
        }

        private static string SomenteDigitos(string valor) =>
            new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
    }
}
