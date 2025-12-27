using MauiMicroSample.Modules;
using MauiMicroSample.Pages;
using MauiMicroSample.Services;
using MauiMicroSample.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;

namespace MauiMicroSample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
#if DEBUG
        var sw = System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine($"[MauiMicroSample] CreateMauiApp starting... (tid={Environment.CurrentManagedThreadId})");
#endif

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiMicroMvvm<AppShell>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        Console.WriteLine($"[MauiMicroSample] Builder configured in {sw.ElapsedMilliseconds}ms");
        Console.WriteLine("[MauiMicroSample] Registering services...");
#endif

        builder.Services.MapView<MainPage, MainPageViewModel>()
            .MapView<DialogDemo, DialogDemoViewModel>()
            .MapView<MessageDemoPage, MessageDemoPageViewModel>()
            .MapView<MessageDisplay, MessageDisplayViewModel>()
            .MapView<MauiInfluencersPage, MauiInfluencersViewModel>()
            .MapView<InfluencerDetail, InfluencerDetailViewModel>()
            .MapView<AppShell, AppShellViewModel>()
            .AddSingleton(Connectivity.Current)
            .AddSingleton(_ => new HttpClient
            {
                BaseAddress = new Uri("https://dansiegel.blob.core.windows.net")
            })
            .AddSingleton(_ => RestService.For<IApiClient>(_.GetRequiredService<HttpClient>()));

        Routing.RegisterRoute($"{nameof(MauiInfluencersPage)}/{nameof(InfluencerDetail)}", typeof(InfluencerDetail));

        builder.Logging.AddConsole();

#if DEBUG
        Console.WriteLine($"[MauiMicroSample] Services registered in {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"[MauiMicroSample] Logging configured in {sw.ElapsedMilliseconds}ms");
#endif

        // On-device Android startup can be very sensitive to heavy synchronous work.
        // The DI validation below builds a second ServiceProvider with ValidateOnBuild and
        // resolves multiple view models, which can significantly delay app startup.
#if DEBUG
        // On-device Android startup can be very sensitive to heavy synchronous work.
        // The DI validation below builds a second ServiceProvider with ValidateOnBuild and
        // resolves multiple view models, which can significantly delay app startup.
#if !ANDROID
        Console.WriteLine("[MauiMicroSample] Validating DI container... (ValidateOnBuild only)");
        using (var sp = builder.Services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        }))
        {
            Console.WriteLine("[MauiMicroSample] DI validation: resolving view models (scoped)...");
            using var scope = sp.CreateScope();
            var scoped = scope.ServiceProvider;

            _ = scoped.GetRequiredService<MainPageViewModel>();
            _ = scoped.GetRequiredService<DialogDemoViewModel>();
            _ = scoped.GetRequiredService<MessageDemoPageViewModel>();
            _ = scoped.GetRequiredService<MessageDisplayViewModel>();
            _ = scoped.GetRequiredService<MauiInfluencersViewModel>();
            _ = scoped.GetRequiredService<InfluencerDetailViewModel>();
            _ = scoped.GetRequiredService<AppShellViewModel>();
            Console.WriteLine("[MauiMicroSample] DI validation: view models resolved.");
        }
        Console.WriteLine("[MauiMicroSample] DI validation passed.");
#else
        Console.WriteLine("[MauiMicroSample] Skipping DI validation on Android (DEBUG).");
#endif
#endif

#if DEBUG
        Console.WriteLine("[MauiMicroSample] Building MauiApp...");
        Console.WriteLine($"[MauiMicroSample] About to call builder.Build() at {sw.ElapsedMilliseconds}ms");
#endif
        var app = builder.Build();

#if DEBUG
        Console.WriteLine($"[MauiMicroSample] builder.Build() completed in {sw.ElapsedMilliseconds}ms");
        Console.WriteLine("[MauiMicroSample] CreateMauiApp completed.");
#endif

        return app;
    }
}
