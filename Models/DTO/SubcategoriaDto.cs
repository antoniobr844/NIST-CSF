using System.Text.Json.Serialization;

namespace NistXGH.Models.Dto
{
    public class SubcategoriaDto
    {
        public int Id { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }  // antes era Subcategoria

        [JsonPropertyName("nome")]
        public string Nome { get; set; }    // usa DESCRICAO

        public string Descricao { get; set; }

        public int Categoria { get; set; }
        public int Funcao { get; set; }
    }
}
