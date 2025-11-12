using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NistXGH.Models
{
    [Table("FUNCOES", Schema = "SGSI")]
    public class Funcoes
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("CODIGO")]
        public string? CODIGO { get; set; }

        [Column("NOME")]
        public string? NOME { get; set; }

        [Column("DESCRICAO")]
        public string? DESCRICAO { get; set; }

        // 🔗 Relação 1:N com Categorias
        public ICollection<Categorias>? Categorias { get; set; }
    }
}
