using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using backend.Interface;
using backend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.Controller
{
    /// <summary>
    /// Controller para gerenciar o campeonato de filmes
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    public class CopaFilmesController : ControllerBase
    {
        private readonly ICampeonatoRegras _campeonatoRegras;
        private readonly ILogger<CopaFilmesController> _logger;

        public CopaFilmesController(ICampeonatoRegras campeonatoRegras, ILogger<CopaFilmesController> logger)
        {
            _campeonatoRegras = campeonatoRegras ?? throw new ArgumentNullException(nameof(campeonatoRegras));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executa o campeonato e retorna os dois finalistas
        /// </summary>
        /// <param name="filmes">Lista de exatamente 8 filmes para o campeonato</param>
        /// <returns>Lista com campeão na primeira posição e vice na segunda</returns>
        /// <response code="200">Campeonato executado com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [Route("Resultado")]
        [ProducesResponseType(typeof(IList<Filme>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<Filme>>> ExecutarCampeonato([FromBody] IList<Filme> filmes)
        {
            try
            {
                _logger.LogInformation("Recebida solicitação para executar campeonato");

                if (filmes == null)
                {
                    _logger.LogWarning("Lista de filmes é nula");
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Dados inválidos",
                        Detail = "A lista de filmes não pode ser nula",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Validar model state
                if (!TryValidateModel(filmes))
                {
                    _logger.LogWarning("Modelo de dados inválido");
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Dados inválidos",
                        Detail = "Os dados dos filmes são inválidos",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Validar regras de negócio
                if (!_campeonatoRegras.ValidarListaFilmes(filmes))
                {
                    _logger.LogWarning("Lista de filmes não atende aos critérios do campeonato");
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Dados inválidos",
                        Detail = "A lista deve conter exatamente 8 filmes válidos e únicos",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var resultado = await _campeonatoRegras.ExecutarCampeonatoAsync(filmes);
                
                _logger.LogInformation("Campeonato executado com sucesso");
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido ao executar campeonato");
                return BadRequest(new ProblemDetails
                {
                    Title = "Dados inválidos",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao executar campeonato");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Erro interno",
                    Detail = "Erro interno do servidor",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Obtém a lista de filmes disponíveis para competição
        /// </summary>
        /// <returns>Lista de filmes disponíveis</returns>
        /// <response code="200">Lista de filmes obtida com sucesso</response>
        /// <response code="503">Serviço indisponível</response>
        [HttpGet]
        [Route("Filmes")]
        [ProducesResponseType(typeof(IList<Filme>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IList<Filme>>> ObterFilmes()
        {
            try
            {
                _logger.LogInformation("Recebida solicitação para obter filmes");
                
                var filmes = await _campeonatoRegras.ObterCompetidoresAsync();
                
                _logger.LogInformation("Lista de filmes obtida com sucesso. Total: {Count}", filmes.Count);
                return Ok(filmes);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao obter filmes da API externa");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
                {
                    Title = "Serviço indisponível",
                    Detail = "Não foi possível obter a lista de filmes. Tente novamente mais tarde.",
                    Status = StatusCodes.Status503ServiceUnavailable
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao obter filmes");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Erro interno",
                    Detail = "Erro interno do servidor",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Valida se uma lista de filmes atende aos critérios do campeonato
        /// </summary>
        /// <param name="filmes">Lista de filmes para validar</param>
        /// <returns>Resultado da validação</returns>
        /// <response code="200">Validação executada com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        [HttpPost]
        [Route("Validar")]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<ValidationResult> ValidarFilmes([FromBody] IList<Filme> filmes)
        {
            try
            {
                _logger.LogInformation("Recebida solicitação para validar filmes");

                if (filmes == null)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Dados inválidos",
                        Detail = "A lista de filmes não pode ser nula",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var isValid = _campeonatoRegras.ValidarListaFilmes(filmes);
                
                var result = new ValidationResult
                {
                    IsValid = isValid,
                    Message = isValid ? "Lista de filmes válida" : "Lista de filmes inválida",
                    FilmesCount = filmes.Count
                };

                _logger.LogInformation("Validação concluída. Resultado: {IsValid}", isValid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao validar filmes");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Erro interno",
                    Detail = "Erro interno do servidor",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }

    /// <summary>
    /// Resultado da validação de filmes
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indica se a lista é válida
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Mensagem descritiva do resultado
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Número de filmes na lista
        /// </summary>
        public int FilmesCount { get; set; }
    }
}
