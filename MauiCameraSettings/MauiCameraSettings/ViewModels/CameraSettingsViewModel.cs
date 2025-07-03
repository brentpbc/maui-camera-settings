using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MauiCameraSettings.Helpers;
using MauiCameraSettings.Models.Results;
using MauiCameraSettings.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using IImage = Microsoft.Maui.Graphics.IImage;
using NativeMedia;

namespace MauiCameraSettings.ViewModels;

public class CameraSettingsViewModel : BaseViewModel
{
    string sizeHuman = string.Empty;
    public string SizeHuman
    {
        get { return sizeHuman; }
        set { SetProperty(ref sizeHuman, value); }
    }
    
    double compressionD = 0.0;
    public double CompressionD
    {
        get { return compressionD; }
        set { SetProperty(ref compressionD, value); }
    }

    double photoSizeD = 0.0;
    public double PhotoSizeD
    {
        get { return photoSizeD; }
        set { SetProperty(ref photoSizeD, value); }
    }
    
    bool showImageButton = true;
    public bool ShowImageButton
    {
        get { return showImageButton; }
        set { SetProperty(ref showImageButton, value); }
    }
    
    bool postProcessImage = true;
    public bool PostProcessImage
    {
        get { return postProcessImage; }
        set { SetProperty(ref postProcessImage, value); }
    }
    
    bool restoreOrientationExif = true;
    public bool RestoreOrientationExif
    {
        get { return restoreOrientationExif; }
        set { SetProperty(ref restoreOrientationExif, value); }
    }
    
    bool forceMainThread = false;
    public bool ForceMainThread
    {
        get { return forceMainThread; }
        set { SetProperty(ref forceMainThread, value); }
    }

    public string Version { get; set; } = MauiVersion.VERSION;

    ImageSource photo = null;
    public ImageSource Photo
    {
        get { return photo; }
        set { SetProperty(ref photo, value); }
    }
    
    public byte[] OriginalFileBytes { get; set; }
    public byte[] FinalFileBytes { get; set; }

    public string FileType { get; set; } = string.Empty;

    public Command GetPhotoCommand { get; set; }
    public Command ViewPhotoCommand { get; set; }
    public Command ResetSettingsCommand { get; set; }
    public Command CancelCommand { get; set; }
    public Command ViewImageMetaDataCommand { get; set; }

    public bool IsResetting = false;

    public CameraSettingsViewModel()
    {
        GetPhotoCommand = new Command(async () => await GetPhoto(), () => !IsBusy);
        ViewPhotoCommand = new Command(async () => await ViewPhoto(), () => !IsBusy);
        ResetSettingsCommand = new Command(async () => await ResetSettings(),() => !IsBusy  );
        ViewImageMetaDataCommand = new Command(async () => await ViewImageMetaData(), () => !IsBusy);
        CancelCommand = new Command(async () =>
        {
            await DialogHelper.PopModalPage(nameof(CameraSettingsPage));
        }, () => !IsBusy);
        

        InitialiseSettings();

    }

    void InitialiseSettings()
    {
#if ANDROID
        CompressionD = Constants.Camera.SAVE_PHOTO_COMPRESSION_QLTY_AND;
        PhotoSizeD = Constants.Camera.SAVE_PHOTO_SIZE_PCT_AND;
        RestoreOrientationExif = Constants.Camera.RESTORE_PHOTO_EXIF_AND;
#else
        CompressionD = Constants.Camera.SAVE_PHOTO_COMPRESSION_QLTY;
        PhotoSizeD = Constants.Camera.SAVE_PHOTO_SIZE_PCT;
        RestoreOrientationExif = Constants.Camera.RESTORE_PHOTO_EXIF;
#endif
        
        PostProcessImage = Constants.Camera.POST_PROCESS_PHOTO;

    }

    protected override void IsBusyChanged()
    {
        base.IsBusyChanged();
        GetPhotoCommand.ChangeCanExecute();
        ViewPhotoCommand.ChangeCanExecute();
        ResetSettingsCommand.ChangeCanExecute();
        CancelCommand.ChangeCanExecute();
        ViewImageMetaDataCommand.ChangeCanExecute();
    }
    
    async Task ResetSettings()
    {
        if (IsBusy) return;
        IsResetting = true;
        
        InitialiseSettings();
        
        await UpdateImage();
        await DialogHelper.DisplayMessage("Settings Reset", "Photo Settings have been reset!");
        IsResetting = false;
    }

