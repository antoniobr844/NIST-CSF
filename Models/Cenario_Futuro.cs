using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NistXGH.Models
{
    [Table("CENARIO_FUTURO", Schema = "SGSI")]
    public class CenarioFuturo
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("PRIORIDADE_ALVO")]
        public int? PRIORIDADE_ALVO { get; set; }

        [ForeignKey(nameof(PRIORIDADE_ALVO))]
        public PrioridadeTb Prioridade { get; set; }

        [Column("NIVEL_ALVO")]
        public int? NIVEL_ALVO { get; set; }

        [ForeignKey(nameof(NIVEL_ALVO))]
        public StatusTb Nivel { get; set; }

        [Column("POLIT_ALVO")]
        public string POLIT_ALVO { get; set; }

        [Column("PRAT_ALVO")]
        public string PRAT_ALVO { get; set; }

        [Column("ARTEF_ALVO")]
        public string ARTEF_ALVO { get; set; }

        [Column("FUNC_ALVO")]
        public string FUNC_ALVO { get; set; }

        [Column("REF_INFO_ALVO")]
        public string REF_INFO_ALVO { get; set; }

        [Column("SUBCATEGORIA")]
        public int SUBCATEGORIA { get; set; }

        [Column("DATA_REGISTRO")]
        public DateTime DATA_REGISTRO { get; set; }

        public virtual Subcategorias SubcategoriaNav { get; set; }
    }
}
