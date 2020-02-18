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
        List<Filme> Campeao(List<Filme> filmes);
        /// <summary>
        /// Método responsavel por indentificar qual filme vai competir com o outro
        /// </summary>
        /// <param name="filmes"></param>
        /// <returns></returns>
        List<Filme> Balancear(List<Filme> filmes);
        /// <summary>
        /// Decide os filmes que vão para a proxima faze
        /// </summary>
        /// <param name="filmes"></param>
        /// <returns></returns>
        List<Filme> Jogar(List<Filme> filmes);

        /// <summary>
        /// Método responsavel por desempatar um filme
        /// </summary>
        /// <param name="filmeA"></param>
        /// <param name="filmeB"></param>
        /// <returns></returns>
        Filme Desempate(Filme filmeA, Filme filmeB);
    }
}
