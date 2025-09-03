namespace ApricotProducts.Models;

/// <summary>
/// Represents the different sizes of the <see cref="ProductVariant">product variants</see>.
/// </summary>
/// <seealso cref="ProductVariant" />
/// <seealso cref="ProductVariantSelected" />
public enum ProductSize
{
    /// <summary>
    /// Extra-small-sized product variant.
    /// </summary>
    XS,
    /// <summary>
    /// Small-sized product variant.
    /// </summary>
    S,
    /// <summary>
    /// Medium-sized product variant.
    /// </summary>
    M,
    /// <summary>
    /// Large-sized product variant.
    /// </summary>
    L,
    /// <summary>
    /// Extra-large-sized product variant.
    /// </summary>
    XL,
    /// <summary>
    /// Extra-extra-large-sized product variant.
    /// </summary>
    XXL
}