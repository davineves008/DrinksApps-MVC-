using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrinksApps.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Data do pedido")]
        public DateTime DataPedido { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorTotal { get; set; }

        [StringLength(30)]
        public string Status { get; set; } = "Pendente";

        // FK correta
        public int ClienteId { get; set; }

        // Navigation property correta
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        public ICollection<ItemPedido>? ItensPedidos { get; set; }
    }
}