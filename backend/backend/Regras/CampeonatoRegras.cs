using backend.Interface;
using backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend.Regra
{
    public class CampeonatoRegras : ICampeonatoRegras
    {
        /// <summary>
        /// Método responsavel por realizar a copa
        /// </summary>
        /// <param name="filmes"></param>
        /// <returns></returns>
        public List<Filme> Campeao(IList<Filme> filmes)
        {
            if (filmes.Count != 8)
                throw new Exception("itens na lista diferente de 8");

            List<Filme> resultado = new List<Filme>();

            List<Filme> oitavas = Balancear(filmes.OrderBy(x => x.titulo).ToList());
            List<Filme> quartas = Jogar(oitavas);
            List<Filme> final = Jogar(quartas);
            Filme campeao = Jogar(final).FirstOrDefault();

            resultado.Add(campeao);
            resultado.Add(final.Where(x => x.id != campeao.id).FirstOrDefault());

            return resultado;
        }
        /// <summary>
        /// Método responsavel por indentificar qual filme vai competir com o outro
        /// </summary>
        /// <param name="filmes"></param>
        /// <returns></returns>
        public List<Filme> Balancear(List<Filme> filmes)
        {
            List<Filme> balancear = new List<Filme>(filmes);
            List<Filme> resposta = new List<Filme>();

            while (balancear.Any())
            {
                resposta.Add(balancear.First());
                resposta.Add(balancear.Last());

                resposta.ForEach(x => balancear.Remove(x));
            }

            return resposta;
        }
        /// <summary>
        /// Decide os filmes que vão para a proxima faze
        /// </summary>
        /// <param name="filmes"></param>
        /// <returns></returns>
        public List<Filme> Jogar(List<Filme> filmes)
        {
            List<Filme> resposta = new List<Filme>();

            for (int posicao = 0; posicao < filmes.Count; posicao += 2)
            {
                if (filmes[posicao].nota > filmes[posicao + 1].nota)
                    resposta.Add(filmes[posicao]);
                else if (filmes[posicao].nota == filmes[posicao + 1].nota)
                    resposta.Add(Desempate(filmes[posicao], filmes[posicao + 1]));
                else
                    resposta.Add(filmes[posicao + 1]);
            }

            return resposta;
        }
        /// <summary>
        /// Método responsavel por desempatar um filme
        /// </summary>
        /// <param name="filmeA"></param>
        /// <param name="filmeB"></param>
        /// <returns></returns>
        public Filme Desempate(Filme filmeA, Filme filmeB)
        {
            Filme resposta = new Filme();

            List<Filme> ordenar = new List<Filme>()
            {
                filmeA,
                filmeB
            };

            resposta = ordenar.OrderBy(x => x.titulo).ToList().FirstOrDefault();

            return resposta;
        }
    }
}
