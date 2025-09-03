using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using ApricotProducts.Models;
using Avalonia.Data.Converters;

namespace ApricotProducts.Converters;

/// <summary>
/// Represents the converter for <see cref="ProductVariant">product variant</see>.
/// </summary>
public class ProductVariantConverter : IMultiValueConverter
{
    private readonly Type _productVariantType = typeof(ProductVariant);

    /// <summary>
    /// Converts the given <paramref name="values">value parameters</paramref> to the <see cref="ProductVariant">product variant</see>.
    /// </summary>
    /// <param name="values">The parameters of the product variant</param>
    /// <param name="targetType">The type of the argument or the property</param>
    /// <param name="parameter">The current value of the argument or the property</param>
    /// <param name="culture">The currently used culture/locale</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">The given type is invalid or the given <paramref name="values" /> parameter has invalid values</exception>
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