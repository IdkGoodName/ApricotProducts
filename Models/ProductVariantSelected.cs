using System.Drawing;

namespace ApricotProducts.Models;

public class ProductVariantSelected(ProductVariant productVariant, bool isSelected)
{
    public ProductVariant ProductVariant { get; } = productVariant;

    public string Name => ProductVariant.Name;

    public ProductSize Size => ProductVariant.Size;

    public string SizeText => Size.ToString();

    public Color Color => ProductVariant.Color;

    public Avalonia.Media.IBrush ColorBrush => ProductVariant.ColorBrush;

    public bool IsSelected { get; set; } = isSelected;
}