using backend.Interface;
using backend.Model;
using backend.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace backend.Regras
{
    /// <summary>
    /// Implementação das regras do campeonato de filmes
    /// </summary>
    public class CampeonatoRegras : ICampeonatoRegras
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ILogger<CampeonatoRegras> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _urlCopaFilmes;

        public CampeonatoRegras(IHttpClientService httpClientService, ILogger<CampeonatoRegras> logger, IConfiguration configuration)
        {
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            _urlCopaFilmes = _configuration["CopaFilmes:ApiUrl"] ?? "http://copafilmes.azurewebsites.net/api/filmes";
        }

        /// <summary>
        /// Executa o campeonato e retorna os dois finalistas (campeão e vice)
        /// </summary>
        /// <param name="filmes">Lista de exatamente 8 filmes para o campeonato</param>
        /// <returns>Lista com campeão na primeira posição e vice na segunda</returns>
        public async Task<IList<Filme>> ExecutarCampeonatoAsync(IList<Filme> filmes)
        {
            if (filmes == null)
                throw new ArgumentNullException(nameof(filmes), "A lista de filmes não pode ser nula");

            if (!ValidarListaFilmes(filmes))
                throw new ArgumentException("A lista deve conter exatamente 8 filmes válidos e únicos", nameof(filmes));

            try
            {
                _logger.LogInformation("Iniciando campeonato com {Count} filmes", filmes.Count);

                // Ordenar filmes alfabeticamente
                var filmesOrdenados = filmes.OrderBy(f => f.Titulo).ToList();
                _logger.LogDebug("Filmes ordenados: {Filmes}", string.Join(", ", filmesOrdenados.Select(f => f.Titulo)));

                // Fase de oitavas (balanceamento)
                var oitavas = BalancearChaveamento(filmesOrdenados);
                _logger.LogDebug("Chaveamento das oitavas: {Chaveamento}", string.Join(" vs ", oitavas.Select((f, i) => i % 2 == 0 ? f.Titulo : $"{f.Titulo}|")));

                // Fase de quartas
                var quartas = await ExecutarFaseAsync(oitavas, "Oitavas");
                
                // Fase de semifinais
                var semifinais = await ExecutarFaseAsync(quartas, "Quartas");
                
                // Fase final
                var final = await ExecutarFaseAsync(semifinais, "Semifinais");
                
                if (final.Count != 2)
                    throw new InvalidOperationException("Erro no campeonato: deveria haver exatamente 2 filmes na final");

                var campeao = final[0];
                var vice = final[1];

                _logger.LogInformation("Campeonato concluído. Campeão: {Campeao}, Vice: {Vice}", campeao.Titulo, vice.Titulo);

                return new List<Filme> { campeao, vice };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar campeonato");
                throw;
            }
        }

        /// <summary>
        /// Obtém a lista de filmes disponíveis para competição
        /// </summary>
        /// <returns>Lista de filmes disponíveis</returns>
        public async Task<IList<Filme>> ObterCompetidoresAsync()
        {
            try
            {
                _logger.LogInformation("Obtendo lista de filmes disponíveis");
                
                var filmes = await _httpClientService.GetAsync<IList<Filme>>(_urlCopaFilmes);
                
                if (filmes == null || !filmes.Any())
                {
                    _logger.LogWarning("Nenhum filme foi retornado da API");
                    return new List<Filme>();
                }

                _logger.LogInformation("Obtidos {Count} filmes da API", filmes.Count);
                return filmes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter competidores da API");
                throw;
            }
        }

        /// <summary>
        /// Valida se a lista de filmes atende aos critérios do campeonato
        /// </summary>
        /// <param name="filmes">Lista de filmes para validar</param>
        /// <returns>True se a lista for válida, False caso contrário</returns>
        public bool ValidarListaFilmes(IList<Filme> filmes)
        {
            if (filmes == null)
                return false;

            if (filmes.Count != 8)
            {
                _logger.LogWarning("Lista de filmes deve conter exatamente 8 filmes. Recebidos: {Count}", filmes.Count);
                return false;
            }

            // Verificar se todos os filmes são válidos
            foreach (var filme in filmes)
            {
                if (filme == null)
                {
                    _logger.LogWarning("Lista contém filme nulo");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(filme.Id) || string.IsNullOrWhiteSpace(filme.Titulo))
                {
                    _logger.LogWarning("Filme com ID ou título inválido: {Filme}", filme);
                    return false;
                }

                if (filme.Nota < 0 || filme.Nota > 10)
                {
                    _logger.LogWarning("Filme com nota inválida: {Filme}", filme);
                    return false;
                }
            }

            // Verificar se não há filmes duplicados
            var idsUnicos = filmes.Select(f => f.Id).Distinct().Count();
            if (idsUnicos != filmes.Count)
            {
                _logger.LogWarning("Lista contém filmes duplicados");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Balanceia o chaveamento conforme as regras: 1º vs 8º, 2º vs 7º, etc.
        /// </summary>
        /// <param name="filmes">Lista de filmes ordenada alfabeticamente</param>
        /// <returns>Lista com os filmes balanceados para as oitavas</returns>
        private List<Filme> BalancearChaveamento(IList<Filme> filmes)
        {
            var resultado = new List<Filme>();
            var filmesTemp = new List<Filme>(filmes);

            while (filmesTemp.Any())
            {
                var primeiro = filmesTemp.First();
                var ultimo = filmesTemp.Last();

                resultado.Add(primeiro);
                resultado.Add(ultimo);

                filmesTemp.Remove(primeiro);
                filmesTemp.Remove(ultimo);
            }

            return resultado;
        }

        /// <summary>
        /// Executa uma fase do campeonato
        /// </summary>
        /// <param name="filmes">Lista de filmes para a fase</param>
        /// <param name="nomeFase">Nome da fase para log</param>
        /// <returns>Lista de vencedores da fase</returns>
        private async Task<List<Filme>> ExecutarFaseAsync(IList<Filme> filmes, string nomeFase)
        {
            var vencedores = new List<Filme>();
            _logger.LogInformation("Executando fase: {Fase} com {Count} filmes", nomeFase, filmes.Count);

            for (int i = 0; i < filmes.Count; i += 2)
            {
                var filme1 = filmes[i];
                var filme2 = filmes[i + 1];
                var vencedor = await DefinirVencedorAsync(filme1, filme2);
                
                vencedores.Add(vencedor);
                _logger.LogDebug("Partida {Fase}: {Filme1} vs {Filme2} -> Vencedor: {Vencedor}", 
                    nomeFase, filme1.Titulo, filme2.Titulo, vencedor.Titulo);
            }

            return vencedores;
        }

        /// <summary>
        /// Define o vencedor entre dois filmes
        /// </summary>
        /// <param name="filme1">Primeiro filme</param>
        /// <param name="filme2">Segundo filme</param>
        /// <returns>Filme vencedor</returns>
        private async Task<Filme> DefinirVencedorAsync(Filme filme1, Filme filme2)
        {
            // Simula processamento assíncrono (pode ser útil para logs ou processamento futuro)
            await Task.Delay(1);

            // Vencedor por maior nota
            if (filme1.Nota > filme2.Nota)
                return filme1;
            
            if (filme2.Nota > filme1.Nota)
                return filme2;

            // Desempate por ordem alfabética
            return await DesempateAsync(filme1, filme2);
        }

        /// <summary>
        /// Resolve o desempate entre dois filmes com mesma nota
        /// </summary>
        /// <param name="filme1">Primeiro filme</param>
        /// <param name="filme2">Segundo filme</param>
        /// <returns>Filme vencedor do desempate</returns>
        private async Task<Filme> DesempateAsync(Filme filme1, Filme filme2)
        {
            // Simula processamento assíncrono
            await Task.Delay(1);

            _logger.LogDebug("Desempate entre {Filme1} e {Filme2} (mesma nota: {Nota})", 
                filme1.Titulo, filme2.Titulo, filme1.Nota);

            // Vencedor por ordem alfabética
            var vencedor = string.Compare(filme1.Titulo, filme2.Titulo, StringComparison.Ordinal) < 0 ? filme1 : filme2;
            
            _logger.LogDebug("Vencedor do desempate: {Vencedor}", vencedor.Titulo);
            return vencedor;
        }
    }
}
