using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
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
        private readonly ApplicationDbContext _dbContext;

        public CreateClassPage()
        {
            InitializeComponent();
            DataContext = this;

            _dbContext = new ApplicationDbContext();
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            DoneButton.Visibility = Visibility.Collapsed;
            ProgressRing.Visibility = Visibility.Visible;
            if (!string.IsNullOrWhiteSpace(ClassNameTextBox.Text))
            {
                _dbContext.Classes.Add(new DbClass
                {
                    Name = ClassNameTextBox.Text
                });
                await _dbContext.SaveChangesAsync();

                // Tell user that the class has been created
                MessageBox.Show("Class created successfully!", "Grades", MessageBoxButton.OK);

                NavigationService.Navigate(new ClassDetailPage());
            }

            DoneButton.Visibility = Visibility.Visible;
            ProgressRing.Visibility = Visibility.Hidden;
        }
    }
}
