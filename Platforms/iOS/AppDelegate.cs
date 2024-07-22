// @Leisn (https://leisn.com , https://github.com/leisn)

using Foundation;

namespace dCalculator.Platforms.iOS;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }
}