    async Task GetPhoto()
    {
        string camera = Constants.Photos.PHOTO_SOURCE_CAMERA;
        string photoLibrary = Constants.Photos.PHOTO_SOURCE_LIBRARY;
        List<string> actions = new List<string>()
        {
            camera,
            photoLibrary
        };
        var answer = await DialogHelper.DisplayActionSheet("Select", "Cancel", null, actions.ToArray());
        if (answer == camera)
        {
            await TakePhoto();
        }

        if (answer == photoLibrary)
        {
            await PickPhoto();
        }
    }

    async Task PickPhoto()
    {
        
        if (IsBusy) return;
        IsBusy = true;
        StatusText = "Opening Photo Picker ...";
        
        var cts = new CancellationTokenSource();
        IMediaFile[] files = null;
        try
        {
            var request = new MediaPickRequest(1, MediaFileType.Image)
            {
                PresentationSourceBounds = System.Drawing.Rectangle.Empty, //May Need to update this for iPad?
                UseCreateChooser = true,
                Title = "Select"
            };
            
            Debug.WriteLine("About to open photo picker");
            var results = await MediaGallery.PickAsync(request, cts.Token);
            StatusText = "After Picking Photo";
            Debug.WriteLine("After awaiting photo picker");
            files = results?.Files?.ToArray();
            Debug.WriteLine("Files Count:"+files?.Length);
            
        }
        catch (OperationCanceledException)
        {
            // handling a cancellation request
            StatusText = "Pick Cancelled";
        }
        catch (Exception e)
        {
            // handling other exceptions
            Debug.WriteLine(e);
            StatusText = "Error Picking Photo"+ e.Message +"." + e.InnerException;
            await DialogHelper.DisplayException("Error Picking Photo", e);
            await LoggingHelper.CreateExceptionLog(nameof(CameraSettingsPage), "Picking Photo", e);
        }
        finally
        {
            cts.Dispose();
        }
        
        if (files != null && files.Length > 0)
        {
            var result = await LoadImage(files[0]);
            if (!result.Success)
            {
                await DialogHelper.DisplayErrorMessage("Error Loading Photo", result.Message);
                await LoggingHelper.CreateErrorLog(nameof(CameraSettingsPage), "Loading Photo after Pick", result.Message);
            }    
        }
        
        IsBusy = false;
        StatusText = string.Empty;
    }


