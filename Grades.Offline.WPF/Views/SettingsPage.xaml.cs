using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using Grades.Offline.WPF.Contracts.Services;
using Grades.Offline.WPF.Contracts.Views;
using Grades.Offline.WPF.Models;

using Microsoft.Extensions.Options;

namespace Grades.Offline.WPF.Views
{
    public partial class SettingsPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly AppConfig _appConfig;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISystemService _systemService;
        private readonly IApplicationInfoService _applicationInfoService;
        private bool _isInitialized;
        private AppTheme _theme;
        private AppLanguage _language;
        private string _versionDescription;

        public AppTheme Theme
        {
            get { return _theme; }
            set { Set(ref _theme, value); }
        }

        public new AppLanguage Language
        {
            get { return _language; }
            set { Set(ref _language, value); }
        }

        public string VersionDescription
        {
            get { return _versionDescription; }
            set { Set(ref _versionDescription, value); }
        }

        public SettingsPage(IOptions<AppConfig> appConfig, IThemeSelectorService themeSelectorService, ISystemService systemService, IApplicationInfoService applicationInfoService)
        {
            _appConfig = appConfig.Value;
            _themeSelectorService = themeSelectorService;
            _systemService = systemService;
            _applicationInfoService = applicationInfoService;
            InitializeComponent();
            DataContext = this;
        }

        public void OnNavigatedTo(object parameter)
        {
            VersionDescription = $"Grades.Offline.WPF - {_applicationInfoService.GetVersion()}";
            Theme = _themeSelectorService.GetCurrentTheme();
            _isInitialized = true;
        }

        public void OnNavigatedFrom()
        {
        }

        private void OnLightChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                _themeSelectorService.SetTheme(AppTheme.Light);
            }
        }

        private void OnDarkChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                _themeSelectorService.SetTheme(AppTheme.Dark);
            }
        }

        private void OnDefaultChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                _themeSelectorService.SetTheme(AppTheme.Default);
            }
        }

        private void OnPrivacyStatementClick(object sender, RoutedEventArgs e)
            => _systemService.OpenInWebBrowser(_appConfig.PrivacyStatement);

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void OnEnglishChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                Localization.Resources.Culture = new CultureInfo("en-US");
                RestartApplication();
            }
        }
        private void OnChineseChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                Localization.Resources.Culture = new CultureInfo("zh-CN");
                RestartApplication();
            }
        }

        private void RestartApplication()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
