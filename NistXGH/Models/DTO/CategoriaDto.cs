using System.Text.Json.Serialization;

namespace NistXGH.Models.Dto
{
    public class CategoriaDto
    {
        public int Id { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        public int Funcao { get; set; }
    }
}
