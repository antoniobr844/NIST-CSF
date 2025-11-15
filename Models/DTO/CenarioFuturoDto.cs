using System.Text.Json.Serialization;

namespace NistXGH.Models.Dto
{
    public class CenarioFuturoDto
    {
        public int ID { get; set; }
        public int SUBCATEGORIA { get; set; }
        public string? POLIT_ALVO { get; set; }
        public string? PRAT_ALVO { get; set; }
        public string? ARTEF_ALVO { get; set; }
        public string? FUNC_ALVO { get; set; }
        public string? REF_INFO_ALVO { get; set; }
        public int? PRIORIDADE_ALVO { get; set; }
        public int? NIVEL_ALVO { get; set; }
    }
}
