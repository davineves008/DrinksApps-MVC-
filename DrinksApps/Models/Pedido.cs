using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrinksApps.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Display (Name ="Data do pedido")]
        public DateTime Datapedido { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valortotal { get; set; }

        [StringLength(30)]
        public string Status { get; set; } = "Pendente";

        //Chave estrangeira 
        [Display(Name ="cliente")]
        public int ClienteId { get; set; }

        [ForeignKey ("clienteId")]
        public Cliente? cliente { get; set; }

        //relacionamento com pedido.
        public ICollection<ItemPedido>? ItensPedidos { get; set; }
    }
}
