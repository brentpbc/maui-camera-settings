using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiCameraSettings.Models;

public class BaseModel : INotifyPropertyChanged
{

    string originalValue = string.Empty;
    public string OriginalValue
    {
        get { return originalValue; }
        set { SetProperty(ref originalValue, value); }
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
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        if (changed == null)
            return;

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    public virtual void SetOriginalValue()
    {
        OriginalValue = string.Empty;
    }

    public virtual string GetCurrentValue()
    {
        return string.Empty;
    }

    public virtual void ResetValue()
    {            
    }
}