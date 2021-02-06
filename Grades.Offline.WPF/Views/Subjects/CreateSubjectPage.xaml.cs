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

namespace Grades.Offline.WPF.Views.Subjects
{
    /// <summary>
    /// Interaction logic for CreateSubjectPage.xaml
    /// </summary>
    public partial class CreateSubjectPage : Page
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateSubjectPage(Guid? classId)
        {
            _dbContext = new ApplicationDbContext();
            InitializeComponent();
            DataContext = this;

            #region InitialClassList
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));

            // Class selection
            dataTable.Rows.Add(Guid.Empty, "Select a class");
            if (classId.HasValue)
            {
                var @class = _dbContext.Classes.FirstOrDefault(c => c.Id == classId.Value);
                dataTable.Rows.Add(classId, @class.Name);

                ClassSelector.SelectedIndex = 1;
            }
            else
            {
                _dbContext.Classes.ToList().ForEach(c => dataTable.Rows.Add(c.Id, c.Name));
                ClassSelector.SelectedIndex = 0;
            }

            ClassSelector.ItemsSource = dataTable.DefaultView;
            #endregion
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            // These fields are required
            if (!string.IsNullOrWhiteSpace(SubjectNameTextBox.Text) && ClassSelector.SelectedIndex != 0)
            {
                DoneButton.Visibility = Visibility.Collapsed;
                ProgressRing.Visibility = Visibility.Visible;

                var selectedRowElementArray = ((DataRowView)ClassSelector.SelectedItem).Row.ItemArray;
                var classId = (Guid)selectedRowElementArray.ElementAt(0);

                _dbContext.Subjects.Add(new DbSubject
                {
                    Name = SubjectNameTextBox.Text,
                    ClassId = classId
                });

                await _dbContext.SaveChangesAsync();

                DoneButton.Visibility = Visibility.Visible;
                ProgressRing.Visibility = Visibility.Hidden;

                MessageBox.Show("Subject created successfully!", "Grades", MessageBoxButton.OK);
                NavigationService.Navigate(new ClassDetailPage(classId));
            }
        }
    }
}
