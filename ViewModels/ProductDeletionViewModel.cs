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
    /// <summary>
    /// Gets the name of the <see cref="Product">product</see>.
    /// </summary>
    public string Name => Product.Name;

    /// <summary>
    /// Gets the <see cref="ProductVariant">variants</see> of the <see cref="Product">product</see>.
    /// </summary>
    public IList<ProductVariant> Variants => Product.Variants;

    /// <summary>
    /// Gets whether the <see cref="Product">product</see> has any <see cref="ProductVariant">variants</see>.
    /// </summary>
    public bool HasVariants => Product.HasVariants;

    /// <summary>
    /// Gets the <see cref="Models.Product">product</see> being deleted.
    /// </summary>
    public Product Product { get; set; } = product;
    #endregion

    #region Properties(GET only)
    /// <summary>
    /// Gets the header/title of the page modal.
    /// </summary>
    public string PageHeader => $"Are you sure you want to delete '{Name}'?";
    #endregion

    #region Methods
    /// <summary>
    /// Disposes the page.
    /// </summary>
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
