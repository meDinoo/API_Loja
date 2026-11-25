using Dapper;
using Loja_Api.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace Loja_Api.Repository
{
    public class RepositoryBase : IRepositoryBase
    {
        private readonly IDbConnection _conn;

        public RepositoryBase(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<T>> GetAsync<T>(
            IDictionary<string, string>? parametros = null) where T : class
        {
            var tipo = typeof(T);
            var tabela = ObterTabela(tipo);
            var propriedades = ObterPropriedadesMapeadas(tipo);
            var colunasSelecionadas = string.Join(", ", propriedades.Select(p =>
                $"{EscaparIdentificador(p.Coluna)} AS {EscaparIdentificador(p.Nome)}"));
            var valores = new DynamicParameters();
            var filtros = new List<string>();

            if (parametros is not null)
            {
                var indice = 0;
                foreach (var (campo, valor) in parametros)
                {
                    var propriedade = propriedades.SingleOrDefault(p =>
                        string.Equals(p.Nome, campo, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(p.Coluna, campo, StringComparison.OrdinalIgnoreCase));

                    if (propriedade.Propriedade is null)
                        throw new ArgumentException($"Campo de filtro '{campo}' não existe no modelo {tipo.Name}.", nameof(parametros));

                    var nomeParametro = $"valor{indice++}";
                    filtros.Add($"{EscaparIdentificador(propriedade.Coluna)} = @{nomeParametro}");
                    valores.Add(nomeParametro, valor);
                }
            }

            var sql = $"SELECT {colunasSelecionadas} FROM {tabela}" +
                      (filtros.Count > 0 ? $" WHERE {string.Join(" AND ", filtros)}" : string.Empty);

            return await _conn.QueryAsync<T>(sql, valores);
        }

        public async Task<T> AddAsync<T>(T objeto) where T : class
        {
            ArgumentNullException.ThrowIfNull(objeto);

            var tipo = typeof(T);
            var tabela = ObterTabela(tipo);
            var mapeadas = ObterPropriedadesMapeadas(tipo);
            var chavesGeradas = mapeadas
                .Where(p => p.Propriedade.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                .ToList();
            if (chavesGeradas.Count > 1)
                throw new InvalidOperationException($"O modelo {tipo.Name} possui mais de uma chave identity.");

            var propriedades = mapeadas
                .Where(p => p.Propriedade.CanRead && p.Propriedade.CanWrite)
                .Where(p => p.Propriedade.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption
                            is not (DatabaseGeneratedOption.Identity or DatabaseGeneratedOption.Computed))
                .ToList();

            if (propriedades.Count == 0)
                throw new InvalidOperationException($"O modelo {tipo.Name} não possui propriedades para inserir.");

            var colunas = string.Join(", ", propriedades.Select(p => EscaparIdentificador(p.Coluna)));
            var parametros = string.Join(", ", propriedades.Select(p => $"@{p.Propriedade.Name}"));
            var output = chavesGeradas.Count == 1
                ? $" OUTPUT INSERTED.{EscaparIdentificador(chavesGeradas[0].Coluna)}"
                : string.Empty;
            var sql = $"INSERT INTO {tabela} ({colunas}){output} VALUES ({parametros});";

            if (chavesGeradas.Count == 1)
            {
                var gerada = await _conn.ExecuteScalarAsync(sql, objeto);
                var tipoChave = Nullable.GetUnderlyingType(chavesGeradas[0].Propriedade.PropertyType)
                                ?? chavesGeradas[0].Propriedade.PropertyType;
                chavesGeradas[0].Propriedade.SetValue(objeto, Convert.ChangeType(gerada, tipoChave));
                return objeto;
            }

            var linhasAfetadas = await _conn.ExecuteAsync(sql, objeto);
            if (linhasAfetadas != 1)
                throw new InvalidOperationException($"A inclusão de {tipo.Name} não afetou exatamente um registro.");

            return objeto;
        }

        public async Task<T> Putasync<T>(T objeto) where T : class
        {
            ArgumentNullException.ThrowIfNull(objeto);

            var tipo = typeof(T);
            var tabela = ObterTabela(tipo);
            var propriedades = ObterPropriedadesMapeadas(tipo);
            var chaves = propriedades.Where(p => p.Propriedade.IsDefined(typeof(KeyAttribute), true)).ToList();
            if (chaves.Count != 1)
                throw new InvalidOperationException($"O modelo {tipo.Name} deve ter exatamente uma propriedade marcada com KeyAttribute.");

            var chave = chaves[0];
            var atualizaveis = propriedades
                .Where(p => p.Propriedade != chave.Propriedade && p.Propriedade.CanRead && p.Propriedade.CanWrite)
                .Where(p => p.Propriedade.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption
                            != DatabaseGeneratedOption.Computed)
                .ToList();

            if (atualizaveis.Count == 0)
                throw new InvalidOperationException($"O modelo {tipo.Name} não possui propriedades atualizáveis.");

            var setSql = string.Join(", ", atualizaveis.Select(p =>
                $"{EscaparIdentificador(p.Coluna)} = @{p.Propriedade.Name}"));
            var sql = $"UPDATE {tabela} SET {setSql} WHERE {EscaparIdentificador(chave.Coluna)} = @{chave.Propriedade.Name};";

            var linhasAfetadas = await _conn.ExecuteAsync(sql, objeto);
            if (linhasAfetadas == 0)
                throw new KeyNotFoundException($"Nenhum registro de {tipo.Name} foi encontrado para atualizar.");

            return objeto;
        }

        public async Task<bool> DeleteAsync<T>(object id) where T : class
        {
            ArgumentNullException.ThrowIfNull(id);

            var tipo = typeof(T);
            var tabela = ObterTabela(tipo);
            var chaves = ObterPropriedadesMapeadas(tipo)
                .Where(p => p.Propriedade.IsDefined(typeof(KeyAttribute), true))
                .ToList();
            if (chaves.Count != 1)
                throw new InvalidOperationException($"O modelo {tipo.Name} deve ter exatamente uma propriedade marcada com KeyAttribute.");

            var chave = chaves[0];
            var sql = $"DELETE FROM {tabela} WHERE {EscaparIdentificador(chave.Coluna)} = @Id;";
            return await _conn.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        private static string ObterTabela(Type tipo)
        {
            var nomeTabela = tipo.GetCustomAttribute<TableAttribute>()?.Name ?? tipo.Name;
            var esquema = tipo.GetCustomAttribute<TableAttribute>()?.Schema;
            return string.IsNullOrWhiteSpace(esquema)
                ? EscaparIdentificador(nomeTabela)
                : $"{EscaparIdentificador(esquema)}.{EscaparIdentificador(nomeTabela)}";
        }

        private static List<(PropertyInfo Propriedade, string Nome, string Coluna)> ObterPropriedadesMapeadas(Type tipo) =>
            tipo.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetIndexParameters().Length == 0)
                .Where(p => !p.IsDefined(typeof(NotMappedAttribute), true))
                .Select(p => (p, p.Name, p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name))
                .ToList();

        private static string EscaparIdentificador(string identificador)
        {
            if (string.IsNullOrWhiteSpace(identificador))
                throw new InvalidOperationException("Nome de tabela ou coluna não pode ser vazio.");

            return $"[{identificador.Replace("]", "]]", StringComparison.Ordinal)}]";
        }
    }
}
