using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace NistXGH.Models
// Esse arquivo é responsável por definir a classe Categoria, que representa uma categoria de dados no sistema
{
    public class CenarioAtual
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Incluído no Perfil")]
        public bool INC_PERFIL { get; set; } = true;

        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres")]
        public string JUSTIFICATIVA { get; set; }

        [StringLength(255, ErrorMessage = "A prioridade deve ter no máximo 255 caracteres")]
        [Display(Name = "Prioridade atual")]
        public int PRIOR_ATUAL { get; set; }

        [StringLength(255, ErrorMessage = "O status deve ter no máximo 255 caracteres")]
        [Display(Name = "Nível atual")]
        public string STATUS_ATUAL { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Políticas, processos e procedimentos atuais")]
        public string POLIT_ATUAL { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Práticas internas")]
        public string PRAT_ATUAL { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Funções e responsabilidades atuais")]
        public string FUNC_RESP { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Referências informativas selecionadas atualmente")]
        public string REF_INFO { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Artefatos e evidências atuais")]
        public string EVID_ATUAL { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        public string NOTAS { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Considerações")]
        public string CONSIDERACOES { get; set; }

        [ForeignKey("SUBCATEGORIA")]
        public int SUBCATEGORIA { get; set; }

        [Display(Name = "Data de Registro")]
        public DateTime DATA_REGISTRO { get; set; }
    }
}
