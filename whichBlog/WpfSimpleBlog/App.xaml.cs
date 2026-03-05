using Application.Interfaces;
using Application.UseCases;
using Infrastructure.Configuration;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Wpf.ViewModels;
using Wpf.Views;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private IHost? _host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder()
                 .ConfigureAppConfiguration((context, config) =>
                 {
                     config.SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                 })
                .ConfigureServices((context, services) =>
                {
                    // ===== CONFIGURAZIONE FIREBASE =====
                    services.Configure<FirebaseSettings>(
                        context.Configuration.GetSection("Firebase"));


                    // ===== INFRASTRUCTURE LAYER =====
                    //Registra l'interfaccia IBlogRepository
                    services.AddSingleton<IBlogRepository>(sp =>
                    {
                        // Recupera le impostazioni di Firebase dal DI container e crea un'istanza di FirebaseRepository
                        var settings = sp.GetRequiredService<IOptions<FirebaseSettings>>().Value;
                        return new FirebaseRepository(settings.DatabaseUrl);
                    });

                    // ===== APPLICATION LAYER =====
                    services.AddSingleton<IBlogService, BlogService>();

                    // ===== PRESENTATION LAYER (WPF) =====
                    services.AddSingleton<HomeViewModel>();
                    services.AddSingleton<Home>();

                })
                .Build();

            await _host.StartAsync();

            var home = _host.Services.GetRequiredService<Home>();
            home.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if( _host != null )
            {
                await _host.StopAsync();
                _host.Dispose();
            }

            base.OnExit(e);
        }
    }

}
