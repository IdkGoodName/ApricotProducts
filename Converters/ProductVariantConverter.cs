using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using ApricotProducts.Models;
using Avalonia.Data.Converters;

namespace ApricotProducts.Converters;

public class ProductVariantConverter : IMultiValueConverter
{
    private readonly Type _productVariantType = typeof(ProductVariant);

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType != _productVariantType || values.Count < 1)
            throw new NotSupportedException();
        else if (
            values.First() is string name
        )
            return new ProductVariant(name, ProductSize.M, Color.White);

        throw new NotSupportedException();
    }
}