using Foundation;
using Microsoft.Maui.Hosting;

namespace MauiCameraSettings;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}