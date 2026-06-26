using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrinksApps.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(80, MinimumLength = 3,
          ErrorMessage = "O nome deve ter entre 3 e 80 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [StringLength(15, MinimumLength = 10,
            ErrorMessage = "O telefone deve ter entre 10 e 15 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "O telefone deve conter apenas números.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(20, MinimumLength = 6,
     ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Confirme a senha.")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; }

        [Required]
        [StringLength(20)]
        public string Perfil { get; set; } = "Funcionario";

        public bool Ativo { get; set; } = true;
    }
}