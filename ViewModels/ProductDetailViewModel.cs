using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using ApricotProducts.Models;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using ReactiveUI;

namespace ApricotProducts.ViewModels;

/// <summary>
/// Represents the page for editing and creating <see cref="Models.Product">products</see>.
/// </summary>
/// <seealso cref="VariantDetailViewModel" />
/// <seealso cref="ProductDeletionViewModel" />
public sealed partial class ProductDetailViewModel : PageViewModelBase, INotifyDataErrorInfo
{
    #region Fields
    // Auto-filled with properties
    private readonly Dictionary<string, bool> _propertyToError = [];

    private bool _hasErrors = false;

    private readonly IDisposable? _eventDisposable;
    #endregion

    /// <summary>
    /// Gets the name of the <see cref="Models.Product">product</see>.
    /// </summary>
    #region Properties (GET/SET)
    [MaxLength(200, ErrorMessage = "Name must be less than or equal to 200 symbols")]
    [MinLength(3, ErrorMessage = "Name must consist of at least 3 characters")]
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets the description of the <see cref="Models.Product">product</see>.
    /// </summary>
    [MaxLength(2000, ErrorMessage = "Description must be less than or equal to 2000 symbols")]
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// Gets the price of the <see cref="Models.Product">product</see>.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets whether the <see cref="Models.Product">product</see> is publicly listed.
    /// </summary>
    public bool IsListed { get; set; }

    /// <summary>
    /// Gets the <see cref="Models.Product">product</see> being managed.
    /// </summary>
    public Product? Product { get; private set; }

    /// <summary>
    /// Gets whether any of the form inputs contains a faulty value.
    /// </summary>
    public bool HasErrors
    {
        get => _hasErrors;
        set => this.RaiseAndSetIfChanged(ref _hasErrors, value);
    }
    #endregion

    #region Properties(GET only)
    /// <summary>
    /// Gets the <see cref="ProductVariantSelected">selected variants</see> of the product.
    /// </summary>
    public IList<ProductVariantSelected> SelectedVariants { get; set; }

    /// <summary>
    /// Gets the resulting selected <see cref="ProductVariant">product variants</see> to change to once saved.
    /// </summary>
    public IList<ProductVariant> NewVariants =>
        SelectedVariants
            .Where(x => x.IsSelected)
            .Select(x => x.ProductVariant)
            .ToList();

    /// <summary>
    /// Gets whether the <c>Save</c> button in the UI is enabled.
    /// </summary>
    /// <remarks>
    /// <para>For now, the save button is always enabled once form inputs have no errors.</para>
    /// </remarks>
    public bool IsSaveButtonEnabled => !HasErrors;

    /// <summary>
    /// Gets whether the product is being created (<see langword="true" />) or is being edited(<see langword="false" />).
    /// </summary>
    public bool IsNew => Product is null;

    /// <summary>
    /// Gets the GUI header text of the page.
    /// </summary>
    public string PageHeader => IsNew ? "Product Creation Form" : "Product Edit Form";

    /// <summary>
    /// Gets the GUI description text of the page.
    /// </summary>
    public string PageDescription =>
        IsNew
        ? "You're currently creating a new product. Click save to create the product."
        : "You're currently editing an existing product. Click save to save changes.";
    #endregion

    #region Events
    /// <summary>
    /// Represents an event whether errors have changed.
    /// </summary>
    /// <remarks>
    /// <para>This never gets fired in this code, as the property changes re-doing the error checks is enough.</para>
    /// </remarks>
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
                Console.WriteLine("Product variant list changed");
                SelectedVariants = FromProductVariants(parent.ProductVariants, variants);
                this.RaisePropertyChanged(nameof(SelectedVariants));
            });
    }

    public ProductDetailViewModel(MainWindowViewModel parent, Product product) : this(parent, product.Name, product.Description, product.Price, product.IsListed, product.Variants, product)
    {
        
    }
    #endregion

    #region Methods
    /// <summary>
    /// Gets whether the new property values have any errors.
    /// </summary>
    /// <param name="propertyName">The name of the property that has updated</param>
    /// <returns>Whether the <paramref name="propertyName">given property</paramref> has any errors</returns>
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

    /// <summary>
    /// Disposes the page.
    /// </summary>
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
    private void PromptAddProductVariant() =>
        Parent.PromptAddProductVariant();

    [RelayCommand]
    private void ViewProductVariant(ProductVariant productVariant) =>
        Parent.ViewPageOf(productVariant);

    [RelayCommand]
    private void RemoveProductVariant(ProductVariant productVariant) =>
        Parent.ProductManager.RemoveProductVariant(productVariant);

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
