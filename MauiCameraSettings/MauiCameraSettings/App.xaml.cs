﻿using MauiCameraSettings.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new CameraSettingsPage()));
    }
}