using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using ApricotProducts.Managers;
using ApricotProducts.Models;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using ReactiveUI;

namespace ApricotProducts.ViewModels;

public partial class ProductListViewModel : PageViewModelBase
{
    private IDisposable? _productsDisposable, _productVariantsDisposable;

    public ProductManager ProductManager { get; }

    public ObservableCollection<Product> Products => ProductManager.Products;

    public ObservableCollection<ProductVariant> ProductVariants => ProductManager.ProductVariants;

    public ProductListViewModel(MainWindowViewModel parent, ProductManager productManager) : base(parent)
    {
        ProductManager = productManager;
        _productsDisposable = productManager
            .Products
            .ToObservableChangeSet()
            .Subscribe(x =>
                this.RaisePropertyChanged(nameof(Products))
            );
        _productVariantsDisposable = productManager
            .ProductVariants
            .ToObservableChangeSet()
            .Subscribe(x =>
                this.RaisePropertyChanged(nameof(ProductVariants))
            );
    }

    [RelayCommand]
    private void ViewProduct(Product product) =>
        Parent.ViewPageOf(product);

    [RelayCommand]
    private void ViewProductVariant(ProductVariant productVariant) =>
        Parent.ViewPageOf(productVariant);

    [RelayCommand]
    private void PromptAddProduct() =>
        Parent.PromptAddProduct();

    [RelayCommand]
    private void PromptAddProductVariant() =>
        Parent.PromptAddProductVariant();

    public void EditProduct(Product product, string name, string description, decimal price, bool isListed, IList<ProductVariant> productVariants) =>
        ProductManager.EditProduct(product, name, description, price, isListed, productVariants);

    public void EditProductVariant(ProductVariant productVariant, string name, ProductSize size, Color color) =>
        ProductManager.EditProductVariant(productVariant, name, size, color);

    [RelayCommand]
    private void PromptRemoveProduct(Product product)
    {
        Console.WriteLine("Removing product '{0}'", product.Name);
        Parent.PromptRemoveProduct(product);
    }

    [RelayCommand]
    private void RemoveProduct(Product product)
    {
        Console.WriteLine("Removing product '{0}'", product.Name);
        ProductManager.RemoveProduct(product);
    }

    [RelayCommand]
    private void RemoveProductVariant(ProductVariant productVariant) =>
        ProductManager.RemoveProductVariant(productVariant);

    public override void Dispose()
    {
        _productsDisposable?.Dispose();
        _productVariantsDisposable?.Dispose();
    }
}
