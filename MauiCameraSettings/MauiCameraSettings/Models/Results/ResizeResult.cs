using Microsoft.Maui.Graphics;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace MauiCameraSettings.Models.Results;

public class ResizeResult
{
    public byte[] Bytes { get; set; }
    public IImage Image { get; set; }
}