using Dapper;
using Loja_Api.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace Loja_Api.Repository
{
    public class RepositoryBase : IRepositoryBase
    {
        internal readonly IDbConnection _conn;

        public RepositoryBase( IDbConnection conn)
        {
            this._conn = conn;
        }

        public  async Task<IEnumerable<T>> GetAsync<T>(IDictionary<string, string> parametros = null) where T : class
        {
            var tipo = typeof(T);
            var atributo = tipo.GetCustomAttribute<TableAttribute>();
            string table = atributo?.Name;

            if (parametros?.Count > 0)
            {
                List<string> filtros = new List<string>();
                var valores = new DynamicParameters();
                int indice = 0;

                foreach (var item in parametros)
                {
                    string nomeParametro = $"valor{indice}";
                    filtros.Add($"[{item.Key}] = @{nomeParametro}");
                    valores.Add(nomeParametro, item.Value);
                    indice++;
                }

                string sql = $"""
                    SELECT * FROM [{table}]
                    WHERE {string.Join(" AND ", filtros)}
                    """;

                return await _conn.QueryAsync<T>(sql, valores);
            }


            return await _conn.QueryAsync<T>($"SELECT * FROM {table}");

        }

        public  async Task<T> AddAsync<T>(T objeto)
        {
            string table = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;

            var propiedades = typeof(T).GetProperties().Where(p => p.CanRead).ToList();

            var colunas = propiedades.Select(p =>
            {
                var coluna = p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name;
                return $"[{coluna}]";
            });

            var valores = propiedades.Select(p => $"@{p.Name}");

            string sql = $"""
                INSERT INTO [{table}] ({string.Join(',', colunas)})
                VALUE
                ({string.Join(',', valores)}
                """;

            var resposta = await _conn.ExecuteAsync(sql, objeto);

            if (resposta == -1 || resposta == 0)
            {
                throw new Exception("Erro ao Adiconar elemento");
            }

            return objeto;

        }

        public async Task<T> Putasync<T>(T objeto)
        {
            return objeto;
        }




    }


}
