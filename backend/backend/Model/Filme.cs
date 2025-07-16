using System.ComponentModel.DataAnnotations;

namespace backend.Model
{
    /// <summary>
    /// Representa um filme na competição
    /// </summary>
    public class Filme
    {
        /// <summary>
        /// Identificador único do filme
        /// </summary>
        [Required(ErrorMessage = "O ID do filme é obrigatório")]
        public string Id { get; set; }

        /// <summary>
        /// Título do filme
        /// </summary>
        [Required(ErrorMessage = "O título do filme é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O título deve ter entre 1 e 200 caracteres")]
        public string Titulo { get; set; }

        /// <summary>
        /// Ano de lançamento do filme
        /// </summary>
        [Range(1900, 2100, ErrorMessage = "O ano deve estar entre 1900 e 2100")]
        public int Ano { get; set; }

        /// <summary>
        /// Nota do filme (0.0 a 10.0)
        /// </summary>
        [Range(0.0, 10.0, ErrorMessage = "A nota deve estar entre 0.0 e 10.0")]
        public double Nota { get; set; }

        /// <summary>
        /// Sobrescreve o método Equals para comparação de filmes
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Filme filme && Id == filme.Id;
        }

        /// <summary>
        /// Sobrescreve o método GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Sobrescreve o método ToString para debug
        /// </summary>
        public override string ToString()
        {
            return $"{Titulo} ({Ano}) - {Nota}";
        }
    }
}
