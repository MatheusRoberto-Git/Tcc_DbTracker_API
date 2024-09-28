using Microsoft.AspNetCore.Mvc;
using Tcc_DbTracker_API.Repository;

namespace Tcc_DbTracker_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DBTrackerController : ControllerBase
    {
        #region [Construtor]

        private readonly Conn_SqlServer _sqlServer;

        public DBTrackerController(Conn_SqlServer sqlServer)
        {
            _sqlServer = sqlServer;
        }

        #endregion

        #region [Get Tables]

        [HttpGet("getAllTables")]
        public IActionResult GetAllTables()
        {
            IEnumerable<string> allTables = _sqlServer.ListarTabelas();
            return Ok(allTables);
        }

        [HttpGet("getAllTables/structures")]
        public ActionResult<IEnumerable<object>> GetAllTableStructures()
        {
            var tabelas = _sqlServer.ListarTabelas();

            var estruturas = tabelas.Select(tabela => new
            {
                TableName = tabela,
                Structure = _sqlServer.EstruturaTabelas(tabela)
            }).ToList();

            return Ok(estruturas);
        }

        [HttpGet("{nomeTabela}/structure")]
        public ActionResult<IEnumerable<dynamic>> GetTableStructure(string nomeTabela)
        {
            if (string.IsNullOrEmpty(nomeTabela))
            {
                return BadRequest("O nome da tabela não pode ser nulo ou vazio.");
            }

            IEnumerable<dynamic> estrutura = _sqlServer.EstruturaTabelas(nomeTabela);

            if (estrutura == null || !estrutura.Any())
            {
                return NotFound($"A tabela '{nomeTabela}' não foi encontrada.");
            }

            var result = new
            {
                TableName = nomeTabela,
                Structure = estrutura
            };

            return Ok(result);
        }

        #endregion
    }
}