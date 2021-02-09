using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using Grades.Offline.WPF.Contracts.Services;
using Grades.Offline.WPF.Contracts.Views;
using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models;
using Grades.Offline.WPF.Services;
using Grades.Offline.WPF.Views;
using Grades.Offline.WPF.Views.Classes;
using Grades.Offline.WPF.Views.Exams;
using Grades.Offline.WPF.Views.Students;
using Grades.Offline.WPF.Views.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Grades.Offline.WPF
{
    // For more inforation about application lifecyle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview

    // WPF UI elements use language en-US by default.
    // If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
    // Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
    public partial class App : Application
    {
        private IHost _host;

        public T GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T;

        public App()
        {
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
            _host = Host.CreateDefaultBuilder(e.Args)
                    .ConfigureAppConfiguration(c =>
                    {
                        c.SetBasePath(appLocation);
                    })
                    .ConfigureServices(ConfigureServices)
                    .Build();

            await _host.StartAsync();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {

            // TODO WTS: Register your services and pages here

            // App Host
            services.AddHostedService<ApplicationHostService>();

            // Services
            services.AddSingleton<IWindowManagerService, WindowManagerService>();
            services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IRightPaneService, RightPaneService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Views
            services.AddTransient<IShellWindow, ShellWindow>();

            services.AddTransient<DashboardPage>();

            services.AddTransient<CreateClassPage>();
            services.AddTransient<ClassDetailPage>();
            services.AddTransient<ClassListPage>();

            services.AddTransient<CreateExamPage>();
            services.AddTransient<ScoreRecordWindow>();
            services.AddTransient<ExamDetailPage>();

            services.AddTransient<CreateStudentPage>();
            services.AddTransient<StudentDetailPage>();

            services.AddTransient<CreateSubjectPage>();
            services.AddTransient<SubjectDetailPage>();

            services.AddTransient<SettingsPage>();

            services.AddTransient<AboutWindow>();

            // Configuration
            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));

            // Database
            // services.AddDbContext<ApplicationDbContext>();
            new ApplicationDbContext().Database.Migrate();
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
        }
    }
}
