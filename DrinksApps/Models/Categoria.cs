using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrinksApps.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="O Nome da categoria é obrigatório.")]
        [StringLength (50)]
        public string Nome {  get; set; }

        [StringLength (500)]
        public string Descricao { get; set; }

        //relacionamento
        public ICollection<Produto> ?Produtos { get; set; }
    }
}
