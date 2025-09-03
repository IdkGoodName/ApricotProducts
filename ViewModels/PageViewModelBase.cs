using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;

namespace ApricotProducts.ViewModels;

/// <summary>
/// Represents the base for all pages in the application.
/// </summary>
/// <param name="parent">The parent window that manages the page</param>
public abstract class PageViewModelBase(MainWindowViewModel parent) : ViewModelBase, IDisposable
{
    /// <summary>
    /// Gets the parent <see cref="MainWindowViewModel">main window view model</see> of the page.
    /// </summary>
    public MainWindowViewModel Parent { get; } = parent;

    /// <summary>
    /// Disposes the page.
    /// </summary>
    public abstract void Dispose();
}
