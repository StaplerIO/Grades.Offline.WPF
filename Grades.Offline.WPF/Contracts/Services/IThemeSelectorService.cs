using System;

using Grades.Offline.WPF.Models;

namespace Grades.Offline.WPF.Contracts.Services
{
    public interface IThemeSelectorService
    {
        void InitializeTheme();

        void SetTheme(AppTheme theme);

        AppTheme GetCurrentTheme();
    }
}
