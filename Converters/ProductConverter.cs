using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ApricotProducts.Models;
using Avalonia.Data.Converters;

namespace ApricotProducts.Converters;

/// <summary>
/// Represents a converter for <see cref="Product">products</see>.
/// </summary>
public class ProductConverter : IMultiValueConverter
{
    private readonly Type _productType = typeof(Product);

    /// <summary>
    /// Converts the given <paramref name="values">value parameters</paramref> to the <see cref="Product">product</see>.
    /// </summary>
    /// <param name="values">The parameters of the product</param>
    /// <param name="targetType">The type of the argument or the property</param>
    /// <param name="parameter">The current value of the argument or the property</param>
    /// <param name="culture">The currently used culture/locale</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">The given type is invalid or the given <paramref name="values" /> parameter has invalid values</exception>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType != _productType || values.Count < 4)
            throw new NotSupportedException();
        else if (
            values.First() is string name &&
            values.ElementAt(1) is decimal price &&
            values.ElementAt(2) is bool listed &&
            values.ElementAt(3) is string description
        )
            return new Product(name, price, description, listed);

        throw new NotSupportedException();
    }
}