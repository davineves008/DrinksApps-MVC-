using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrinksApps.Models
{
    [Table("Produtos")]
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(500)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A quantidade em estoque é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
        public int Estoque { get; set; }

        public string? ImagemUrl { get; set; }

        public bool Ativo { get; set; } = true;

        // Chave estrangeira
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }

        //metodo que atualiza a quantia do produto.
        public void AtualizarEstoque(int quantidadeVendida)
        {
            if (quantidadeVendida <= 0)
                return;

            Estoque -= quantidadeVendida;

            if (Estoque <= 0)
            {
                Estoque = 0;
                Ativo = false;
            }
        }

    }
}