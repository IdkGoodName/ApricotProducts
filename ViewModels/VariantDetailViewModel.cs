using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ApricotProducts.Models;
using Avalonia.Controls.Platform;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace ApricotProducts.ViewModels;

/// <summary>
/// Represents the page for editing and creating <see cref="Models.ProductVariant">product variants</see>.
/// </summary>
/// <param name="parent">The parent window of the page</param>
/// <param name="name">The name of the product variant</param>
/// <param name="size">The size of the product variant</param>
/// <param name="color">The color of the product variant</param>
/// <param name="productVariant">The product variant being edited</param>
/// <seealso cref="ProductDetailViewModel" />
public sealed partial class VariantDetailViewModel(MainWindowViewModel parent, string name, ProductSize size, System.Drawing.Color color, ProductVariant? productVariant = null)
    : PageViewModelBase(parent), INotifyDataErrorInfo
{
    #region Fields
    private byte _r = color.R, _g = color.G, _b = color.B;

    private readonly IEnumerable<string> _availableSizeNames = Enum
        .GetNames<ProductSize>()
        .Select(x => x.ToLower())
        .ToArray();

    // Auto-filled with properties
    private readonly Dictionary<string, bool> _propertyToError = [];

    private string _sizeText = size.ToString();

    private bool _hasErrors = false;
    #endregion

    #region Properties (GET/SET)
    /// <summary>
    /// Gets the name of the <see cref="Models.ProductVariant">product variant</see>.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets the size of the <see cref="Product">product</see>.
    /// </summary>
    public ProductSize Size { get; set; } = size;

    /// <summary>
    /// Gets the Avalonia color of the <see cref="Product">product</see>.
    /// </summary>
    public Color VariantColor => new(255, R, G, B);

    /// <summary>
    /// Gets the Avalonia color brush of the <see cref="Product">product</see>.
    /// </summary>
    public IBrush ColorBrush => new SolidColorBrush(VariantColor);

    /// <summary>
    /// Gets the red channel value of the <see cref="VariantColor">product's color</see>.
    /// </summary>
    public byte R
    {
        get => _r;
        set
        {
            this.RaiseAndSetIfChanged(ref _r, value);
            this.RaisePropertyChanged(nameof(ColorBrush));
        }
    }

    /// <summary>
    /// Gets the green channel value of the <see cref="VariantColor">product's color</see>.
    /// </summary>
    public byte G
    {
        get => _g;
        set
        {
            this.RaiseAndSetIfChanged(ref _g, value);
            this.RaisePropertyChanged(nameof(ColorBrush));
        }
    }

    /// <summary>
    /// Gets the blue channel value of the <see cref="VariantColor">product's color</see>.
    /// </summary>
    public byte B
    {
        get => _b;
        set
        {
            this.RaiseAndSetIfChanged(ref _b, value);
            this.RaisePropertyChanged(nameof(ColorBrush));
        }
    }

    /// <summary>
    /// Gets the <see cref="Models.ProductVariant">product variant</see> being managed.
    /// </summary>
    public ProductVariant? ProductVariant { get; private set; } = productVariant;

    /// <summary>
    /// Gets the <see cref="Size"> of the <see cref="Product">product</see> in the text form.
    /// </summary>
    /// <remarks>
    /// <para>This is primarily used in the inputs of the GUI</para>
    /// </remarks>
    public string SizeText
    {
        get => _sizeText;
        set
        {
            _sizeText = value;
            Enum.TryParse(typeof(ProductSize), value, true, out object? size);

            if (size is not null)
                Size = (ProductSize)size;
        }
    }

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
    /// Gets whether the <c>Save</c> button in the UI is enabled.
    /// </summary>
    /// <remarks>
    /// <para>For now, the save button is always enabled once form inputs have no errors.</para>
    /// </remarks>
    public bool IsSaveButtonEnabled => !HasErrors;

    /// <summary>
    /// Gets whether the product variant is being created (<see langword="true" />) or is being edited(<see langword="false" />).
    /// </summary>
    public bool IsNew => ProductVariant is null;

    /// <summary>
    /// Gets the GUI header text of the page.
    /// </summary>
    public string PageHeader => IsNew ? "Variant Creation Form" : "Variant Edit Form";

    /// <summary>
    /// Gets the GUI description text of the page.
    /// </summary>
    public string PageDescription =>
        IsNew
        ? "You're currently creating a new product variant. Click save to create the product variant."
        : "You're currently editing an existing product variant. Click save to save changes.";
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
    /// <summary>
    /// Initializes a new page with a <paramref name="productVariant">product variant</paramref> that is being edited.
    /// </summary>
    /// <param name="parent">The parent window of the page</param>
    /// <param name="productVariant">The product variant being edited</param>
    public VariantDetailViewModel(MainWindowViewModel parent, ProductVariant productVariant) : this(parent, productVariant.Name, productVariant.Size, productVariant.Color, productVariant) =>
        ProductVariant = productVariant;
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
            nameof(Name) =>
                string.IsNullOrWhiteSpace(Name)
                ? ["Name cannot be empty"]
                : (Name.Length < 3 || Name.Length > 200)
                ? ["Name must consist of at least 3 symbols and cannot be more than 200 symbols"]
                : [],
            nameof(SizeText) =>
                _availableSizeNames.Contains(SizeText.ToLower())
                ? []
                : ["Invalid size name"],
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
    public override void Dispose() { }

    [RelayCommand]
    private void GoBackToList() =>
        Parent.PageGoBack();

    [RelayCommand]
    private void AddOrEditVariant()
    {
        if (IsNew)
            AddVariant();
        else
            EditVariant();
        GoBackToList();
    }

    private void AddVariant() =>
        Parent.ProductListPage.ProductVariants.Add(new ProductVariant(Name, Size, System.Drawing.Color.FromArgb(255, R, G, B)));

    private void EditVariant() =>
        // To not change immediately without confirmation
        Parent.EditProductVariant(ProductVariant!, Name, Size, System.Drawing.Color.FromArgb(255, R, G, B));
    #endregion
}
