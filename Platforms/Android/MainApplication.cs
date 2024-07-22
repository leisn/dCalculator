﻿// @Leisn (https://leisn.com , https://github.com/leisn)

using Android.App;
using Android.Runtime;

namespace dCalculator
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
