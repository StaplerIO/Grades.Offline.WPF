using System.Windows.Controls;

using Grades.Offline.WPF.Behaviors;

using MahApps.Metro.Controls;

namespace Grades.Offline.WPF.Contracts.Views
{
    public interface IShellWindow
    {
        Frame GetNavigationFrame();

        void ShowWindow();

        void CloseWindow();

        Frame GetRightPaneFrame();

        SplitView GetSplitView();

        RibbonTabsBehavior GetRibbonTabsBehavior();
    }
}
