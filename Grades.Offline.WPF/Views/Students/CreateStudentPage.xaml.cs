using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Views.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

namespace Grades.Offline.WPF.Views.Students
{
    /// <summary>
    /// Interaction logic for CreateStudentPage.xaml
    /// </summary>
    public partial class CreateStudentPage : Page
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateStudentPage()
        {
            _dbContext = new ApplicationDbContext();
            InitializeComponent();

            #region InitialClassSelector
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));

            // Tip selection
            dataTable.Rows.Add(Guid.Empty, "Select a class");

            // Class selection
            _dbContext.Classes.ToList().ForEach(c => dataTable.Rows.Add(c.Id, c.Name));
            ClassSelector.ItemsSource = dataTable.DefaultView;
            ClassSelector.SelectedIndex = 0;
            #endregion
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StudentNameTextBox.Text) && !string.IsNullOrWhiteSpace(StudentSnoTextBox.Text) && ClassSelector.SelectedIndex != 0)
            {
                DoneButton.Visibility = Visibility.Collapsed;
                ProgressRing.Visibility = Visibility.Visible;

                var selectedRowElementArray = ((DataRowView)ClassSelector.SelectedItem).Row.ItemArray;
                var classId = (Guid)selectedRowElementArray.ElementAt(0);

                _dbContext.Students.Add(new DbStudent
                {
                    FullName = StudentNameTextBox.Text,
                    Sno = int.Parse(StudentSnoTextBox.Text),
                    ClassId = classId
                });

                await _dbContext.SaveChangesAsync();

                DoneButton.Visibility = Visibility.Visible;
                ProgressRing.Visibility = Visibility.Hidden;

                MessageBox.Show("Stuent created successfully!", "Grades", MessageBoxButton.OK);
                NavigationService.Navigate(new ClassDetailPage(classId));
            }
        }

        // Only allow integers, no decimal or string
        private void StudentSnoTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
        }
    }
}
