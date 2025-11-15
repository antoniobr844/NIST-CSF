using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NistXGH.Models
{
    [Table("CENARIO_LOG", Schema = "SGSI")]
    public class CenarioLog
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("CENARIO_ID")]
        public int CENARIO_ID { get; set; }

        [Column("CENARIO_TIPO")]
        [StringLength(10)]
        public string CENARIO_TIPO { get; set; } // 'ATUAL' ou 'FUTURO'

        [Column("SUBCATEGORIA_ID")]
        public int SUBCATEGORIA_ID { get; set; }

        [Column("CAMPO_ALTERADO")]
        [StringLength(50)]
        public string CAMPO_ALTERADO { get; set; }

        [Column("VALOR_ANTIGO")]
        public string VALOR_ANTIGO { get; set; }

        [Column("VALOR_NOVO")]
        public string VALOR_NOVO { get; set; }

        [Column("USUARIO")]
        [StringLength(100)]
        public string USUARIO { get; set; }

        [Column("DATA_ALTERACAO")]
        public DateTime DATA_ALTERACAO { get; set; }

        [Column("IP_MAQUINA")]
        [StringLength(45)]
        public string IP_MAQUINA { get; set; }
    }
}