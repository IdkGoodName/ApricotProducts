using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using ApricotProducts.Managers;
using ApricotProducts.Models;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using ReactiveUI;

namespace ApricotProducts.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    private readonly ProductListViewModel _productListPage;

    private Stack<PageViewModelBase> _pageStack = new();

    /// <summary>
    /// Gets the manager of the application's <see cref="Product">products</see> and their <see cref="ProductVariant">product variants</see>.
    /// </summary>
    public ProductManager ProductManager { get; } = new ProductManager([], []);

    /// <summary>
    /// Gets the primary product list page of the application.
    /// </summary>
    public ProductListViewModel ProductListPage => _productListPage;

    /// <summary>
    /// Gets the list of <see cref="Product">products</see> in the application.
    /// </summary>
    public ObservableCollection<Product> Products => ProductManager.Products;

    /// <summary>
    /// Gets the list of <see cref="ProductVariant">product variants</see> in the application.
    /// </summary>
    public ObservableCollection<ProductVariant> ProductVariants => ProductManager.ProductVariants;

    /// <summary>
    /// Gets the stack of pages currently being viewed and managed in the application.
    /// </summary>
    public Stack<PageViewModelBase> PageStack
    {
        get => _pageStack;
        set => this.RaiseAndSetIfChanged(ref _pageStack, value);
    }

    /// <summary>
    /// Gets the currently visible page in the application.
    /// </summary>
    public ViewModelBase CurrentPage => PageStack.Peek();

    /// <summary>
    /// Initializes the current window's context view model.
    /// </summary>
    public MainWindowViewModel()
    {
        ProductManager
            .WhenAnyPropertyChanged(nameof(ProductManager.Products))
            .Subscribe(x =>
                this.RaisePropertyChanged(nameof(Products))
            );
        ProductManager
            .WhenAnyPropertyChanged(nameof(ProductManager.ProductVariants))
            .Subscribe(x =>
                this.RaisePropertyChanged(nameof(ProductVariants))
            );
        _pageStack.Push(_productListPage = new ProductListViewModel(this, ProductManager));
    }

    /// <summary>
    /// Removes the current page from the <see cref="PageStack">page stack</see> and focuses on the new top-most remaining page.
    /// </summary>
    [RelayCommand]
    public void PageGoBack()
    {
        Console.WriteLine("Popping from page stack: {0}", PageStack.Peek());
        PageStack.Pop().Dispose();
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    /// <summary>
    /// Adds a new page to the <see cref="PageStack">page stack</see> and makes it visible.
    /// </summary>
    /// <remarks>
    /// <para>The new added page will be added as <see cref="ProductDetailViewModel" />.</para>
    /// </remarks>
    public void ViewPageOf(Product product)
    {
        Console.WriteLine("Viewing product: '{0}'", product.Name);
        PageStack.Push(new ProductDetailViewModel(this, product));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    /// <summary>
    /// Adds a new page to the <see cref="PageStack">page stack</see> and makes it visible.
    /// </summary>
    /// <remarks>
    /// <para>The new added page will be added as <see cref="VariantDetailViewModel" />.</para>
    /// </remarks>
    public void ViewPageOf(ProductVariant productVariant)
    {
        Console.WriteLine("Viewing product variant: '{0}'", productVariant.Name);
        PageStack.Push(new VariantDetailViewModel(this, productVariant));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    /// <summary>
    /// Adds a new product creation prompt page to the <see cref="PageStack">page stack</see>.
    /// </summary>
    /// <remarks>
    /// <para>The new added page will be added as <see cref="ProductDetailViewModel" />.</para>
    /// </remarks>
    [RelayCommand]
    public void PromptAddProduct()
    {
        Console.WriteLine("Prompting add product");
        PageStack.Push(new ProductDetailViewModel(this, "", "", 20.99m, true, []));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    /// <summary>
    /// Adds a new product variant creation prompt page to the <see cref="PageStack">page stack</see>.
    /// </summary>
    /// <remarks>
    /// <para>The new added page will be added as <see cref="VariantDetailViewModel" />.</para>
    /// </remarks>
    [RelayCommand]
    public void PromptAddProductVariant()
    {
        Console.WriteLine("Prompting add product variant");
        PageStack.Push(new VariantDetailViewModel(this, "", ProductSize.M, Color.White));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    /// <summary>
    /// Adds a new product deletion prompt modal page to the <see cref="PageStack">page stack</see>.
    /// </summary>
    [RelayCommand]
    public void PromptRemoveProduct(Product product)
    {
        Console.WriteLine("Prompting remove product '{0}'", product.Name);
        PageStack.Push(new ProductDeletionViewModel(this, product));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

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
}
