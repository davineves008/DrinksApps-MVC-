using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrinksApps.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatorio.")]
        [StringLength(100)]
        public string Nome  { get; set; }

        [Required(ErrorMessage = "A idade do cliente é obrigatorio.")]
        [Range(18, 130, ErrorMessage ="A idade deve ter entre 18 e 130  anos.")]
        public int Idade { get; set; }

        [Required(ErrorMessage ="O cpf  é obrigatorio.")]
        [StringLength (14)]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O email  é obrigatorio.")]
        [StringLength (100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage ="O telefone  é obrigatorio.")]
        [StringLength (20)]
        public string Telefone { get; set; }

        [Required(ErrorMessage ="O endereço é obrigatorio.")]
        [StringLength (200)]
        public string Endereco { get; set; }

      
        public string? Cidade { get; set; }

        public string? Estado { get; set; }

        public bool Ativo { get; set; } = true;

        //relacionamento com pedidos;
        [NotMapped]
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();




    }
}
