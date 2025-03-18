using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using EBISX_POS.Services;
using EBISX_POS.State;
using EBISX_POS.ViewModels;
using EBISX_POS.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EBISX_POS
{
    public partial class App : Application
    {
        // Provides a strongly-typed access to the current instance of App.
        public new static App Current => (App)Application.Current!;

        // The DI container
        public IServiceProvider Services { get; private set; } = null!;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Configure and build the DI container.
            Services = ConfigureServices();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = Services.GetRequiredService<MainWindow>();
                //desktop.MainWindow = Services.GetRequiredService<LogInWindow>();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register services
            services.AddSingleton<AuthService>();
            services.AddSingleton<MenuService>(); // Register MenuService
            services.AddSingleton<OrderService>(); 

            // Register ViewModels
            services.AddTransient<LogInWindowViewModel>();
            services.AddTransient<MainViewModel>(); // Register MainViewModel
            services.AddTransient<ItemListViewModel>(); // Register ItemListViewModel
            services.AddTransient<OrderSummaryViewModel>(); 
            services.AddTransient<SubItemWindowViewModel>(); 

            // Register Views
            services.AddTransient<LogInWindow>();
            services.AddTransient<MainWindow>(); // Register MainWindow
            services.AddTransient<OrderSummaryView>();
            services.AddTransient<ItemListView>(provider => new ItemListView(provider.GetRequiredService<MenuService>())); // Register ItemListView

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Correct way to register ApiSettings from appsettings.json
            services.Configure<ApiSettings>(configuration);

            // Register logging
            services.AddLogging(configure => configure.AddConsole());

            return services.BuildServiceProvider();
        }
    }
}