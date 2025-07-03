using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings.Controls;

public class PinchTapPanContainer : ContentView
{
    public static readonly BindableProperty HideCommandProperty = BindableProperty.Create(nameof(HideCommand), typeof(Command), typeof(PinchTapPanContainer));
    
    public Command HideCommand
    {
        get
        {
            return (Command)GetValue(HideCommandProperty);
        }

        set
        {
            SetValue(HideCommandProperty, value);
        }
    }
    
    private double _startScale, _currentScale;
    private double _startX, _startY;
    private double _xOffset, _yOffset;

    public double MinScale { get; set; } = 1;
    public double MaxScale { get; set; } = 4;
    
    private TapGestureRecognizer tap;
    private PinchGestureRecognizer pinch;
    private PanGestureRecognizer pan;
    

    public PinchTapPanContainer()
    {
        tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        tap.Tapped += OnTapped;
        GestureRecognizers.Add(tap);

        pinch = new PinchGestureRecognizer();
        pinch.PinchUpdated += OnPinchUpdated;
        GestureRecognizers.Add(pinch);

        pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(pan);
    }

    public void Cleanup()
    {
        tap.Tapped -= OnTapped;
        pinch.PinchUpdated -= OnPinchUpdated;
        pan.PanUpdated -= OnPanUpdated;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        RestoreScaleValues();
        Content.AnchorX = 0.5;
        Content.AnchorY = 0.5;

        base.OnSizeAllocated(width, height);
    }

    private void RestoreScaleValues()
    {
        Content.ScaleTo(MinScale, 250, Easing.CubicInOut);
        Content.TranslateTo(0, 0, 250, Easing.CubicInOut);

        _currentScale = MinScale;
        _xOffset = Content.TranslationX = 0;
        _yOffset = Content.TranslationY = 0;
    }

    private void OnTapped(object sender, EventArgs e)
    {
        if (Content.Scale > MinScale)
        {
            RestoreScaleValues();
            if (HideCommand != null)
            {
                HideCommand.Execute(null);    
            }
        }
        else
        {
            StartScaling();
            ExecuteScaling(MaxScale, .5, .5);
            EndGesture();
        }
    }

    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                StartScaling();
                break;

            case GestureStatus.Running:
                ExecuteScaling(e.Scale, e.ScaleOrigin.X, e.ScaleOrigin.Y);
                break;

            case GestureStatus.Completed:
                EndGesture();
                if (_currentScale == 1.0f && HideCommand != null)
                {
                    HideCommand.Execute(null);
                }
                break;
        }
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _startX = e.TotalX;
                _startY = e.TotalY;

                Content.AnchorX = 0;
                Content.AnchorY = 0;

                break;

            case GestureStatus.Running:
                var maxTranslationX = Content.Scale * Content.Width - Content.Width;
                Content.TranslationX = Math.Min(0, Math.Max(-maxTranslationX, _xOffset + e.TotalX - _startX));

                var maxTranslationY = Content.Scale * Content.Height - Content.Height;
                Content.TranslationY = Math.Min(0, Math.Max(-maxTranslationY, _yOffset + e.TotalY - _startY));

                break;

            case GestureStatus.Completed:
                EndGesture();
                break;
        }
    }

    private void StartScaling()
    {
        _startScale = Content.Scale;

        Content.AnchorX = 0;
        Content.AnchorY = 0;
    }

    private void ExecuteScaling(double scale, double x, double y)
    {
        _currentScale += (scale - 1) * _startScale;
        _currentScale = Math.Max(MinScale, _currentScale);
        _currentScale = Math.Min(MaxScale, _currentScale);

        var deltaX = (Content.X + _xOffset) / Width;
        var deltaWidth = Width / (Content.Width * _startScale);
        var originX = (x - deltaX) * deltaWidth;

        var deltaY = (Content.Y + _yOffset) / Height;
        var deltaHeight = Height / (Content.Height * _startScale);
        var originY = (y - deltaY) * deltaHeight;

        var targetX = _xOffset - (originX * Content.Width) * (_currentScale - _startScale);
        var targetY = _yOffset - (originY * Content.Height) * (_currentScale - _startScale);

        Content.TranslationX = double.Clamp(targetX, -Content.Width * (_currentScale - 1), 0);
        Content.TranslationY = double.Clamp(targetY, -Content.Height * (_currentScale - 1), 0);

        Content.Scale = _currentScale;
    }

    private void EndGesture()
    {
        _xOffset = Content.TranslationX;
        _yOffset = Content.TranslationY;
    }

}