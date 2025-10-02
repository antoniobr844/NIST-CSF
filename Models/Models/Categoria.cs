// Models/Categoria.cs
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace NistXGH.Models
// Esse arquivo é responsável por definir a classe Categoria, que representa uma categoria de dados no sistema
{
    public class Categoria
    {
        public int Id { get; set; }

        // esse campo é obrigatório e deve ter no máximo 100 caracteres
        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Resultado { get; set; }

        // esse campo é obrigatório e deve ter no máximo 255 caracteres
        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Incluído no Perfil")]
        public bool IncluidoPerfil { get; set; } = true;

        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres")]
        public string Justificativa { get; set; }

        [StringLength(255, ErrorMessage = "A prioridade deve ter no máximo 255 caracteres")]
        [Display(Name = "Prioridade atual")]
        public string Prioridade { get; set; }

        [StringLength(255, ErrorMessage = "O status deve ter no máximo 255 caracteres")]
        [Display(Name = "Status atual")]
        public string Status { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Políticas, processos e procedimentos atuais")]
        public string PoliticasPro { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Práticas internas")]
        public string PraticasInternas { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Funções e responsabilidades atuais")]
        public string FuncoesResp { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Referências informativas selecionadas atualmente")]
        public string ReferenciasInfo { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Artefatos e evidências atuais")]
        public string ArtefatosEvi { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        public string Notas { get; set; }

        [StringLength(255, ErrorMessage = "O campo deve ter no máximo 255 caracteres")]
        [Display(Name = "Considerações")]
        public string Consideracoes { get; set; }

        [StringLength(50, ErrorMessage = "O ícone deve ter no máximo 50 caracteres")]
        public string Icone { get; set; }
    }
}
