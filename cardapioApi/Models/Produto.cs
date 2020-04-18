using System;
using System.Collections.Generic;

namespace cardapioApi.Models
{
    public partial class Produto
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; }
        public string Descricao { get; set; }
        public string ImgUrl { get; set; }
        public decimal Preco { get; set; }
    }
}
