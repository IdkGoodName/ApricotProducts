using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ApricotProducts.Models;
using Avalonia.Data.Converters;

namespace ApricotProducts.Converters;

public class ProductConverter : IMultiValueConverter
{
    private readonly Type _productType = typeof(Product);

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