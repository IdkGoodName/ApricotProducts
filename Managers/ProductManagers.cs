using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using ApricotProducts.Models;
using ReactiveUI;

namespace ApricotProducts.Managers;

public class ProductManager(ObservableCollection<Product> products, ObservableCollection<ProductVariant> productVariants) : ReactiveObject
{
    #region Fields
    private readonly ObservableCollection<Product> _products = products;

    private readonly ObservableCollection<ProductVariant> _productVariants = productVariants;
    #endregion

    #region Properties
    public ObservableCollection<Product> Products => _products;

    public ObservableCollection<ProductVariant> ProductVariants => _productVariants;
    #endregion

    #region Methods
    public void EditProduct(Product product, string name, string description, decimal price, bool isListed, IList<ProductVariant> productVariants)
    {
        // To not change immediately without confirmation
        (product.Name, product.Description, product.Price, product.IsListed, product.Variants) = (name, description, price, isListed, productVariants.ToList());
        this.RaisePropertyChanged(nameof(Products));
    }

    public void EditProductVariant(ProductVariant productVariant, string name, ProductSize size, Color color)
    {
        // To not change immediately without confirmation
        (productVariant.Name, productVariant.Size, productVariant.Color) = (name, size, color);
        this.RaisePropertyChanged(nameof(ProductVariants));
    }

    public void RemoveProduct(Product product) =>
        Products.Remove(product);

    public void RemoveProductVariant(ProductVariant productVariant)
    {
        _productVariants.Remove(productVariant);

        foreach (Product product in _products)
            product.Variants.Remove(productVariant);

        this.RaisePropertyChanged(nameof(Products));
    }
    #endregion
}