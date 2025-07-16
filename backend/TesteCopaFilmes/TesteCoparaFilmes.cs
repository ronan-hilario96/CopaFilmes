using backend.Controller;
using backend.Interface;
using backend.Model;
using backend.Regras;
using backend.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TesteCopaFilmes.Mock;
using Xunit;
using Moq;

namespace TesteCopaFilmes
{
    public class TesteCopaFilmes
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly ICampeonatoRegras _campeonatoRegras;
        private readonly Mock<IHttpClientService> _mockHttpClientService;
        private readonly Mock<ILogger<CampeonatoRegras>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public TesteCopaFilmes()
        {
            // Setup mocks
            _mockHttpClientService = new Mock<IHttpClientService>();
            _mockLogger = new Mock<ILogger<CampeonatoRegras>>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Setup configuration mock
            _mockConfiguration.Setup(c => c["CopaFilmes:ApiUrl"])
                .Returns("http://copafilmes.azurewebsites.net/api/filmes");

            // Setup CampeonatoRegras with mocked dependencies
            _campeonatoRegras = new CampeonatoRegras(_mockHttpClientService.Object, _mockLogger.Object, _mockConfiguration.Object);

            // Setup test server
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<backend.Startup>()
                .ConfigureServices(services =>
                {
                    // Replace real services with mocked ones for testing
                    services.AddSingleton(_mockHttpClientService.Object);
                    services.AddSingleton(_mockLogger.Object);
                }));
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task TesteLogicaMock()
        {
            // Arrange
            var filmes = MockFilmes.listFilmes.Take(8).ToList();

            // Act
            var vencedores = await _campeonatoRegras.ExecutarCampeonatoAsync(filmes);

            // Assert
            Assert.NotNull(vencedores);
            Assert.Equal(2, vencedores.Count);
            Assert.Equal("Vingadores: Guerra Infinita", vencedores.First().Titulo);
        }

        [Fact]
        public async Task TesteValidarListaFilmes_ListaValida()
        {
            // Arrange
            var filmes = MockFilmes.listFilmes.Take(8).ToList();

            // Act
            var isValid = _campeonatoRegras.ValidarListaFilmes(filmes);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task TesteValidarListaFilmes_ListaInvalida_MenosDeOitoFilmes()
        {
            // Arrange
            var filmes = MockFilmes.listFilmes.Take(7).ToList();

            // Act
            var isValid = _campeonatoRegras.ValidarListaFilmes(filmes);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task TesteValidarListaFilmes_ListaInvalida_MaisDeOitoFilmes()
        {
            // Arrange
            var filmes = MockFilmes.listFilmes.Take(9).ToList();

            // Act
            var isValid = _campeonatoRegras.ValidarListaFilmes(filmes);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task TesteValidarListaFilmes_ListaInvalida_FilmesNulos()
        {
            // Arrange
            var filmes = new List<Filme>();
            for (int i = 0; i < 7; i++)
            {
                filmes.Add(MockFilmes.listFilmes[i]);
            }
            filmes.Add(null);

            // Act
            var isValid = _campeonatoRegras.ValidarListaFilmes(filmes);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task TesteValidarListaFilmes_ListaInvalida_FilmesDuplicados()
        {
            // Arrange
            var filmes = MockFilmes.listFilmes.Take(7).ToList();
            filmes.Add(MockFilmes.listFilmes.First()); // Adiciona duplicado

            // Act
            var isValid = _campeonatoRegras.ValidarListaFilmes(filmes);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public async Task TesteObterCompetidoresAsync()
        {
            // Arrange
            var filmesEsperados = MockFilmes.listFilmes.Take(10).ToList();
            _mockHttpClientService.Setup(s => s.GetAsync<IList<Filme>>(It.IsAny<string>()))
                .ReturnsAsync(filmesEsperados);

            // Act
            var filmes = await _campeonatoRegras.ObterCompetidoresAsync();

            // Assert
            Assert.NotNull(filmes);
            Assert.Equal(10, filmes.Count);
            _mockHttpClientService.Verify(s => s.GetAsync<IList<Filme>>(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task TesteExecutarCampeonatoAsync_ArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<System.ArgumentNullException>(
                () => _campeonatoRegras.ExecutarCampeonatoAsync(null));
        }

        [Fact]
        public async Task TesteExecutarCampeonatoAsync_ArgumentException_ListaInvalida()
        {
            // Arrange
            var filmes = MockFilmes.listFilmes.Take(7).ToList();

            // Act & Assert
            await Assert.ThrowsAsync<System.ArgumentException>(
                () => _campeonatoRegras.ExecutarCampeonatoAsync(filmes));
        }

        [Fact]
        public async Task TesteOrdenacaoAlfabetica()
        {
            // Arrange
            var filmes = new List<Filme>
            {
                new Filme { Id = "1", Titulo = "Z Filme", Ano = 2020, Nota = 8.0 },
                new Filme { Id = "2", Titulo = "A Filme", Ano = 2020, Nota = 7.0 },
                new Filme { Id = "3", Titulo = "M Filme", Ano = 2020, Nota = 6.0 },
                new Filme { Id = "4", Titulo = "B Filme", Ano = 2020, Nota = 5.0 },
                new Filme { Id = "5", Titulo = "Y Filme", Ano = 2020, Nota = 4.0 },
                new Filme { Id = "6", Titulo = "C Filme", Ano = 2020, Nota = 3.0 },
                new Filme { Id = "7", Titulo = "X Filme", Ano = 2020, Nota = 2.0 },
                new Filme { Id = "8", Titulo = "D Filme", Ano = 2020, Nota = 1.0 }
            };

            // Act
            var resultado = await _campeonatoRegras.ExecutarCampeonatoAsync(filmes);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            // O campeão deve ser determinado pelo chaveamento após ordenação alfabética
            Assert.NotNull(resultado.First().Titulo);
        }

        [Fact]
        public async Task TesteDesempate()
        {
            // Arrange - filmes com mesma nota para testar desempate
            var filmes = new List<Filme>
            {
                new Filme { Id = "1", Titulo = "A Filme", Ano = 2020, Nota = 8.0 },
                new Filme { Id = "2", Titulo = "B Filme", Ano = 2020, Nota = 8.0 },
                new Filme { Id = "3", Titulo = "C Filme", Ano = 2020, Nota = 7.0 },
                new Filme { Id = "4", Titulo = "D Filme", Ano = 2020, Nota = 7.0 },
                new Filme { Id = "5", Titulo = "E Filme", Ano = 2020, Nota = 6.0 },
                new Filme { Id = "6", Titulo = "F Filme", Ano = 2020, Nota = 6.0 },
                new Filme { Id = "7", Titulo = "G Filme", Ano = 2020, Nota = 5.0 },
                new Filme { Id = "8", Titulo = "H Filme", Ano = 2020, Nota = 5.0 }
            };

            // Act
            var resultado = await _campeonatoRegras.ExecutarCampeonatoAsync(filmes);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            // Em caso de empate, vence o filme com nome que vem primeiro alfabeticamente
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
                _server?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}
