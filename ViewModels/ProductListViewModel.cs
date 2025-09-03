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

    /// <summary>
    /// Gets the manager of the application's <see cref="Product">products</see> and their <see cref="ProductVariant">product variants</see>.
    /// </summary>
    public ProductManager ProductManager { get; }

    /// <summary>
    /// Gets the list of <see cref="Product">products</see> in the application.
    /// </summary>
    public ObservableCollection<Product> Products => ProductManager.Products;

    /// <summary>
    /// Gets the list of <see cref="ProductVariant">product variants</see> in the application.
    /// </summary>
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

    /// <summary>
    /// Adds a new page to the <see cref="PageStack">page stack</see> and makes it visible.
    /// </summary>
    /// <remarks>
    /// <para>The new added page will be added as <see cref="ProductDetailViewModel" />.</para>
    /// </remarks>
    [RelayCommand]
    private void ViewProduct(Product product) =>
        Parent.ViewPageOf(product);

    /// <summary>
    /// Adds a new page to the <see cref="PageStack">page stack</see> and makes it visible.
    /// </summary>
    /// <remarks>
    /// <para>The new added page will be added as <see cref="VariantDetailViewModel" />.</para>
    /// </remarks>
    [RelayCommand]
    private void ViewProductVariant(ProductVariant productVariant) =>
        Parent.ViewPageOf(productVariant);

    /// <summary>
    /// Adds a new product creation prompt page to the <see cref="PageStack">page stack</see>.
    /// </summary>
    /// <remarks>
    /// <para>The new added page will be added as <see cref="ProductDetailViewModel" />.</para>
    /// </remarks>
    [RelayCommand]
    private void PromptAddProduct() =>
        Parent.PromptAddProduct();

    /// <summary>
    /// Adds a new product variant creation prompt page to the <see cref="PageStack">page stack</see>.
    /// </summary>
    /// <remarks>
    /// <para>The new added page will be added as <see cref="VariantDetailViewModel" />.</para>
    /// </remarks>
    [RelayCommand]
    private void PromptAddProductVariant() =>
        Parent.PromptAddProductVariant();

    /// <summary>
    /// Edits the given <paramref name="product" />.
    /// </summary>
    /// <param name="product">The product being edited</param>
    /// <param name="name">The new name of the product</param>
    /// <param name="description">The new description of the product</param>
    /// <param name="price">The new price of the product</param>
    /// <param name="isListed">The new listing of the product</param>
    /// <param name="productVariants">The new list of <see cref="ProductVariant">variants</see> of the product</param>
    public void EditProduct(Product product, string name, string description, decimal price, bool isListed, IList<ProductVariant> productVariants) =>
        ProductManager.EditProduct(product, name, description, price, isListed, productVariants);

    /// <summary>
    /// Edits the given <paramref name="productVariant">product variant</paramref>.
    /// </summary>
    /// <param name="productVariant">The product variant being edited</param>
    /// <param name="name">The new name of the product variant</param>
    /// <param name="size">The new size of the product variant</param>
    /// <param name="color">The new color of the product</param>
    public void EditProductVariant(ProductVariant productVariant, string name, ProductSize size, Color color) =>
        ProductManager.EditProductVariant(productVariant, name, size, color);

    /// <summary>
    /// Adds a new product deletion prompt modal page to the <see cref="PageStack">page stack</see>.
    /// </summary>
    [RelayCommand]
    private void PromptRemoveProduct(Product product) =>
        Parent.PromptRemoveProduct(product);

    [RelayCommand]
    private void RemoveProduct(Product product) =>
        ProductManager.RemoveProduct(product);

    [RelayCommand]
    private void RemoveProductVariant(ProductVariant productVariant) =>
        ProductManager.RemoveProductVariant(productVariant);

    public override void Dispose()
    {
        _productsDisposable?.Dispose();
        _productVariantsDisposable?.Dispose();
    }
}
