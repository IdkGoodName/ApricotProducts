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
    public string Name { get; set; } = name;

    public ProductSize Size { get; set; } = size;

    public Color VariantColor => new(255, R, G, B);

    public IBrush ColorBrush => new SolidColorBrush(VariantColor);

    public byte R
    {
        get => _r;
        set
        {
            this.RaiseAndSetIfChanged(ref _r, value);
            this.RaisePropertyChanged(nameof(ColorBrush));
        }
    }

    public byte G
    {
        get => _g;
        set
        {
            this.RaiseAndSetIfChanged(ref _g, value);
            this.RaisePropertyChanged(nameof(ColorBrush));
        }
    }

    public byte B
    {
        get => _b;
        set
        {
            this.RaiseAndSetIfChanged(ref _b, value);
            this.RaisePropertyChanged(nameof(ColorBrush));
        }
    }

    public ProductVariant? ProductVariant { get; private set; } = productVariant;

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

    public bool HasErrors
    {
        get => _hasErrors;
        set => this.RaiseAndSetIfChanged(ref _hasErrors, value);
    }
    #endregion

    #region Properties(GET only)
    public bool IsSaveButtonEnabled => !HasErrors;

    public bool IsNew => ProductVariant is null;

    public string PageHeader => IsNew ? "Variant Creation Form" : "Variant Edit Form";

    public string PageDescription =>
        IsNew
        ? "You're currently creating a new product variant. Click save to create the product variant."
        : "You're currently editing an existing product variant. Click save to save changes.";
    #endregion

    #region Events
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    #endregion

    #region Constructors
    public VariantDetailViewModel(MainWindowViewModel parent, ProductVariant productVariant) : this(parent, productVariant.Name, productVariant.Size, productVariant.Color, productVariant) =>
        ProductVariant = productVariant;
    #endregion

    #region Methods
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
