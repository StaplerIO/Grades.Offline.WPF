using Grades.Offline.WPF.Data;
using Grades.Offline.WPF.Views.Classes;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for CreateMultipleStudents.xaml
    /// </summary>
    public partial class CreateMultipleStudents : Page
    {
        private DataTable _students;
        private readonly ApplicationDbContext _dbContext;

        public CreateMultipleStudents(Guid? classId)
        {
            _dbContext = new ApplicationDbContext();
            InitializeComponent();

            // Init list
            _students = new DataTable();
            _students.Columns.Add("Name");
            _students.Columns.Add("Sno");

            _students.NewRow();
            StudentList.ItemsSource = _students.DefaultView;

            #region InitializeClassSelector
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

        private void NewEntryButton_Click(object sender, RoutedEventArgs e)
        {
            _students.NewRow();
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRowElementArray = ((DataRowView)ClassSelector.SelectedItem).Row.ItemArray;
            var classId = (Guid)selectedRowElementArray.ElementAt(0);

            foreach (DataRow student in _students.Rows)
            {
                if (!int.TryParse((string)student["Sno"], out _))
                {
                    var illegalSnoDialog = new TaskDialog
                    {
                        WindowTitle = Localization.Resources.DialogTitle,
                        MainInstruction = "Sno number must be an integer",
                        Content = $"Sno \"{(string)student["Sno"]}\" is not an integer",
                        MainIcon = TaskDialogIcon.Warning,
                        ButtonStyle = TaskDialogButtonStyle.Standard
                    };
                    illegalSnoDialog.Buttons.Add(new TaskDialogButton("Okay"));
                    illegalSnoDialog.ShowDialog(Window.GetWindow(this));

                    return;
                }

                _dbContext.Students.Add(new Models.DbModels.DbStudent
                {
                    ClassId = classId,
                    FullName = (string)student["Name"],
                    Sno = int.Parse((string)student["Sno"])
                });
            }

            await _dbContext.SaveChangesAsync();

            #region ShowDialog
            var dialog = new TaskDialog
            {
                WindowTitle = Localization.Resources.DialogTitle,
                MainInstruction = Localization.Resources.StudentCreateSuccess,
                MainIcon = TaskDialogIcon.Information,
                Content = $"{_students.Rows.Count} students created successfully",
                ExpandedInformation = $"{Localization.Resources.StudentOwner} \"{selectedRowElementArray.ElementAt(1)}\"",
                ButtonStyle = TaskDialogButtonStyle.CommandLinks
            };
            var ignoreButton = new TaskDialogButton(Localization.Resources.Continue_creating_student);
            var proceedButton = new TaskDialogButton(Localization.Resources.GoClass);
            dialog.Buttons.Add(ignoreButton);
            dialog.Buttons.Add(proceedButton);

            var result = dialog.ShowDialog(Window.GetWindow(this));

            if (result == proceedButton)
            {
                NavigationService.Navigate(new ClassDetailPage(classId));
            }
            #endregion
        }
    }
}
