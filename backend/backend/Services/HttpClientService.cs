using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    /// <summary>
    /// Serviço para requisições HTTP com tratamento de erros
    /// </summary>
    public interface IHttpClientService
    {
        /// <summary>
        /// Realiza uma requisição GET e deserializa o resultado
        /// </summary>
        /// <typeparam name="T">Tipo para deserialização</typeparam>
        /// <param name="url">URL da requisição</param>
        /// <returns>Objeto deserializado</returns>
        Task<T> GetAsync<T>(string url);
    }

    /// <summary>
    /// Implementação do serviço HTTP com HttpClient
    /// </summary>
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpClientService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpClientService(HttpClient httpClient, ILogger<HttpClientService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Realiza uma requisição GET e deserializa o resultado
        /// </summary>
        /// <typeparam name="T">Tipo para deserialização</typeparam>
        /// <param name="url">URL da requisição</param>
        /// <returns>Objeto deserializado</returns>
        public async Task<T> GetAsync<T>(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL não pode ser nula ou vazia", nameof(url));

            try
            {
                _logger.LogInformation($"Iniciando requisição GET para: {url}");
                
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Erro na requisição GET para {url}. Status: {response.StatusCode}, Conteúdo: {errorContent}");
                    throw new HttpRequestException($"Erro na requisição: {response.StatusCode} - {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrEmpty(content))
                {
                    _logger.LogWarning($"Conteúdo vazio retornado de: {url}");
                    return default(T);
                }

                var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                _logger.LogInformation($"Requisição GET para {url} concluída com sucesso");
                
                return result;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Erro ao deserializar resposta de {url}");
                throw new InvalidOperationException($"Erro ao processar resposta da API: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao realizar requisição para {url}");
                throw new InvalidOperationException($"Erro inesperado na requisição: {ex.Message}", ex);
            }
        }
    }
}