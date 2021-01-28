using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grades.Offline.WPF.Views.Classes
{
    /// <summary>
    /// Interaction logic for CreateClassPage.xaml
    /// </summary>
    public partial class CreateClassPage : Page
    {
        public CreateClassPage()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
