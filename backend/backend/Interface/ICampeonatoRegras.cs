using backend.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Interface
{
    /// <summary>
    /// Interface para as regras do campeonato de filmes
    /// </summary>
    public interface ICampeonatoRegras
    {
        /// <summary>
        /// Executa o campeonato e retorna os dois finalistas (campeão e vice)
        /// </summary>
        /// <param name="filmes">Lista de exatamente 8 filmes para o campeonato</param>
        /// <returns>Lista com campeão na primeira posição e vice na segunda</returns>
        /// <exception cref="ArgumentException">Lançada quando não há exatamente 8 filmes</exception>
        /// <exception cref="ArgumentNullException">Lançada quando a lista de filmes é nula</exception>
        Task<IList<Filme>> ExecutarCampeonatoAsync(IList<Filme> filmes);

        /// <summary>
        /// Obtém a lista de filmes disponíveis para competição
        /// </summary>
        /// <returns>Lista de filmes disponíveis</returns>
        /// <exception cref="HttpRequestException">Lançada quando há erro na requisição HTTP</exception>
        Task<IList<Filme>> ObterCompetidoresAsync();

        /// <summary>
        /// Valida se a lista de filmes atende aos critérios do campeonato
        /// </summary>
        /// <param name="filmes">Lista de filmes para validar</param>
        /// <returns>True se a lista for válida, False caso contrário</returns>
        bool ValidarListaFilmes(IList<Filme> filmes);
    }
}
