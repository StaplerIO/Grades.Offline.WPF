using System.Windows;
using System.Windows.Controls;

using Grades.Offline.WPF.Contracts.Views;

using MahApps.Metro.Controls;

namespace Grades.Offline.WPF.Views
{
    public partial class ShellDialogWindow : MetroWindow, IShellDialogWindow
    {
        public ShellDialogWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Frame GetDialogFrame()
            => dialogFrame;

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
