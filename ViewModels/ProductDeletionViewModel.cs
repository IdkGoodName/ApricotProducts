using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection.Metadata;
using ApricotProducts.Managers;
using ApricotProducts.Models;
using Avalonia.Controls.Platform;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using ReactiveUI;
using Tmds.DBus.Protocol;

namespace ApricotProducts.ViewModels;

public sealed partial class ProductDeletionViewModel(MainWindowViewModel parent, Product product) : PageViewModelBase(parent)
{
    #region Properties (GET/SET)
    public string Name => Product.Name;

    public IList<ProductVariant> Variants => Product.Variants;

    public bool HasVariants => Product.HasVariants;

    public Product Product { get; set; } = product;
    #endregion

    #region Properties(GET only)
    public string PageHeader => $"Are you sure you want to delete '{Name}'?";
    #endregion

    #region Methods
    public override void Dispose() { }

    [RelayCommand]
    private void GoBackToList() =>
        Parent.PageGoBack();

    [RelayCommand]
    private void RemoveProduct()
    {
        Parent.ProductManager.RemoveProduct(Product);
        GoBackToList();
    }
    #endregion
}
