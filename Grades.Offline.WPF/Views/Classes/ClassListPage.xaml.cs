using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.ViewModels;
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
    /// Interaction logic for ClassListPage.xaml
    /// </summary>
    public partial class ClassListPage : Page
    {
        public ClassListPage()
        {
            InitializeComponent();

            var dbContext = new ApplicationDbContext();
            var classList = new List<ClassViewModel>();
            foreach (var @class in dbContext.Classes)
            {
                classList.Add(new ClassViewModel
                {
                    Id = @class.Id,
                    Name = @class.Name
                });
            }

            ClassList.ItemsSource = classList;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = ((ListViewItem) sender).Content as ClassViewModel;
            NavigationService.Navigate(new ClassDetailPage(viewModel.Id));
        }
    }
}
