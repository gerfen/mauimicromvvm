using Android.App;
using Android.Runtime;

namespace MauiMicroSample
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
#if DEBUG
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                Android.Util.Log.Error("MauiMicroSample", $"UnhandledException: {e.ExceptionObject}");

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                Android.Util.Log.Error("MauiMicroSample", $"UnobservedTaskException: {e.Exception}");
                e.SetObserved();
            };
#endif
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}