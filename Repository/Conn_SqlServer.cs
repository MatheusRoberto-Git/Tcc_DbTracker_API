using Dapper;
using System.Data.SqlClient;

namespace Tcc_DbTracker_API.Repository
{
    public class Conn_SqlServer
    {
        #region [Construtor]

        private readonly string _connectionString;

        public Conn_SqlServer(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #endregion

        #region [Tabelas]

        public IEnumerable<string> ListarTabelas()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";

                return connection.Query<string>(query);
            }
        }

        public IEnumerable<dynamic> EstruturaTabelas(string nmTabela)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT 
                    COLUMN_NAME, 
                    DATA_TYPE, 
                    IS_NULLABLE, 
                    CHARACTER_MAXIMUM_LENGTH 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = @TabelaNome";

                return connection.Query(query, new { TabelaNome = nmTabela });
            }
        }

        #endregion
    }
}
