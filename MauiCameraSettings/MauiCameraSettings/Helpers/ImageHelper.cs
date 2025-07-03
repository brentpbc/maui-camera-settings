using System;
using System.IO;
using System.Threading.Tasks;
using ExifLibrary;
using MauiCameraSettings.Models.Results;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using IImage = Microsoft.Maui.Graphics.IImage;
namespace MauiCameraSettings.Helpers;

public class ImageHelper
{
    /// <summary>
    /// Resize an image
    /// </summary>
    /// <param name="memStream">Stream containing original image</param>
    /// <param name="imageType">Image Type</param>
    /// <param name="resizeValue">Resize Value 0 - 100, 0=Very small, 100=Original Image Size</param>
    /// <param name="compression">Compression 0 - 100, 0=No Compression, 100=100% Compression</param>
    /// <param name="restoreExifOrientation">True = restore EXIF Orientation metadata</param>
    /// <returns>Return a new memory stream with Resized Image</returns>
    public static async Task<TypedTaskResult<byte[]>> ResizeImageAsync(MemoryStream memStream, string imageType, int resizeValue, int compression, bool restoreExifOrientation)
    {
        if (memStream == null)
        {
            return TypedTaskResult<byte[]>.Failed("Error memstream is null");
        }
        
        //Clamp Resize Values
        resizeValue = Math.Clamp(resizeValue, 1, 100);
        
        var type = imageType.ToLower();
        ImageFormat fmt = ImageFormat.Jpeg;
        if (type == "png")
        {
            fmt = ImageFormat.Png;
        }

        Orientation originalOrientation = Orientation.Normal;
        bool hasOrientation = false;
        if (restoreExifOrientation)
        {
            //Read image metadata
            var file = await ImageFile.FromStreamAsync(memStream);
            if (file.Properties.Contains(ExifTag.Orientation))
            {
                hasOrientation = true;
                originalOrientation = file.Properties.Get<ExifEnumProperty<Orientation>>(ExifTag.Orientation); //http://oozcitak.github.io/exiflibrary/    
            }
            memStream.Position = 0;    
        }
        
        
        IImage image = PlatformImage.FromStream(memStream, fmt);
        if (image != null)
        {
            float resAmount =  resizeValue * .01f;
            float newWidth = image.Width * resAmount;
            float newHeight = image.Height * resAmount;
            IImage newImage = image.Downsize(newWidth, newHeight, false);

            var resizedImageStream = new MemoryStream();
            
            int qlty = Convert.ToInt32(Constants.Camera.MAX_COMPRESSION_QLTY - compression);
            float quality = Math.Clamp(qlty * .01f,0.1f,1f); //Convert to fraction
        
            await newImage.SaveAsync(resizedImageStream, ImageFormat.Jpeg, quality);
            
            // Reset destination stream position to 0 if saving to a file
            resizedImageStream.Position = 0;
            
            if (restoreExifOrientation && hasOrientation)
            {
                var resizedFile = await ImageFile.FromStreamAsync(resizedImageStream);
                var resizedOrientation = resizedFile.Properties.Get<ExifEnumProperty<Orientation>>(ExifTag.Orientation);
                if (resizedOrientation == null || originalOrientation != resizedOrientation)
                {
                    resizedFile.Properties.Set(ExifTag.Orientation, (ushort)originalOrientation);    
                }
                resizedImageStream.Position = 0;
                await resizedFile.SaveAsync(resizedImageStream);
                resizedImageStream.Position = 0;
            }
            
            return TypedTaskResult<byte[]>.Succeeded(resizedImageStream.ToArray());
        }
        
        return TypedTaskResult<byte[]>.Failed("Error resizing image, image retrieved from stream was null");
    }
}