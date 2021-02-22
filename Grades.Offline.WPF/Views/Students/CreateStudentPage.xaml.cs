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

namespace Grades.Offline.WPF.Views.Students
{
    /// <summary>
    /// Interaction logic for CreateStudentPage.xaml
    /// </summary>
    public partial class CreateStudentPage : Page
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateStudentPage(Guid? classId)
        {
            _dbContext = new ApplicationDbContext();
            InitializeComponent();

            #region InitialClassSelector
            var dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Id", typeof(Guid)));
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)));

            // Class selection
            dataTable.Rows.Add(Guid.Empty, Localization.Resources.SelectClass);
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
            // These fieldsa are required
            if (!string.IsNullOrWhiteSpace(StudentNameTextBox.Text) && !string.IsNullOrWhiteSpace(StudentSnoTextBox.Text) && ClassSelector.SelectedIndex != 0)
            {
                DoneButton.Visibility = Visibility.Collapsed;
                ProgressRing.Visibility = Visibility.Visible;

                var selectedRowElementArray = ((DataRowView)ClassSelector.SelectedItem).Row.ItemArray;
                var classId = (Guid)selectedRowElementArray.ElementAt(0);

                var student = new DbStudent
                {
                    FullName = StudentNameTextBox.Text,
                    Sno = int.Parse(StudentSnoTextBox.Text),
                    ClassId = classId
                };

                _dbContext.Students.Add(student);

                await _dbContext.SaveChangesAsync();

                DoneButton.Visibility = Visibility.Visible;
                ProgressRing.Visibility = Visibility.Hidden;

                var dialog = new TaskDialog();
                dialog.WindowTitle = Localization.Resources.DialogTitle;
                dialog.MainInstruction = Localization.Resources.StudentCreateSuccess;
                dialog.MainIcon = TaskDialogIcon.Information;
                dialog.Content = $"{Localization.Resources.Full_name} : {student.FullName}";
                dialog.ExpandedInformation = $"{Localization.Resources.StudentOwner} \"{selectedRowElementArray.ElementAt(1)}\"";
                dialog.ButtonStyle = TaskDialogButtonStyle.CommandLinks;
                var ignoreButton = new TaskDialogButton(Localization.Resources.Continue_creating_student);
                var proceedButton = new TaskDialogButton(Localization.Resources.GoClass);
                dialog.Buttons.Add(ignoreButton);
                dialog.Buttons.Add(proceedButton);

                var result = dialog.ShowDialog(Window.GetWindow(this));

                if (result == proceedButton)
                {
                    NavigationService.Navigate(new ClassDetailPage(classId));
                }

                StudentNameTextBox.Text = string.Empty;

                DoneButton.Visibility = Visibility.Visible;
                ProgressRing.Visibility = Visibility.Hidden;
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
