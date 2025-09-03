using System.Collections.Generic;

namespace ApricotProducts.Models;

/// <summary>
/// Represents a product.
/// </summary>
/// <remarks>
/// <para>Represents a product that can be created and managed via <see cref="Managers.ProductManager" /></para>
/// </remarks>
/// <seealso cref="ProductVariant" />
/// <seealso cref="ProductVariantSelected" />
public class Product
{
    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets the price of the product.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets the description of the product.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets whether the product is publicly listed.
    /// </summary>
    public bool IsListed { get; set; }

    /// <summary>
    /// Gets the list of <see cref="ProductVariant">variants</see> of the product.
    /// </summary>
    public IList<ProductVariant> Variants { get; set; }

    /// <summary>
    /// Gets the amount of <see cref="ProductVariant">variants</see> available with one less <see cref="ProductVariant">variant</see>.
    /// </summary>
    /// <remarks>
    /// <para>This is primarily used in UI, as arithmetic is quite limited and clunky in the XAML.</para>
    /// </remarks>
    public int VariantPlusMore => Variants.Count - 1;

    /// <summary>
    /// Gets whether the amount of <see cref="ProductVariant">variants</see> is more than one.
    /// </summary>
    /// <remarks>
    /// <para>This is primarily used in UI, as arithmetic is quite limited and clunky in the XAML.</para>
    /// </remarks>
    public bool HasMoreThanOneVariant => Variants.Count > 1;

    /// <summary>
    /// Gets whether the product has <see cref="ProductVariant">variants</see>.
    /// </summary>
    /// <remarks>
    /// <para>This is primarily used in UI, as arithmetic is quite limited and clunky in the XAML.</para>
    /// </remarks>
    public bool HasVariants => Variants.Count > 0;

    /// <summary>
    /// Initializes a new instance of <see cref="Product">product</see> with the given <paramref name="variants" />.
    /// </summary>
    /// <param name="name">The name of the product</param>
    /// <param name="price">The price of the product</param>
    /// <param name="description">The description of the product</param>
    /// <param name="listed">Whether the product is listed publicly</param>
    /// <param name="variants">The list of variants of the product</param>
    public Product(string name, decimal price, string description, bool listed, IList<ProductVariant> variants) =>
        (Name, Price, Description, IsListed, Variants) = (name, price, description, listed, variants);

    /// <summary>
    /// Initializes a new instance of <see cref="Product">product</see> without any of its <see cref="ProductVariant">variants</see>.
    /// </summary>
    /// <param name="name">The name of the product</param>
    /// <param name="price">The price of the product</param>
    /// <param name="description">The description of the product</param>
    /// <param name="listed">Whether the product is listed publicly</param>
    public Product(string name, decimal price, string description, bool listed) : this(name, price, description, listed, []) { }
}