using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Models.DbModels;
using Grades.Offline.WPF.Views.Classes;
using Ookii.Dialogs.Wpf;
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

                var subject = new DbSubject
                {
                    Name = SubjectNameTextBox.Text,
                    ClassId = classId
                };

                _dbContext.Subjects.Add(subject);

                await _dbContext.SaveChangesAsync();

                DoneButton.Visibility = Visibility.Visible;
                ProgressRing.Visibility = Visibility.Hidden;

                var dialog = new TaskDialog();
                dialog.WindowTitle = "Dialog - Grades";
                dialog.MainInstruction = "Subject created";
                dialog.MainIcon = TaskDialogIcon.Information;
                dialog.Content = $"You have created subject \"{subject.Name}\" successfully!";
                dialog.ExpandedInformation = $"The subject belongs to class \"{selectedRowElementArray.ElementAt(1)}\"";
                dialog.ButtonStyle = TaskDialogButtonStyle.CommandLinks;
                var ignoreButton = new TaskDialogButton("Continue creating subject");
                var proceedButton = new TaskDialogButton("Go to class page");
                dialog.Buttons.Add(ignoreButton);
                dialog.Buttons.Add(proceedButton);

                var result = dialog.ShowDialog(Window.GetWindow(this));

                if (result == proceedButton)
                {
                    NavigationService.Navigate(new ClassDetailPage(classId));
                }

                SubjectNameTextBox.Text = string.Empty;

                DoneButton.Visibility = Visibility.Visible;
                ProgressRing.Visibility = Visibility.Hidden;
                
            }
        }
    }
}
