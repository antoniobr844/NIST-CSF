// Models/Dto/CenarioAtualDto.cs
namespace NistXGH.Models.Dto
{
    public class CenarioAtualDto
    {
        public int SUBCATEGORIA { get; set; }
        public string JUSTIFICATIVA { get; set; } = string.Empty;
        public string PRIOR_ATUAL { get; set; } = string.Empty;
        public string NIVEL_ATUAL { get; set; } = string.Empty;
        public string POLIT_ATUAL { get; set; } = string.Empty;
        public string PRAT_ATUAL { get; set; } = string.Empty;
        public string FUNC_RESP { get; set; } = string.Empty;
        public string REF_INFO { get; set; } = string.Empty;
        public string EVID_ATUAL { get; set; } = string.Empty;
        public string NOTAS { get; set; } = string.Empty;
        public string CONSIDERACOES { get; set; } = string.Empty;
        public string ICONE { get; set; } = string.Empty;
    }
}