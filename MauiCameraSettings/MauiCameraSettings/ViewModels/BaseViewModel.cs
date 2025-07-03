using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MauiCameraSettings.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    bool isRefreshing = false;
    public bool IsRefreshing
    {
        get { return isRefreshing; }
        set
        {
            SetProperty(ref isRefreshing, value);
            IsBusy = isRefreshing;
        }
    }

    bool isBusy = false;
    public bool IsBusy
    {
        get { return isBusy; }
        set
        {
            SetProperty(ref isBusy, value);
            IsNotBusy = !isBusy;
            IsBusyChanged();

        }
    }

    bool isNotBusy = true;
    public bool IsNotBusy
    {
        get { return isNotBusy; }
        set { SetProperty(ref isNotBusy, value); }
    }


    bool isDirty = false;
    public bool IsDirty
    {
        get { return isDirty; }
        set
        {
            SetProperty(ref isDirty, value);
        }
    }

    bool showBusyCancelButton = false;
    public bool ShowBusyCancelButton
    {
        get { return showBusyCancelButton; }
        set
        {
            SetProperty(ref showBusyCancelButton, value);
        }
    }

    bool showBusyProgressBar = false;
    public bool ShowBusyProgressBar
    {
        get { return showBusyProgressBar; }
        set
        {
            SetProperty(ref showBusyProgressBar, value);
        }
    }

    float busyProgressPercentage = 0.0f;
    public float BusyProgressPercentage
    {
        get { return busyProgressPercentage; }
        set
        {
            SetProperty(ref busyProgressPercentage, value);
        }
    }

    string title = string.Empty;
    public string Title
    {
        get { return title; }
        set { SetProperty(ref title, value); }
    }

    string emptyText = string.Empty;
    public string EmptyText
    {
        get { return emptyText; }
        set { SetProperty(ref emptyText, value); }
    }
    
    string statusText = string.Empty;
    /// <summary>
    /// Text to be displayed on BusyDialog
    /// </summary>
    /// <remarks>
    /// See BusyDialog
    /// </remarks>
    public string StatusText
    {
        get { return statusText; }
        set { SetProperty(ref statusText, value); }
    }

    string searchQuery = string.Empty;
    public string SearchQuery
    {
        get { return searchQuery; }
        set { SetProperty(ref searchQuery, value); }
    }

    /// <summary>
    /// Will add a done button to toolbar on page to pop modal
    /// </summary>
    /// <remarks>
    /// See MobileCommon.Views.BasePage.xaml.cs
    /// </remarks>
    public bool IsModal { get; set; } = false;

    public string PageName { get; set; } = string.Empty;

    
    /// <summary>
    /// Override this method to Call .ChangeCanExecute() on commands in VM
    /// </summary>
    protected virtual void IsBusyChanged()
    {
        //Override in subclass e.g. SaveCommand.ChangeCanExecute();
    }
    
    public virtual void CleanUp()
    {
        //Unsubscribe from Messaging Center here!
        Unsubscribe();
    }

    /// <summary>
    /// Override to unsubscribe from Messaging Center messages here!
    /// </summary>
    public virtual void Unsubscribe()
    {
        //Unsubscribe from Messaging Center here!
    }

    protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        if (changed == null)
            return;

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    ~BaseViewModel()
    {
        var type = this.GetType();
        string displayName = PageName;
        if (string.IsNullOrEmpty(title))
        {
            Debug.WriteLine($"Garbage Collector cleaned up {type} viewmodel.");
        }
        else
        {
            Debug.WriteLine($"Garbage Collector cleaned up {title} viewmodel({type}).");    
        }
    }
}