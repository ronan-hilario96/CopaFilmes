using backend.Model;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Interface
{
    public interface ICampeonatoRegras
    {
        /// <summary>
        /// Método responsavel por realizar a copa
        /// </summary>
        /// <param name="filmes"></param>
        /// <returns></returns>
        ICollection<Filme> Campeao(ICollection<Filme> filmes);
        Task<IEnumerable<Filme>> ObtemCompetidores();
    }
}
