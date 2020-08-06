using System.Collections;
using System.Collections.Generic;
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

        [HttpPost]
        [Route("Resultado")]
        public IEnumerable Resultado(IList<Filme> listaFilmes)
        {
            return _campeonatoRegras.Campeao(listaFilmes);
        }

        [HttpGet]
        [Route("Filmes")]
        public async Task<IEnumerable> Filmes()
        {
            return await _campeonatoRegras.ObtemCompetidores();
        }
    }
}