    //Wrapped CapturePhoto
    async Task TakePhoto()
    {
        if (IsBusy) return;
        var result = await СapturePhoto();
        if (!result.Success)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                StatusText = string.Empty;
                IsBusy = false;
                await DialogHelper.DisplayErrorMessage("Error Capturing Photo", result.Message);
            });
            
            await LoggingHelper.CreateErrorLog(nameof(CameraSettingsPage), "Capture Photo", result.Message);
        }
    }
    async Task<TaskResult> СapturePhoto()
    {
        CancellationTokenSource cts = null;
        bool success = true;
        string errorMessage = string.Empty;
        try
        {
            if (!MediaGallery.CheckCapturePhotoSupport())
            {
                return TaskResult.Failed("Capture Photo Not Supported");
            }

            var status = await PermissionHelper.CheckAndRequest<Permissions.Camera>(
                "The application needs permission to camera");

            if (!status)
            {
                return TaskResult.Failed("The application did not get the necessary permission to camera");
            }

            cts = new CancellationTokenSource();

            var file = await MediaGallery.CapturePhotoAsync(cts.Token);
            if (file == null)
            {
                return TaskResult.Failed("No Photo was taken.");
            }
            var result = await LoadImage(file);
            if (!result.Success)
            {
                success = false;
                errorMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            success = false;
        }
        finally
        {
            cts?.Dispose();
        }
        
        if (success) return TaskResult.Succeeded();
        
        return TaskResult.Failed(errorMessage);
    }
    
    async Task<TaskResult> LoadImage(IMediaFile file)
    {
        StatusText = "Loading Photo...";
        try
        {
            string fileName = file.NameWithoutExtension; //Can return an null or empty value
            FileType = file.Extension;
            var contentType = file.ContentType;
            
            var stream = await file.OpenReadAsync(); //If writing to a file will need to dispose
            var memoryStream = new MemoryStream(); 
            await stream.CopyToAsync(memoryStream);
            OriginalFileBytes = memoryStream.ToArray();            
            await memoryStream.DisposeAsync();
            await stream.DisposeAsync();
            file.Dispose();
            await UpdateImage();

            ShowImageButton = false;
            return TaskResult.Succeeded();
        }
        catch (Exception e)
        {
            return TaskResult.Failed("Error Loading Photo"+ e.Message +"." + e.InnerException);
        }
    }
    
    public async Task Update(CancellationToken cts)
    {
        Debug.WriteLine("=== Update ===");
        if (IsBusy || OriginalFileBytes == null || OriginalFileBytes.Length == 0 || IsResetting) return;
        if (String.IsNullOrEmpty(FileType))return;
        Debug.WriteLine("About to Cancel token");
        try
        {
            Debug.WriteLine("About to delay by 100 milliseconds");
            await Task.Delay(TimeSpan.FromMilliseconds(100), cts );
            Debug.WriteLine("After to delay by 100 milliseconds");
            
            if (!cts.IsCancellationRequested)
            {
                Debug.WriteLine("About to Update Image");
                await UpdateImage();    
            }
            else
            {
                Debug.WriteLine("*** Cancelled Resize Requested ***");
            }            
        }
        catch (Exception e)
        {
            Debug.WriteLine("*** Resize Exception ***: "+e.Message + e.InnerException);
            Debug.WriteLine(e);
        }
    }

    async Task UpdateImage()
    {
        if (OriginalFileBytes == null || OriginalFileBytes.Length == 0) return;
        if (String.IsNullOrEmpty(FileType))return;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            //Only update Observable collection from the main ui thread
            StatusText = "Updating Image...";
            IsBusy = true;
        });
        MemoryStream memoryStream = null;
        string dimensions = string.Empty;
        try
        {
            if (postProcessImage)
            {
                memoryStream = new MemoryStream(OriginalFileBytes); //Dont dispose or Image.FromSource wont load properly
                memoryStream.Position = 0; //Initialise position to start of stream
                var resizeResult = await ImageHelper.ResizeImageAsync(memoryStream, FileType, 
                    Convert.ToInt32(PhotoSizeD),Convert.ToInt32(CompressionD), RestoreOrientationExif,ForceMainThread);
                if (!resizeResult.Success)
                {
                    await memoryStream.DisposeAsync();
                }
                else
                {
                    var result = resizeResult.Content;
                    FinalFileBytes = result.Bytes;        
                    dimensions = $"{result.Image.Width}Wx{result.Image.Height}H";
                }
            }
            else
            {
                FinalFileBytes = OriginalFileBytes;
                memoryStream = new MemoryStream(OriginalFileBytes); //Dont dispose or Image.FromSource wont load properly
                memoryStream.Position = 0; //Initialise position to start of stream
                var type = FileType.ToLower();
                ImageFormat fmt = ImageFormat.Jpeg;
                if (type == "png")
                {
                    fmt = ImageFormat.Png;
                }
                IImage image = PlatformImage.FromStream(memoryStream, fmt);
                await memoryStream.DisposeAsync();
                dimensions = $"{image.Width}Wx{image.Height}H";
            }
            
            //Get human size
            SizeHuman = UtilsHelper.ByteStrToHumanReadableStr(FinalFileBytes.Length) + " " + dimensions;
            var imageSource = ImageSource.FromStream(
                () =>
                {
                    var ms = new MemoryStream(FinalFileBytes);
                    return ms;
                });
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //Only update Observable collection from the main ui thread
                Photo = imageSource;
            });
            
        }
        catch (Exception e)
        {
            if (memoryStream != null) await memoryStream.DisposeAsync();
            await DialogHelper.DisplayException("Error Updating Image", e);
            await LoggingHelper.CreateExceptionLog(nameof(CameraSettingsPage), "Updating Image", e);
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                StatusText = string.Empty;
            });
        }
    }

    async Task ViewPhoto()
    {
        if (IsBusy) return;
        
        if (FinalFileBytes != null && FinalFileBytes.Length > 0)
        {
            await DialogHelper.NavigateToModalPage("Photo Upload Settings Photo Detail Page", 
                new NavigationPage(
                    new PhotoDetailPage(FinalFileBytes)));
        }
    }
    
    public async Task ViewImageMetaData()
    {
        if (FinalFileBytes == null || FinalFileBytes.Length == 0)
        {
            await DialogHelper.DisplayErrorMessage("No Photo Selected", "Select a photo first to view meta data.");
            return;
        }
        await DialogHelper.DisplayImageMetaDataDialog(FinalFileBytes);
    }    
}