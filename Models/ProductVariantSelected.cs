using System.Drawing;

namespace ApricotProducts.Models;
/// <summary>
/// Represents a selectable <see cref="Models.ProductVariant">product variant</see>.
/// </summary>
/// <param name="productVariant">The product variant that can be selected</param>
/// <param name="isSelected">Whether the product variant is selected</param>
/// <seealso cref="Models.ProductVariant" />
/// <seealso cref="Product" />

public class ProductVariantSelected(ProductVariant productVariant, bool isSelected)
{

    /// <summary>
    /// Gets the <see cref="Models.ProductVariant">product variant</see> that can be selected.
    /// </summary>
    public ProductVariant ProductVariant { get; } = productVariant;

    /// <summary>
    /// Gets the name of the <see cref="Models.ProductVariant">product variant</see>.
    /// </summary>
    public string Name => ProductVariant.Name;

    /// <summary>
    /// Gets the size of the <see cref="Models.ProductVariant">product variant</see>.
    /// </summary>
    public ProductSize Size => ProductVariant.Size;

    /// <summary>
    /// Gets the color <see cref="Models.ProductVariant">product variant</see>.
    /// </summary>
    public Color Color => ProductVariant.Color;

    /// <summary>
    /// Gets the color brush of the <see cref="Models.ProductVariant">product variant</see>.
    /// </summary>
    public Avalonia.Media.IBrush ColorBrush => ProductVariant.ColorBrush;

    /// <summary>
    /// Gets whether the <see cref="Models.ProductVariant">product variant</see> is selected.
    /// </summary>
    public bool IsSelected { get; set; } = isSelected;
}