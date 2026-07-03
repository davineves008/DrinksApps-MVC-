using System.ComponentModel.DataAnnotations;

namespace DrinksApps.Models
{
    public class CheckoutView
    {
        public List<CarrinhoItem> Itens { get; set; } = new();

        public decimal Total { get; set; }

        [Required(ErrorMessage = "Informe a rua.")]
        public string Rua { get; set; }

        [Required(ErrorMessage = "Informe o número.")]
        public string Numero { get; set; }

        [Required(ErrorMessage = "Informe o bairro.")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Informe a cidade.")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "Informe o CEP.")]
        public string CEP { get; set; }

        [Required(ErrorMessage = "Selecione uma forma de pagamento.")]
        public string FormaPagamento { get; set; }

        [StringLength(300, ErrorMessage = "As observações podem ter no máximo 300 caracteres.")]
        public string? Observacoes { get; set; }
    }
}
