using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiCameraSettings.Helpers;
using MauiCameraSettings.Models;
using MauiCameraSettings.ViewModels;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings.Views;

public partial class BasicListModalPage : ContentPage
{
    BasicListViewModel ViewModel;
    public BasicListModalPage(string title, List<CollectionItem> items, bool canShareItems = false)
    {
        InitializeComponent();
        BindingContext = ViewModel = new BasicListViewModel(title,items,canShareItems);
    }

    async void DoneToolbarItem_Clicked(System.Object sender, System.EventArgs e)
    {
        await DialogHelper.PopModalPage(nameof(BasicListModalPage));
    }
}

public class BasicListViewModel : BaseViewModel
{
    List<CollectionItem> items = new List<CollectionItem>();
    public List<CollectionItem> Items
    {
        get { return items; }
        set { SetProperty(ref items, value); }
    }
    
    bool canShareItems = true;
    public bool CanShareItems
    {
        get { return canShareItems; }
        set { SetProperty(ref canShareItems, value); }
    }

    public Command ShareCommand { get; }
    public BasicListViewModel(string title, List<CollectionItem> items, bool canShareItems)
    {
        Title = title;
        Items = items;                
        CanShareItems = canShareItems;
        ShareCommand = new Command(async () => await ShareOrCopyItems(), ()=> !IsBusy);
    }

    protected override void IsBusyChanged()
    {
        base.IsBusyChanged();
        ShareCommand.ChangeCanExecute();
    }

    async Task ShareOrCopyItems()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in Items)
        {
            sb.AppendLine($"{item.Code} - {item.Description}");
        }
        
        await DialogHelper.ShowShareOrCopy($"Share {Title}", sb.ToString(), Title );
    }
}