using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiCameraSettings.Helpers;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings.Views;

public partial class PhotoDetailPage : ContentPage
{
    public byte[] PhotoBytes { get; set; }        
    ImageSource PhotoImageSource { get; set; }
    string FilePath { get; set; }
    bool IsLocalPath { get; set; }
    public PhotoDetailPage(byte[] photoBytes, string title = "")
    {
        InitializeComponent();
        PhotoBytes = photoBytes;
        Title = title;
    }

    public PhotoDetailPage(ImageSource imageSource, string title = "")
    {
        InitializeComponent();
        PhotoImageSource = imageSource;
        Title = title;
    }

    public PhotoDetailPage(string filePath, bool isLocalPath = false, string title = "")
    {
        InitializeComponent();
        FilePath = filePath;
        IsLocalPath = isLocalPath;
        Title = title;
    }

    public PhotoDetailPage(string localFilePath, string webUrl, string title = "")
    {
        InitializeComponent();
        if (!string.IsNullOrEmpty(localFilePath))
        {
            DetailImage.Source = ImageSource.FromFile(localFilePath);
        }
        else if (!string.IsNullOrEmpty(webUrl))
        {
            DetailImage.Source = ImageSource.FromUri(new Uri(webUrl));
        }
        Title = title;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (PhotoBytes != null)
        {
            DetailImage.Source = ImageSource.FromStream(() =>
            {
                return new MemoryStream(PhotoBytes);
            });
            return;
        }

        if (PhotoImageSource != null)
        {
            DetailImage.Source = PhotoImageSource;
            return;
        }


        if (!string.IsNullOrEmpty(FilePath))
        {
            if (IsLocalPath)
            {
                string fullPath = UtilsHelper.GetFullAppDataPath(FilePath);
                DetailImage.Source = ImageSource.FromFile(fullPath);
            }
            else
            {
                DetailImage.Source = ImageSource.FromFile(FilePath);
            }

            return;
        }

    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await DialogHelper.PopModalPage(nameof(PhotoDetailPage));
    }
}