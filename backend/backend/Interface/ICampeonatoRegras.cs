using backend.Model;
using System.Collections.Generic;

namespace backend.Interface
{
    public interface ICampeonatoRegras
    {
        /// <summary>
        /// Método responsavel por realizar a copa
        /// </summary>
        /// <param name="filmes"></param>
        /// <returns></returns>
        List<Filme> Campeao(IList<Filme> filmes);
    }
}
