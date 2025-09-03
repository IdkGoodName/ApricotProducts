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

    public ProductManager ProductManager { get; } = new ProductManager([], []);

    public ProductListViewModel ProductListPage => _productListPage;

    public ObservableCollection<Product> Products => ProductManager.Products;

    public ObservableCollection<ProductVariant> ProductVariants => ProductManager.ProductVariants;

    public Stack<PageViewModelBase> PageStack
    {
        get => _pageStack;
        set => this.RaiseAndSetIfChanged(ref _pageStack, value);
    }

    public ViewModelBase CurrentPage => PageStack.Peek();

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

    [RelayCommand]
    public void PageGoBack()
    {
        Console.WriteLine("Popping from page stack: {0}", PageStack.Peek());
        PageStack.Pop().Dispose();
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    public void ViewPageOf(Product product)
    {
        Console.WriteLine("Viewing product: '{0}'", product.Name);
        PageStack.Push(new ProductDetailViewModel(this, product));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    public void ViewPageOf(ProductVariant productVariant)
    {
        Console.WriteLine("Viewing product variant: '{0}'", productVariant.Name);
        PageStack.Push(new VariantDetailViewModel(this, productVariant));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    [RelayCommand]
    public void PromptAddProduct()
    {
        Console.WriteLine("Prompting add product");
        PageStack.Push(new ProductDetailViewModel(this, "", "", 20.99m, true, []));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    [RelayCommand]
    public void PromptAddProductVariant()
    {
        Console.WriteLine("Prompting add product variant");
        PageStack.Push(new VariantDetailViewModel(this, "", ProductSize.M, Color.White));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    [RelayCommand]
    public void PromptRemoveProduct(Product product)
    {
        Console.WriteLine("Prompting remove product '{0}'", product.Name);
        PageStack.Push(new ProductDeletionViewModel(this, product));
        this.RaisePropertyChanged(nameof(PageStack));
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    public void EditProduct(Product product, string name, string description, decimal price, bool isListed, IList<ProductVariant> productVariants) =>
        ProductManager.EditProduct(product, name, description, price, isListed, productVariants);

    public void EditProductVariant(ProductVariant productVariant, string name, ProductSize size, Color color) =>
        ProductManager.EditProductVariant(productVariant, name, size, color);
}
