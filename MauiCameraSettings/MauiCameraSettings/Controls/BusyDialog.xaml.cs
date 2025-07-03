using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings.Controls;

public partial class BusyDialog : ContentView
{
    public static readonly BindableProperty StatusTextProperty = BindableProperty.Create(
            nameof(StatusText), 
            typeof(string), 
            typeof(BusyDialog));

    public static readonly BindableProperty ShowCancelButtonProperty = BindableProperty.Create(
        nameof(ShowCancelButton),
        typeof(bool),
        typeof(BusyDialog),
        false);

    public static readonly BindableProperty CancelCommandProperty = BindableProperty.Create(
        nameof(CancelCommand),
        typeof(Command),
        typeof(BusyDialog),
        null);

    public static readonly BindableProperty ShowProgressBarProperty = BindableProperty.Create(
        nameof(ShowProgressBar),
        typeof(bool),
        typeof(BusyDialog),
        false);

    public static readonly BindableProperty ProgressBarPercentageProperty = BindableProperty.Create(
        nameof(ProgressBarPercentage),
        typeof(float),
        typeof(BusyDialog),
        0.0f);

    public string StatusText
    {
        get
        {
            return (string)GetValue(StatusTextProperty);
        }

        set
        {
            SetValue(StatusTextProperty, value);
        }            
    }

    public bool ShowCancelButton
    {
        get
        {
            return (bool)GetValue(ShowCancelButtonProperty);
        }

        set
        {
            SetValue(ShowCancelButtonProperty, value);
        }
    }

    public Command CancelCommand
    {
        get
        {
            return (Command)GetValue(CancelCommandProperty);
        }

        set
        {
            SetValue(CancelCommandProperty, value);
        }
    }

    public bool ShowProgressBar
    {
        get
        {
            return (bool)GetValue(ShowProgressBarProperty);
        }

        set
        {
            SetValue(ShowProgressBarProperty, value);
        }
    }

    public float ProgressBarPercentage
    {
        get
        {
            return (float)GetValue(ProgressBarPercentageProperty);
        }

        set
        {
            SetValue(ProgressBarPercentageProperty, value);
        }
    }

    public BusyDialog()
    {
        InitializeComponent();
    }
}