using System.Text.Json.Serialization;

namespace NistXGH.Models.Dto
{
    public class FuncaoDto
    {
        public int Id { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }
    }
}
