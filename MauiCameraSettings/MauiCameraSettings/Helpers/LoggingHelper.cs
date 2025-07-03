using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiCameraSettings.Helpers;

public class LoggingHelper
{
    public static void WriteDebug(string area, string message)
    {
        Debug.WriteLine($"{DateTime.Now:dd/MM/yyyy hh:mm:ss:ffff} - {area} - {message}");
    }
    public static Task<int> CreateErrorLog(string location, string action, string errorDescription, [CallerMemberName] string callerMemberName = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        WriteDebug("LoggingServiceCommon", $"Error Log Created! Location: {location}, Action: {action}\nDescription:\n{errorDescription}");

        return Task.FromResult(1);
    }

    public static Task<int> CreateExceptionLog(string location, string action, Exception ex, string extraInfo = "", [CallerMemberName] string callerMemberName = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        string extra = (!string.IsNullOrEmpty(extraInfo)) ? $"Extra Info: {extraInfo} " : string.Empty;
        return CreateErrorLog(location, action, extra + "Exception: " + ex.Message + " " + ex.InnerException + " Stacktrace:" + ex.StackTrace, callerMemberName, callerFilePath, callerLineNumber);
    }
}   