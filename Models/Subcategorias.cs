using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NistXGH.Models
{
    [Table("SUBCATEGORIAS", Schema = "SGSI")]
    public class Subcategorias
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("CATEGORIA")]
        public int CATEGORIA { get; set; }

        [Column("FUNCAO")]
        public int FUNCAO { get; set; }

        [Column("SUBCATEGORIA")]
        public string? SUBCATEGORIA { get; set; }

        [Column("DESCRICAO")]
        public string? DESCRICAO { get; set; }

        // Relação N:1 com Categorias
        [ForeignKey("CATEGORIA")]
        public Categorias? CategoriaNav { get; set; }

        [ForeignKey("FUNCAO")]
        public virtual Funcoes? FuncaoNav { get; set; }
    }
}
