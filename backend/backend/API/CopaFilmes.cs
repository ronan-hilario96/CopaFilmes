using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Model;
using Microsoft.AspNetCore.Mvc;
using backend.Controllers;

namespace backend.API
{
    [ApiController]
    [Route("v1/[controller]")]
    public class CopaFilmes
    {
        private readonly string URLCOPA = "http://copafilmes.azurewebsites.net/api/filmes";

        [HttpPost]
        [Route("Resultado")]
        public List<Filme> Resultado(List<Filme> listaFilmes)
        {
            return new FilmesController().Campeao(listaFilmes);
        }

        [HttpGet]
        [Route("Filmes")]
        public async Task<List<Filme>> Filmes()
        
        {
            List<Filme> resposta = new List<Filme>();
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(URLCOPA))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();
                if(data != null)
                {
                    resposta = JsonSerializer.Deserialize<List<Filme>>(data);
                }
            }
            return resposta;
        }
    }
}
