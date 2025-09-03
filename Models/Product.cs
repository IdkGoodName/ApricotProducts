using System.Collections.Generic;

namespace ApricotProducts.Models;

public class Product
{
    public string Name { get; set; }

    public decimal Price { get; set; }

    public string Description { get; set; }

    public bool IsListed { get; set; }

    public IList<ProductVariant> Variants { get; set; }

    public int VariantPlusMore => Variants.Count - 1;

    public bool HasMoreThanOneVariant => Variants.Count > 1;

    public bool HasVariants => Variants.Count > 0;

    public Product(string name, decimal price, string description, bool listed, IList<ProductVariant> variants) =>
        (Name, Price, Description, IsListed, Variants) = (name, price, description, listed, variants);

    public Product(string name, decimal price, string description, bool listed) : this(name, price, description, listed, []) { }
}