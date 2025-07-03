using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MauiCameraSettings.Helpers;
using MauiCameraSettings.ViewModels;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings.Views;

public partial class CameraSettingsPage : ContentPage
{
    public CameraSettingsViewModel ViewModel { get; set; }
    public CameraSettingsPage()
    {
        InitializeComponent();
        BindingContext = ViewModel = new CameraSettingsViewModel();
    }
    
    CancellationTokenSource source;
    void CompressionSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var newStep = Math.Round(e.NewValue / 1);
        CompressionSlider.Value = newStep * 1;
    }
    
    void CompressionSlider_OnDragCompleted(object sender, EventArgs e)
    {
        UpdateImage();
    }

    void PhotoSizeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var newStep = Math.Round(e.NewValue / 1);
        PhotoSizeSlider.Value = newStep * 1;
    }
    
    void PhotoSizeSlider_OnDragCompleted(object sender, EventArgs e)
    {
        UpdateImage();
    }

    void ProcessImage_OnToggled(object sender, ToggledEventArgs e)
    {
        UpdateImage();
    }
    
    void UpdateImage()
    {
        _ = source?.CancelAsync();
        source?.Dispose();
        source = new CancellationTokenSource();
        var token = source.Token;
        _ = Task.Run(async () => await ViewModel.Update(token),token);
    }

    void RestoreOrientationSwitch_OnToggled(object sender, ToggledEventArgs e)
    {
        UpdateImage();
    }
}