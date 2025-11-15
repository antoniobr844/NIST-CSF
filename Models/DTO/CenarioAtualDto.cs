// Models/Dto/CenarioAtualDto.cs
namespace NistXGH.Models.Dto
{
    public class CenarioAtualDto
    {
        public int SUBCATEGORIA { get; set; }
        public string? JUSTIFICATIVA { get; set; }
        public int PRIOR_ATUAL { get; set; }
        public int STATUS_ATUAL { get; set; }
        public string POLIT_ATUAL { get; set; }
        public string PRAT_ATUAL { get; set; }
        public string FUNC_RESP { get; set; }
        public string REF_INFO { get; set; }
        public string? EVID_ATUAL { get; set; }
        public string NOTAS { get; set; }
        public string CONSIDERACOES { get; set; }
        public DateTime DATA_REGISTRO { get; set; } = DateTime.Now;
    }
}