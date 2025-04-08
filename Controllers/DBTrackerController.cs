using Microsoft.AspNetCore.Mvc;
using Tcc_DbTracker_API.Repository;
using Tcc_DbTracker_API.Services;

namespace Tcc_DbTracker_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DBTrackerController : ControllerBase
    {
        #region [Construtor]

        private readonly Conn_SqlServer _sqlServer;
        private readonly EmailService _emailService;

        public DBTrackerController(Conn_SqlServer sqlServer, EmailService emailService)
        {
            _sqlServer = sqlServer;
            _emailService = emailService;
        }

        #endregion

        #region [Get Tables]

        [HttpGet("getAllTables")]
        public IActionResult GetAllTables()
        {
            try
            {
                IEnumerable<string> allTables = _sqlServer.ListarAllTables();

                _emailService.SendEmail("Backup - Tabelas", $"Backup de tabelas realizado com sucesso às {DateTime.Now}");

                return Ok(allTables);
            }
            catch (Exception ex)
            {
                _emailService.SendEmail("Erro no Backup - Tabelas",$"Erro ao obter a lista de tabelas às {DateTime.Now}: {ex.Message}" );
                return StatusCode(500, "Erro interno ao obter a lista de tabelas.");
            }
        }

        [HttpGet("getAllTables/structures")]
        public ActionResult<IEnumerable<object>> GetAllTableStructures()
        {
            try
            {
                var tabelas = _sqlServer.ListarAllTables();

                var estruturas = tabelas.Select(tabela => new
                {
                    TableName = tabela,
                    Structure = _sqlServer.EstruturaTables(tabela)
                }).ToList();

                return Ok(estruturas);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao obter estruturas das tabelas.");
            }
        }

        [HttpGet("{nomeTabela}/structure")]
        public ActionResult<object> GetTableStructure(string nomeTabela)
        {
            try
            {
                if (string.IsNullOrEmpty(nomeTabela))
                {
                    return BadRequest("O nome da tabela não pode ser nulo ou vazio.");
                }

                var estrutura = _sqlServer.EstruturaTables(nomeTabela);

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
            catch (Exception)
            {
                return StatusCode(500, $"Erro interno ao obter estrutura da tabela '{nomeTabela}'.");
            }
        }

        #endregion

        #region [Get Procedures]

        [HttpGet("getAllProcedures")]
        public IActionResult GetAllProcedures()
        {
            try
            {
                var procedures = _sqlServer.ListarAllProcedures();

                if (procedures == null || !procedures.Any())
                    return NotFound("Nenhuma stored procedure encontrada.");

                return Ok(procedures);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao obter a lista de procedures.");
            }
        }

        [HttpGet("getAllProcedures/scripts")]
        public IActionResult GetAllProcedureScripts()
        {
            try
            {
                var procedures = _sqlServer.GetScriptAllProceduresName();

                if (procedures == null || !procedures.Any())
                    return NotFound("Nenhuma procedure encontrada.");

                return Ok(procedures.Select(p => new
                {
                    ProcedureName = p.NomeProcedure,
                    Definition = p.Definicao
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao buscar os scripts das procedures.");
            }
        }

        [HttpGet("getScriptProcedure/{nomeProcedure}")]
        public IActionResult GetProcedureDefinition(string nomeProcedure)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nomeProcedure))
                    return BadRequest("O nome da procedure não pode ser vazio.");

                var definition = _sqlServer.GetAllScriptProcedure(nomeProcedure);

                if (string.IsNullOrWhiteSpace(definition))
                    return NotFound($"Procedure '{nomeProcedure}' não encontrada.");

                return Ok(new
                {
                    ProcedureName = nomeProcedure,
                    Definition = definition
                });
            }
            catch (Exception)
            {
                return StatusCode(500, $"Erro interno ao obter a definição da procedure '{nomeProcedure}'.");
            }
        }

        #endregion

        #region [Get Functions]

        [HttpGet("getAllFunctions")]
        public IActionResult GetAllFunctionNames()
        {
            try
            {
                var nomesFuncoes = _sqlServer.ListarFunction();

                if (nomesFuncoes == null || !nomesFuncoes.Any())
                    return NotFound("Nenhuma função encontrada.");

                return Ok(nomesFuncoes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter nomes das funções: {ex.Message}");
                return StatusCode(500, "Erro interno ao buscar os nomes das funções.");
            }
        }

        [HttpGet("getAllFunctions/scripts")]
        public IActionResult GetAllFunctionScripts()
        {
            try
            {
                var funcoes = _sqlServer.GetScriptFunction();

                if (funcoes == null || !funcoes.Any())
                    return NotFound("Nenhuma função encontrada.");

                return Ok(funcoes.Select(f => new
                {
                    FunctionName = f.NomeFuncao,
                    Definition = f.Definicao
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao buscar os scripts das funções.");
            }
        }

        [HttpGet("function/{nomeFunction}")]
        public IActionResult GetFunctionByName(string nomeFunction)
        {
            if (string.IsNullOrEmpty(nomeFunction))
                return BadRequest("O nome da função não pode ser nulo ou vazio.");

            try
            {
                var script = _sqlServer.GetScriptFunctionName(nomeFunction);

                if (string.IsNullOrWhiteSpace(script))
                    return NotFound($"Função '{nomeFunction}' não encontrada.");

                return Ok(new { Nome = nomeFunction, Script = script });
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao buscar a função.");
            }
        }

        #endregion

        #region [Get Views]

        [HttpGet("getAllViews")]
        public IActionResult GetAllViews()
        {
            try
            {
                var views = _sqlServer.ListarAllViews();

                if (views == null || !views.Any())
                    return NotFound("Nenhuma view encontrada.");

                return Ok(views);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao listar views: {ex.Message}");
            }
        }

        [HttpGet("getAllViews/scripts")]
        public IActionResult GetAllViewScripts()
        {
            try
            {
                var scripts = _sqlServer.ScriptsAllViews();

                if (scripts == null || !scripts.Any())
                    return NotFound("Nenhum script de view encontrado.");

                return Ok(scripts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar scripts das views: {ex.Message}");
            }
        }

        [HttpGet("{nomeView}/script")]
        public IActionResult GetViewScript(string nomeView)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nomeView))
                    return BadRequest("O nome da view não pode ser vazio.");

                var script = _sqlServer.ScriptViewName(nomeView);

                if (string.IsNullOrEmpty(script))
                    return NotFound($"View '{nomeView}' não encontrada.");

                return Ok(new { ViewName = nomeView, Script = script });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar script da view: {ex.Message}");
            }
        }

        #endregion

        #region [Get Triggers]

        [HttpGet("getAllTriggers")]
        public IActionResult GetAllTriggers()
        {
            try
            {
                var triggers = _sqlServer.ListarAllTriggers();

                if (triggers == null || !triggers.Any())
                    return NotFound("Nenhuma trigger encontrada.");

                return Ok(triggers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao listar triggers: {ex.Message}");
            }
        }

        [HttpGet("getAllTriggers/scripts")]
        public IActionResult GetAllTriggerScripts()
        {
            try
            {
                var scripts = _sqlServer.ScriptsAllTriggers();

                if (scripts == null || !scripts.Any())
                    return NotFound("Nenhum script de trigger encontrado.");

                return Ok(scripts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar scripts das triggers: {ex.Message}");
            }
        }

        [HttpGet("{nomeTrigger}/scriptTrigger")]
        public IActionResult GetTriggerScript(string nomeTrigger)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nomeTrigger))
                    return BadRequest("O nome da trigger não pode ser vazio.");

                var script = _sqlServer.ScriptTriggerName(nomeTrigger);

                if (string.IsNullOrEmpty(script))
                    return NotFound($"Trigger '{nomeTrigger}' não encontrada.");

                return Ok(new { TriggerName = nomeTrigger, Script = script });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar script da trigger: {ex.Message}");
            }
        }

        #endregion

        #region [Get Indexes]

        [HttpGet("getAllIndexes")]
        public IActionResult GetAllIndexes()
        {
            try
            {
                var indexes = _sqlServer.ListarAllIndexes();

                if (indexes == null || !indexes.Any())
                    return NotFound("Nenhum índice encontrado.");

                return Ok(indexes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao listar indexes: {ex.Message}");
            }
        }        

        [HttpGet("getIndexScript/{nomeIndex}")]
        public IActionResult GetIndexScript(string nomeIndex)
        {
            try
            {
                var script = _sqlServer.GetScriptIndexName(nomeIndex);

                if (string.IsNullOrEmpty(script))
                    return NotFound($"Index '{nomeIndex}' não encontrado.");

                return Ok(script);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter script do index: {ex.Message}");
            }
        }

        #endregion

        #region [Get Constraints]

        [HttpGet("getAllConstraints")]
        public IActionResult GetAllConstraints()
        {
            try
            {
                var constraints = _sqlServer.ListarAllConstraints();

                if (constraints == null || !constraints.Any())
                    return NotFound("Nenhuma constraint encontrada.");

                return Ok(constraints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao listar constraints: {ex.Message}");
            }
        }

        [HttpGet("getAllConstraintScripts")]
        public IActionResult GetAllConstraintScripts()
        {
            try
            {
                var scripts = _sqlServer.GetScriptsConstraints();
                return Ok(scripts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter scripts de constraints: {ex.Message}");
            }
        }        

        #endregion
    }
}