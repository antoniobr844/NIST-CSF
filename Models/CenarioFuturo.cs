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

        [ForeignKey("PRIORIDADE_ALVO")]
        public virtual PrioridadeTb Prioridade { get; set; }

        [Column("NIVEL_ALVO")]
        public int? NIVEL_ALVO { get; set; }

        [ForeignKey("NIVEL_ALVO")]
        public virtual StatusTb Nivel { get; set; }

        [Column("POLIT_ALVO")]
        public string? POLIT_ALVO { get; set; } // CLOB

        [Column("PRAT_ALVO")]
        public string? PRAT_ALVO { get; set; } // CLOB

        [Column("ARTEF_ALVO")]
        [StringLength(200)]
        public string? ARTEF_ALVO { get; set; }

        [Column("FUNC_ALVO")]
        [StringLength(200)]
        public string? FUNC_ALVO { get; set; }

        [Column("REF_INFO_ALVO")]
        [StringLength(50)]
        public string? REF_INFO_ALVO { get; set; }

        [Column("SUBCATEGORIA")]
        public int SUBCATEGORIA { get; set; }

        [Column("DATA_REGISTRO")]
        public DateTime? DATA_REGISTRO { get; set; }

        [ForeignKey("SUBCATEGORIA")]
        public virtual Subcategorias SubcategoriaNav { get; set; }

        

    }
}