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
using Grades.Offline.WPF.Managers;
using Grades.Offline.WPF.Models;

using Microsoft.Extensions.Options;
using Ookii.Dialogs.Wpf;

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
                _appConfig.Theme = AppTheme.Light;
                _themeSelectorService.SetTheme(AppTheme.Light);
            }
        }

        private void OnDarkChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                _appConfig.Theme = AppTheme.Dark;
                _themeSelectorService.SetTheme(AppTheme.Dark);
            }
        }

        private void OnDefaultChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                _appConfig.Theme = AppTheme.Default;
                _themeSelectorService.SetTheme(AppTheme.Default);
            }
        }

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
                UpdateLanguageSettings(AppLanguage.English);
            }
        }
        private void OnChineseChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                UpdateLanguageSettings(AppLanguage.Chinese);
            }
        }

        private void UpdateLanguageSettings(AppLanguage language)
        {
            _appConfig.Language = language;
            ConfigManager.UpdateConfigFile(_appConfig);

            var dialog = new TaskDialog
            {
                WindowTitle = Localization.Resources.DialogTitle,
                MainInstruction = Localization.Resources.Language_changed,
                MainIcon = TaskDialogIcon.Information,
                Content = $"{Localization.Resources.Language_changed}\n{Localization.Resources.RestartApplication}",
                ButtonStyle = TaskDialogButtonStyle.CommandLinks
            };
            var button = new TaskDialogButton("OK");
            dialog.Buttons.Add(button);

            dialog.ShowDialog();
        }
    }
}
