// Models/PrioridadeTb.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NistXGH.Models
{
    [Table("SGSI_PRIORIDADE_TB")]
    public class PrioridadeTb
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("NIVEL")]
        [StringLength(5)]
        public string NIVEL { get; set; }
    }
}
