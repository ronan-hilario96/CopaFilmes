using backend;
using backend.Controller;
using backend.Regra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Linq;
using System.Net.Http;
using TesteCopaFilmes.Mock;
using Xunit;

namespace TesteCopaFilmes
{
    public class TesteCoparaFilmes
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public TesteCoparaFilmes()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }
        
        [Fact]
        public async void TesteLogicaMock()
        {
            var regras = new CampeonatoRegras();

            var filmes = MockFilmes.listFilmes.Take(8);

            var vencedores = regras.Campeao(filmes.ToList());

            Assert.Equal("Vingadores: Guerra Infinita", vencedores.First().titulo);
        }

        [Fact]
        public async void TesteLogicaIntegrado()
        {
            var controller = new CopaFilmes(new CampeonatoRegras());

            var filmes = controller.Filmes().Result;

            var vencedores = controller.Resultado(filmes.Take(8).ToList());

            Assert.Equal("Vingadores: Guerra Infinita", vencedores.First().titulo);
        }
    }
}
