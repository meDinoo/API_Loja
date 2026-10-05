using Dapper;
using Loja_Api.Model;
using System.Data;

namespace Loja_Api.Repository
{
    public class AuthRepository 
    {
        internal readonly IDbConnection connection;

        public AuthRepository(IDbConnection conn)
        {
            this.connection = conn;
        }

        public async Task<IEnumerable<T>> teste<T>(string aa)
        {
            return await connection.QueryAsync<T>(sql: "Select * from adbc");
        }


    }
}
