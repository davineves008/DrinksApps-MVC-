using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrinksApps.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Data do Pedido")]
        public DateTime DataPedido { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "O valor total é obrigatório.")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Valor Total")]
        public decimal ValorTotal { get; set; }

        [StringLength(30)]
        public string Status { get; set; } 



        // FK do usuário que realizou o pedido
        [Display(Name = "Cliente")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public virtual ICollection<ItemPedido> ItensPedidos { get; set; }
            = new List<ItemPedido>();

        // Dados da entrega
        public string Rua { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string CEP { get; set; }

        // Pagamento
        public string FormaPagamento { get; set; }

        public string? Observacoes { get; set; }
    }
}
