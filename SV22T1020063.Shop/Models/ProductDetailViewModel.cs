using SV22T1020063.Models.Catalog;

namespace SV22T1020063.Shop.Models
{
    /// <summary>
    /// ViewModel for the Product Detail page.
    /// </summary>
    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = new Product();
        public List<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
        public List<ProductPhoto> Photos { get; set; } = new List<ProductPhoto>();
        public List<Product> SimilarProducts { get; set; } = new List<Product>();
    }
}
