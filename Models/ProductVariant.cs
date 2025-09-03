using System;
using System.Drawing;

namespace ApricotProducts.Models;

public class ProductVariant
{
    public Color Color { get; set; }

    public ProductSize Size { get; set; }

    public string Name { get; set; }

    public Avalonia.Media.Color MediaColor => new(255, Color.R, Color.G, Color.B);

    public Avalonia.Media.IBrush ColorBrush => new Avalonia.Media.SolidColorBrush(MediaColor);

    public ProductVariant(string name, ProductSize productSize, Color color) =>
        (Name, Size, Color) = (name, productSize, color);
}