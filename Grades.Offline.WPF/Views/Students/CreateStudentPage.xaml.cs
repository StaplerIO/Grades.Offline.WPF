using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
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
        }

        // If we do this in constructor, the ClassSelector will be NULL, will throw exceptions
        private void ClassSelector_Loaded(object sender, RoutedEventArgs e)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));

            // Tip selection
            dataTable.Rows.Add(Guid.Empty, "Select a class");

            // Class selection
            _dbContext.Classes.ToList().ForEach(c => dataTable.Rows.Add(c.Id, c.Name));
            ClassSelector.ItemsSource = dataTable.DefaultView;
            ClassSelector.SelectedIndex = 0;
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            DoneButton.Visibility = Visibility.Collapsed;
            ProgressRing.Visibility = Visibility.Visible;

            var selectedRowElementArray = ((DataRowView)ClassSelector.SelectedItem).Row.ItemArray;
            var classId = (Guid)selectedRowElementArray.ElementAt(0);

            _dbContext.Students.Add(new DbStudent
            {
                FullName = StudentNameTextBox.Text,
                Sno = int.Parse(StudentSnoTextBox.Text),
                Class = _dbContext.Classes.FirstOrDefault(c => c.Id == classId)
            });

            await _dbContext.SaveChangesAsync();

            DoneButton.Visibility = Visibility.Visible;
            ProgressRing.Visibility = Visibility.Hidden;
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
