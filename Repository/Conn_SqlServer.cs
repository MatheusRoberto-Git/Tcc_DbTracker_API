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

        public IEnumerable<string> ListarAllTables()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";

                return connection.Query<string>(query);
            }
        }

        public IEnumerable<dynamic> EstruturaTables(string nmTabela)
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

        #region [Procedures]

        public IEnumerable<string> ListarAllProcedures()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT ROUTINE_NAME 
            FROM INFORMATION_SCHEMA.ROUTINES 
            WHERE ROUTINE_TYPE = 'PROCEDURE'";

                return connection.Query<string>(query);
            }
        }

        public string GetAllScriptProcedure(string nomeProcedure)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT sm.definition
            FROM sys.procedures p
            INNER JOIN sys.sql_modules sm ON p.object_id = sm.object_id
            WHERE p.name = @NomeProcedure";

                return connection.QueryFirstOrDefault<string>(query, new { NomeProcedure = nomeProcedure });
            }
        }

        public IEnumerable<(string NomeProcedure, string Definicao)> GetScriptAllProceduresName()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT 
                SPECIFIC_NAME AS NomeProcedure,
                OBJECT_DEFINITION(OBJECT_ID(SPECIFIC_NAME)) AS Definicao
            FROM INFORMATION_SCHEMA.ROUTINES
            WHERE ROUTINE_TYPE = 'PROCEDURE'";

                var results = connection.Query(query)
                    .Select(row => (
                        NomeProcedure: (string)row.NomeProcedure,
                        Definicao: ((string)row.Definicao)?
                            .Replace("\r\n", Environment.NewLine)
                            .Replace("\t", "    ")
                    ));

                return results;
            }
        }

        #endregion

        #region [Functions]

        public IEnumerable<string> ListarFunction()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT ROUTINE_NAME
            FROM INFORMATION_SCHEMA.ROUTINES
            WHERE ROUTINE_TYPE = 'FUNCTION'";

                return connection.Query<string>(query);
            }
        }

        public IEnumerable<(string NomeFuncao, string Definicao)> GetScriptFunction()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT 
                ROUTINE_NAME AS NomeFuncao,
                OBJECT_DEFINITION(OBJECT_ID(ROUTINE_NAME)) AS Definicao
            FROM INFORMATION_SCHEMA.ROUTINES
            WHERE ROUTINE_TYPE = 'FUNCTION'";

                var results = connection.Query(query)
                    .Select(row => (
                        NomeFuncao: (string)row.NomeFuncao,
                        Definicao: ((string)row.Definicao)?
                            .Replace("\r\n", Environment.NewLine)
                            .Replace("\t", "    ")
                    ));

                return results;
            }
        }

        public string GetScriptFunctionName(string nomeFunction)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT sm.definition
            FROM sys.sql_modules sm
            INNER JOIN sys.objects so ON sm.object_id = so.object_id
            WHERE so.type IN ('FN', 'IF', 'TF') AND so.name = @NomeFunc";

                return connection.QueryFirstOrDefault<string>(query, new { NomeFunc = nomeFunction });
            }
        }

        #endregion

        #region [Views]

        public IEnumerable<string> ListarAllViews()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT TABLE_NAME
            FROM INFORMATION_SCHEMA.VIEWS";

                return connection.Query<string>(query);
            }
        }

        public IEnumerable<dynamic> ScriptsAllViews()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT 
                sm.definition AS Script,
                o.name AS ViewName
            FROM sys.sql_modules sm
            INNER JOIN sys.objects o ON sm.object_id = o.object_id
            WHERE o.type = 'V'";

                return connection.Query(query);
            }
        }

        public string ScriptViewName(string nomeView)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT sm.definition
            FROM sys.sql_modules sm
            INNER JOIN sys.objects o ON sm.object_id = o.object_id
            WHERE o.type = 'V' AND o.name = @Nome";

                return connection.QueryFirstOrDefault<string>(query, new { Nome = nomeView });
            }
        }

        #endregion

        #region [Triggers]

        public IEnumerable<string> ListarAllTriggers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT name 
            FROM sys.triggers 
            WHERE parent_class_desc = 'OBJECT_OR_COLUMN'";

                return connection.Query<string>(query);
            }
        }

        public IEnumerable<dynamic> ScriptsAllTriggers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT 
                t.name AS TriggerName,
                sm.definition AS Script
            FROM sys.triggers t
            INNER JOIN sys.sql_modules sm ON t.object_id = sm.object_id";

                return connection.Query(query);
            }
        }

        public string ScriptTriggerName(string nomeTrigger)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT sm.definition
            FROM sys.triggers t
            INNER JOIN sys.sql_modules sm ON t.object_id = sm.object_id
            WHERE t.name = @Nome";

                return connection.QueryFirstOrDefault<string>(query, new { Nome = nomeTrigger });
            }
        }

        #endregion

        #region [Indexes]

        public IEnumerable<dynamic> ListarAllIndexes()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT 
                t.name AS TableName,
                ind.name AS IndexName,
                ind.type_desc AS IndexType,
                ind.is_unique AS IsUnique,
                ind.is_primary_key AS IsPrimaryKey
            FROM sys.indexes ind
            INNER JOIN sys.tables t ON ind.object_id = t.object_id
            WHERE ind.name IS NOT NULL";

                return connection.Query(query);
            }
        }        

        public string GetScriptIndexName(string nomeIndex)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT TOP 1 'CREATE ' +
                CASE WHEN i.is_unique = 1 THEN 'UNIQUE ' ELSE '' END +
                i.type_desc + ' INDEX [' + i.name + '] ON [' + s.name + '].[' + t.name + '] (' +
                STRING_AGG(COL_NAME(ic.object_id, ic.column_id) COLLATE DATABASE_DEFAULT, ', ') 
                    WITHIN GROUP (ORDER BY ic.key_ordinal) + ')' AS IndexScript
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE i.name = @nomeIndex
            GROUP BY i.name, i.is_unique, i.type_desc, s.name, t.name";

                return connection.QueryFirstOrDefault<string>(query, new { nomeIndex });
            }
        }

        #endregion

        #region [Constraints]

        public IEnumerable<dynamic> ListarAllConstraints()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT 
                TABLE_NAME,
                CONSTRAINT_NAME,
                CONSTRAINT_TYPE
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS";

                return connection.Query(query);
            }
        }

        public IEnumerable<string> GetScriptsConstraints()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT definition
            FROM sys.check_constraints
            UNION ALL
            SELECT OBJECT_DEFINITION(object_id)
            FROM sys.objects
            WHERE type IN ('C','D','F','PK','UQ')";

                return connection.Query<string>(query);
            }
        }        

        #endregion
    }
}