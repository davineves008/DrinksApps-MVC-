namespace DrinksApps.Models
{
    public class CarrinhoItem
    {
        public int ProdutoId { get; set; }

        public string Nome { get; set; }

        public decimal Preco { get; set; }

        public string? ImagemUrl { get; set; }

        public int Quantidade { get; set; }

        public decimal Total
        {
            get { return Preco * Quantidade; }
        }
    }
}