using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;

namespace ApricotProducts.ViewModels;

public abstract class PageViewModelBase(MainWindowViewModel parent) : ViewModelBase, IDisposable
{
    public MainWindowViewModel Parent { get; } = parent;

    public abstract void Dispose();
}
