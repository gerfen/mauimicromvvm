using MauiMicroMvvm;
using MauiMicroMvvm.Behaviors;
using MauiMicroMvvm.Internals;
using INavigation = MauiMicroMvvm.INavigation;

namespace Microsoft.Maui.Hosting;

public static class MauiMicroBuilderExtensions
{
    public static MauiAppBuilder UseMauiMicroMvvm<TShell>(this MauiAppBuilder builder)
        where TShell : Shell
    {
        builder.Services
            .AddSingleton<TShell>()
            .AddSingleton<IWindowCreator, WindowCreator<TShell>>()
            .AddSingleton<IViewFactory, ViewFactory>()
            .AddSingleton<IBehaviorFactory, BehaviorFactory>()
            .AddSingleton<INavigation, DefaultNavigation<TShell>>()
            .AddSingleton<IPageDialogs, PageDialogs<TShell>>()
            .AddScoped<ViewModelContext>();
        return builder;
    }

    public static IServiceCollection MapView<TView, TViewModel>(this IServiceCollection services, string? key = null)
        where TView : VisualElement
        where TViewModel : class
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(TView).Name;

        if (typeof(TView).IsAssignableTo(typeof(Page)))
            Routing.RegisterRoute(key, typeof(TView));

        return typeof(TView).IsAssignableTo(typeof(Shell))
            ? services.AddSingleton<TView>(sp =>
            {
                var viewFactory = sp.GetRequiredService<IViewFactory>();
                var view = viewFactory.CreateView<TView>();
                viewFactory.Configure(view);
                return view;
            })
            : services.AddTransient<TView>(sp =>
            {
                var viewFactory = sp.GetRequiredService<IViewFactory>();
                var view = viewFactory.CreateView<TView>();
                viewFactory.Configure(view);
                return view;
            })
            .AddSingleton(new ViewMapping(key, typeof(TView), typeof(TViewModel)))
            .AddTransient<TViewModel>();
    }

    public static IServiceCollection ApplyBehavior<TView, TBehavior>(this IServiceCollection services)
        where TView : VisualElement
        where TBehavior : Behavior
    {
        return services.AddTransient<TBehavior>()
            .AddSingleton<RegisteredBehavior<TView, TBehavior>>();
    }

    public static IServiceCollection ApplyBehavior<TView>(this IServiceCollection services, Action<IServiceProvider, TView> onAttached, Action<IServiceProvider, TView>? onDetached = null)
        where TView : VisualElement
    {
        onDetached ??= delegate { };
        return services.AddSingleton(new DelegateViewBehavior<TView>(onAttached, onDetached));
    }

    public static IServiceCollection ApplyBehavior<TView>(this IServiceCollection services, Action<TView> onAttached, Action<TView>? onDetached = null)
        where TView : VisualElement
    {
        onDetached ??= delegate { };
        return services.AddSingleton(new DelegateViewBehavior<TView>((_, view) => onAttached(view), (_, view) => onDetached(view)));
    }
}