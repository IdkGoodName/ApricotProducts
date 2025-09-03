using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using ApricotProducts.Models;
using ReactiveUI;

namespace ApricotProducts.Managers;

/// <summary>
/// Represents the manager of the application's <see cref="Product">products</see> and their <see cref="ProductVariant">product variants</see>.
/// </summary>
/// <param name="products"></param>
/// <param name="productVariants"></param>
public class ProductManager(ObservableCollection<Product> products, ObservableCollection<ProductVariant> productVariants) : ReactiveObject
{
    #region Fields
    private readonly ObservableCollection<Product> _products = products;

    private readonly ObservableCollection<ProductVariant> _productVariants = productVariants;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the list of <see cref="Product">products</see> in the application.
    /// </summary>
    public ObservableCollection<Product> Products => _products;

    /// <summary>
    /// Gets the list of <see cref="ProductVariant">product variants</see> in the application.
    /// </summary>
    public ObservableCollection<ProductVariant> ProductVariants => _productVariants;
    #endregion

    #region Methods
    /// <summary>
    /// Edits the given <paramref name="product" />.
    /// </summary>
    /// <param name="product">The product being edited</param>
    /// <param name="name">The new name of the product</param>
    /// <param name="description">The new description of the product</param>
    /// <param name="price">The new price of the product</param>
    /// <param name="isListed">The new listing of the product</param>
    /// <param name="productVariants">The new list of <see cref="ProductVariant">variants</see> of the product</param>
    public void EditProduct(Product product, string name, string description, decimal price, bool isListed, IList<ProductVariant> productVariants)
    {
        // To not change immediately without confirmation
        (product.Name, product.Description, product.Price, product.IsListed, product.Variants) = (name, description, price, isListed, productVariants.ToList());
        this.RaisePropertyChanged(nameof(Products));
    }

    /// <summary>
    /// Edits the given <paramref name="productVariant">product variant</paramref>.
    /// </summary>
    /// <param name="productVariant">The product variant being edited</param>
    /// <param name="name">The new name of the product variant</param>
    /// <param name="size">The new size of the product variant</param>
    /// <param name="color">The new color of the product</param>
    public void EditProductVariant(ProductVariant productVariant, string name, ProductSize size, Color color)
    {
        // To not change immediately without confirmation
        (productVariant.Name, productVariant.Size, productVariant.Color) = (name, size, color);
        this.RaisePropertyChanged(nameof(ProductVariants));
    }

    /// <summary>
    /// Removes the given <paramref name="product" /> from the <see cref="Products">product list</see>.
    /// </summary>
    /// <param name="product">The product being removed</param>
    public void RemoveProduct(Product product) =>
        Products.Remove(product);

    /// <summary>
    /// Removes the given <paramref name="productVariant" /> from the <see cref="ProductVariants">product variant list</see>.
    /// </summary>
    /// <param name="productVariant">The product variant being removed</param>
    public void RemoveProductVariant(ProductVariant productVariant)
    {
        _productVariants.Remove(productVariant);

        foreach (Product product in _products)
            product.Variants.Remove(productVariant);

        this.RaisePropertyChanged(nameof(Products));
        this.RaisePropertyChanged(nameof(ProductVariants));
    }
    #endregion
}