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

public sealed partial class ProductDetailViewModel : PageViewModelBase, INotifyDataErrorInfo
{
    #region Fields
    // Auto-filled with properties
    private readonly Dictionary<string, bool> _propertyToError = [];

    private bool _hasErrors = false;

    private IDisposable? _eventDisposable;
    #endregion

    #region Properties (GET/SET)
    [MaxLength(200, ErrorMessage = "Name must be less than or equal to 200 symbols")]
    [MinLength(3, ErrorMessage = "Name must consist of at least 3 characters")]
    [Required]
    public string Name { get; set; }

    [MaxLength(2000, ErrorMessage = "Description must be less than or equal to 2000 symbols")]
    [Required]
    public string Description { get; set; }

    public decimal Price { get; set; }

    public bool IsListed { get; set; }

    public Product? Product { get; private set; }

    public bool HasErrors
    {
        get => _hasErrors;
        set => this.RaiseAndSetIfChanged(ref _hasErrors, value);
    }
    #endregion

    #region Properties(GET only)
    public IList<ProductVariantSelected> SelectedVariants { get; set; }

    public IList<ProductVariant> NewVariants =>
        SelectedVariants
            .Where(x => x.IsSelected)
            .Select(x => x.ProductVariant)
            .ToList();

    public bool IsSaveButtonEnabled => !HasErrors;

    public bool IsNew => Product is null;

    public string PageHeader => IsNew ? "Product Creation Form" : "Product Edit Form";

    public string PageDescription =>
        IsNew
        ? "You're currently creating a new product. Click save to create the product."
        : "You're currently editing an existing product. Click save to save changes.";
    #endregion

    #region Events
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    #endregion

    #region Constructors
    public ProductDetailViewModel(MainWindowViewModel parent, string name, string description, decimal price, bool isListed, IList<ProductVariant> variants, Product? product = null) : base(parent)
    {
        (Name, Description, Price, IsListed, Product) = (name, description, price, isListed, product);
        SelectedVariants = FromProductVariants(parent.ProductVariants, variants);
        _eventDisposable = parent
            .ProductManager
            .ProductVariants
            .ToObservableChangeSet()
            .Subscribe(x =>
            {
                Console.WriteLine("Product list changed");
                if (x is not null)
                    SelectedVariants = FromProductVariants(parent.ProductVariants, variants);
            });
    }

    public ProductDetailViewModel(MainWindowViewModel parent, Product product) : this(parent, product.Name, product.Description, product.Price, product.IsListed, product.Variants, product)
    {
        
    }
    #endregion

    #region Methods
    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName is null)
            return Enumerable.Empty<string>();

        // Dictionary would be slower in this case
        IList<string> errors = propertyName switch
        {
            nameof(Price) =>
                Price < 0 ? ["Price cannot be negative"] : [],
            nameof(Name) =>
                string.IsNullOrWhiteSpace(Name)
                ? ["Name cannot be empty"]
                : (Name.Length < 3 || Name.Length > 200)
                ? ["Name must consist of at least 3 symbols and cannot be more than 200 symbols"]
                : [],
            nameof(Description) =>
                string.IsNullOrWhiteSpace(Description)
                ? ["Description cannot be empty"]
                : Description.Length > 2000
                ? ["Description cannot be more than 2000 symbols"]
                : [],
            _ => []
        };

        // For buttons
        _propertyToError[propertyName] = errors.Count > 0;
        HasErrors = _propertyToError.ContainsValue(true);
        this.RaisePropertyChanged(nameof(IsSaveButtonEnabled));
        return errors;
    }

    public override void Dispose() =>
        _eventDisposable?.Dispose();

    [RelayCommand]
    private void GoBackToList() =>
        Parent.PageGoBack();

    [RelayCommand]
    private void AddOrEditProduct()
    {
        if (IsNew)
            AddProduct();
        else
            EditProduct();
        GoBackToList();
    }

    [RelayCommand]
    public void PromptAddProductVariant() =>
        Parent.PromptAddProductVariant();

    private void AddProduct()
    {
        Console.WriteLine("Adding product with variants: [{0}]", string.Join(", ", NewVariants.Select(x => $"\"{x.Name}\" {x.Size} {x.Color}")));
        Parent.ProductListPage.Products.Add(new Product(Name, Price, Description, IsListed, NewVariants));
    }

    private void EditProduct() =>
        // To not change immediately without confirmation
        Parent.EditProduct(Product!, Name, Description, Price, IsListed, NewVariants);
    #endregion

    #region Static methods
    private static IList<ProductVariantSelected> FromProductVariants(IList<ProductVariant> all, IList<ProductVariant> containsVariants) =>
        all
            .Select(x =>
                new ProductVariantSelected(x, containsVariants.Contains(x))
            )
            .ToList();
    #endregion
}
