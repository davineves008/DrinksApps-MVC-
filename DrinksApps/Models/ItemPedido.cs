using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrinksApps.Models
{
    [Table("ItensPedidos")]
    public class ItemPedido
    {
        [Key]
        public int Id { get; set; }

        //pedido.
        [Required]
        public int PedidoId { get; set; }
        [ForeignKey("PedidoId")]
        public Pedido? Pedido { get; set; }

        //produto.

        [Required]
        public int ProdutoId { get; set; }
        [ForeignKey("ProdutoId")]

        public Produto? Produto { get; set; }

        //quantidade.
        [Required(ErrorMessage ="A quantidade é obrigatoria")]
        [Range(1,999)]
        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecoUnitario { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

    }
}
