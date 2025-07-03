using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExifLibrary;
using MauiCameraSettings.Models;
using MauiCameraSettings.Views;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;

namespace MauiCameraSettings.Helpers;

public static class DialogHelper
{
    public static async Task DisplayMessage(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    public static async Task DisplayErrorMessage(string title, string message)
    {
        var msg = message;
        if (msg.Length > Constants.MAX_ALERT_MSG_LENGTH)
        {
            msg = msg.Substring(0, Constants.MAX_ALERT_MSG_LENGTH);
        }

        var page = GetCurrentPage();
        await page.DisplayAlert(title, msg, "OK");
    }

    public static async Task DisplayException(string title, Exception e)
    {
        var msg = e.Message;
        if (msg.Length > Constants.MAX_ALERT_MSG_LENGTH)
        {
            msg = msg.Substring(0, Constants.MAX_ALERT_MSG_LENGTH);
        }
        await Application.Current.MainPage.DisplayAlert(title, msg + ". See error logs for details", "OK");
    }

    public static async Task<bool> AskConfirmation(string title, string message)
    {
        return await Application.Current.MainPage.DisplayAlert(title, message, "Yes", "No");
    }        

    public static async Task<string> DisplayActionSheet(string title, string cancel, string destruction, string[] buttons)
    {
        return await Application.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
    }

    public static async Task NavigateToPage(string pageName, ContentPage page)
    {
        Page currentPage = GetCurrentPage();                                    

        if (currentPage != null)
        {
            await currentPage.Navigation.PushAsync(page);
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error Navigating To Page", $"Error navigating to page {pageName}", "OK");
            await LoggingHelper.CreateErrorLog("DialogService", "NavigateToPage", $"Error navigating to page {pageName}");
        }
    }

    public static async Task NavigateBack(string pageName)
    {
        Page currentPage = GetCurrentPage();

        if (currentPage != null)
        {
            await currentPage.Navigation.PopAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error Navigating Backwards", $"Error navigating backwards from page {pageName}", "OK");
            await LoggingHelper.CreateErrorLog("DialogService", "NavigateBack", $"Error navigating backwards from page {pageName}");
        }
    }

    public static async Task InsertPageBeforeCurrentPage(string pageName, Page page, bool popCurrentPage)
    {
        Page currentPage = GetCurrentPage();
        if (currentPage != null)
        {
            if (currentPage is NavigationPage navPage)
            {
                currentPage = navPage.CurrentPage;
            }
            currentPage.Navigation.InsertPageBefore(page, currentPage);

            if (popCurrentPage)
            {
                await currentPage.Navigation.PopAsync();
            }
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error Navigating To Modal Page", $"Error navigating to modal page {pageName}", "OK");
            await LoggingHelper.CreateErrorLog("DialogService", "NavigateToModalPage", $"Error navigating to modal page {pageName}");
        }            
    }

    public static async Task NavigateToModalPage(string pageName, NavigationPage page)
    {
        Page currentPage = GetCurrentPage();            
        if (currentPage != null)
        {
            await currentPage.Navigation.PushModalAsync(page);
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error Navigating To Modal Page", $"Error navigating to modal page {pageName}", "OK");
            await LoggingHelper.CreateErrorLog("DialogService", "NavigateToModalPage", $"Error navigating to modal page {pageName}");
        }
    }

    public static async Task PopModalPage(string pageName)
    {
        Page currentPage = GetCurrentPage();

        if (currentPage != null)
        {
            if (currentPage.Navigation.ModalStack.Count > 0)
            {
                await currentPage.Navigation.PopModalAsync();    
            }
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error Poping Modal Page", $"Error poping modal page {pageName}", "OK");
            await LoggingHelper.CreateErrorLog("DialogService", "PopModalPage", $"Error poping modal page {pageName}");
        }
    }

    private static Page GetCurrentPage()
    {
        Page currentPage = null;
        if (Application.Current.MainPage is FlyoutPage)
        {
            currentPage = ((NavigationPage)((FlyoutPage)Application.Current.MainPage).Detail).CurrentPage;
        }
        else if (Application.Current.MainPage is TabbedPage)
        {
            currentPage = ((TabbedPage)Application.Current.MainPage).CurrentPage;
        }
        else if (Application.Current.MainPage is NavigationPage)
        {
            currentPage = ((NavigationPage)Application.Current.MainPage).CurrentPage;
        }

        if (currentPage == null) return null;
        //Check to see if there is a modal page being displayed atm and use that as the current page to push/pop new pages
        if (currentPage.Navigation.ModalStack != null && currentPage.Navigation.ModalStack.Count > 0)
        {
            currentPage = currentPage.Navigation.ModalStack.Last();
        }

        return currentPage;
    }

    public static async Task DisplayShareDialog(string title, string textToShare, string subject = "")
    {
        await Share.RequestAsync(new ShareTextRequest
        {
            Title = title,
            Text = textToShare,
            Subject = subject,
            PresentationSourceBounds = DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Tablet
                        ? new Microsoft.Maui.Graphics.Rect(0, 20, 0, 0)
                        : Microsoft.Maui.Graphics.Rect.Zero
        });
    }

    public static async Task DisplayFileShareDialog(string title, string filePath)
    {
        await Share.RequestAsync(new ShareFileRequest
        {
            Title = title,
            File = new ShareFile(filePath),
            PresentationSourceBounds = DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Tablet
                ? new Rect(0, 20, 0, 0)
                : Rect.Zero
        });
    }

    public static async Task CopyToClipboard(string clipboardText, string analyticsCopyEvent = "")
    {
        await Clipboard.SetTextAsync(clipboardText);
    }
    
    public static async Task ShowShareOrCopy(
        string title,
        string shareText,
        string shareTitle = "",
        string analyticsCopyEvent = "",
        string analyticsShareEvent = "")
    {
        if (string.IsNullOrEmpty(shareText))
        {
            await DisplayMessage($"Nothing to share", $"Nothing to share, share text is empty.");
            return;
        }

        List<string> actions = new List<string>() { Constants.DialogActions.COPY_ACTION, Constants.DialogActions.SHARE_ACTION };
        string action = await DisplayActionSheet(title, "Cancel", null, actions.ToArray());
        if (action == Constants.DialogActions.COPY_ACTION)
        {
            await Clipboard.SetTextAsync(shareText);
        }

        if (action == Constants.DialogActions.SHARE_ACTION)
        {
            await DisplayShareDialog(shareTitle, shareText, shareTitle);
        }
    }

    //http://oozcitak.github.io/exiflibrary/
    public static async Task DisplayImageMetaDataDialog(byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            await DisplayErrorMessage("No image bytes provided", "Error image bytes are null or empty.");
            return;
        }
        List<CollectionItem> metaData = new List<CollectionItem>();
        string template = CollectionItem.Templates.DESCR_ONLY_TEMPLATE;
        try
        {
            ImageFile file = ImageFile.FromBuffer(imageBytes);
            foreach (var property in file.Properties)
            {
                metaData.Add(new CollectionItem() { Code = property.Name, Description = property.Name + ": "+property.Value.ToString(), TemplateName = template});
            }
        }
        catch (Exception ex)
        {
            await DisplayException("Error Loading Metadata", ex);
            return;
        }
    
        await NavigateToModalPage(
            "Image Metadata",
            new NavigationPage(
                new BasicListModalPage(
                    "Image Meta Data",
                    metaData,
                    canShareItems:true)));
    }
}