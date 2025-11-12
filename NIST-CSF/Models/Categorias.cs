using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NistXGH.Models
{
    [Table("CATEGORIAS", Schema = "SGSI")]
    public class Categorias
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

        [Column("FUNCAO")]
        public int FUNCAO { get; set; }

        // 🔗 Relação N:1 com Funcoes
        [ForeignKey("FUNCAO")]
        public Funcoes? FuncaoNav { get; set; }

        // 🔗 Relação 1:N com Subcategorias
        public ICollection<Subcategorias>? Subcategorias { get; set; }
    }
}
