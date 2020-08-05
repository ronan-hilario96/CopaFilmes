using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Interface;
using backend.Model;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controller
{
    [ApiController]
    [Route("v1/[controller]")]
    public class CopaFilmes
    {
        private readonly ICampeonatoRegras _campeonatoRegras;

        public CopaFilmes(ICampeonatoRegras campeonatoRegras)
        {
            _campeonatoRegras = campeonatoRegras;
        }

        private readonly string URLCOPA = "http://copafilmes.azurewebsites.net/api/filmes";

        [HttpPost]
        [Route("Resultado")]
        public List<Filme> Resultado(List<Filme> listaFilmes)
        {
            return _campeonatoRegras.Campeao(listaFilmes);
        }

        [HttpGet]
        [Route("Filmes")]
        public async Task<List<Filme>> Filmes()
        {
            try
            {
                List<Filme> resposta = new List<Filme>();
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage res = await client.GetAsync(URLCOPA))
                using (HttpContent content = res.Content)
                {
                    resposta = JsonSerializer.Deserialize<List<Filme>>(await content.ReadAsStringAsync());
                }
                return resposta;
            } catch (Exception e)
            {
                throw e;
            }
        }
    }
}
