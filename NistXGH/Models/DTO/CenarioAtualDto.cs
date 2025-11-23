using System.ComponentModel.DataAnnotations;

namespace NistXGH.Models.Dto
{
    public class CenarioAtualDto
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public int SUBCATEGORIA { get; set; }
        public string? JUSTIFICATIVA { get; set; } = string.Empty;
        public int PRIOR_ATUAL { get; set; }
        public int STATUS_ATUAL { get; set; }
        public string POLIT_ATUAL { get; set; } = string.Empty;
        public string PRAT_ATUAL { get; set; } = string.Empty;
        public string FUNC_RESP { get; set; } = string.Empty;
        public string REF_INFO { get; set; } = string.Empty;
        public string? EVID_ATUAL { get; set; } = string.Empty;
        public string? NOTAS { get; set; } = string.Empty;
        public string? CONSIDERACOES { get; set; } = string.Empty;
        public DateTime DATA_REGISTRO { get; set; } = DateTime.Now;
    }
}
