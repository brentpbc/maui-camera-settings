using System;

namespace MauiCameraSettings.Helpers;

public class UtilsHelper
{
    public static void ConvertBytesToHumanReadable(double fileSizeInBytes, out double humanReadableSize, out string[] sizes, out int order)
    {
        sizes = new string[] { "B", "KB", "MB", "GB", "TB" };
        double fileSize = fileSizeInBytes;
        order = 0;
        while (fileSize >= 1024 && order < sizes.Length - 1)
        {
            order++;
            fileSize = fileSize / 1024;
        }
        humanReadableSize = fileSize;
    }
    
    public static string ByteStrToHumanReadableStr(double fileSizeInBytes)
    {
        try
        {
            ConvertBytesToHumanReadable(fileSizeInBytes, out double humanReadableSize, out string[] sizes, out int order);
            string result = String.Format("{0:0.##} {1}", humanReadableSize, sizes[order]);
            return result;
        }
        catch (Exception)
        {

            return string.Empty;
        }

    }

    public static string ByteStrToHumanReadableStr(string fileSizeInBytesStr)
    {
        try
        {
            if (!string.IsNullOrEmpty(fileSizeInBytesStr) && Double.TryParse(fileSizeInBytesStr, out double filesizeInBytesD))
            {
                ConvertBytesToHumanReadable(filesizeInBytesD, out double humanReadableSize, out string[] sizes, out int order);
                string result = String.Format("{0:0.##} {1}", humanReadableSize, sizes[order]);
                return result;
            }

            return string.Empty;
        }
        catch (Exception)
        {

            return string.Empty;
        }
    }   
}