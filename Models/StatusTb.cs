// Models/StatusTb.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NistXGH.Models
{
    [Table("SGSI_STATUS_TB")]
    public class StatusTb
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("NIVEL")]
        [StringLength(50)]
        public string NIVEL { get; set; }

        [Column("STATUS")]
        [StringLength(50)]
        public string STATUS { get; set; }
    }
}
